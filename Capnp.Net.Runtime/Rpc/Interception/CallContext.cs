using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc.Interception
{

    /// <summary>
    /// Context of an intercepted call. Provides access to parameters and results, 
    /// and the possibility to redirect  the call to some other capability.
    /// </summary>
    public class CallContext
    {
        class PromisedAnswer : IPromisedAnswer
        {
            CallContext _callContext;
            TaskCompletionSource<DeserializerState> _futureResult = new TaskCompletionSource<DeserializerState>();

            public PromisedAnswer(CallContext callContext)
            {
                _callContext = callContext;
            }

            public Task<DeserializerState> WhenReturned => _futureResult.Task;

            async Task<Proxy> AccessWhenReturned(MemberAccessPath access)
            {
                await WhenReturned;
                return new Proxy(Access(access));
            }

            public ConsumedCapability Access(MemberAccessPath access)
            {
                if (_futureResult.Task.IsCompleted)
                {
                    try
                    {
                        return access.Eval(WhenReturned.Result);
                    }
                    catch (AggregateException exception)
                    {
                        throw exception.InnerException;
                    }
                }
                else
                {
                    return new LazyCapability(AccessWhenReturned(access));
                }
            }

            public void Dispose()
            {
            }

            public void Return()
            {
                if (_callContext.IsCanceled)
                {
                    _futureResult.SetCanceled();
                }
                else if (_callContext.Exception != null)
                {
                    _futureResult.SetException(new RpcException(_callContext.Exception));
                }
                else
                {
                    _futureResult.SetResult(_callContext.OutArgs);
                }
            }
        }

        public ulong InterfaceId { get; }
        public ushort MethodId { get; }
        public bool IsTailCall { get; }
        public InterceptionState State { get; private set; }
        public SerializerState InArgs { get; set; }
        public DeserializerState OutArgs { get; set; }
        public string Exception { get; set; }
        public bool IsCanceled { get; set; }
        public object Bob 
        {
            get => _bob;
            set
            {
                if (value != _bob)
                {
                    BobProxy?.Dispose();
                    BobProxy = null;

                    _bob = value;

                    switch (value)
                    {
                        case Proxy proxy:
                            BobProxy = proxy;
                            break;

                        case Skeleton skeleton:
                            BobProxy = CapabilityReflection.CreateProxy<object>(
                                LocalCapability.Create(skeleton));
                            break;

                        case ConsumedCapability cap:
                            BobProxy = CapabilityReflection.CreateProxy<object>(cap);
                            break;

                        case null:
                            break;

                        default:
                            BobProxy = CapabilityReflection.CreateProxy<object>(
                                LocalCapability.Create(
                                    Skeleton.GetOrCreateSkeleton(value, false)));
                            break;
                    }
                }
            }
        }

        internal Proxy BobProxy { get; private set; }

        readonly CensorCapability _censorCapability;
        PromisedAnswer _promisedAnswer;
        object _bob;

        internal IPromisedAnswer Answer => _promisedAnswer;

        internal CallContext(CensorCapability censorCapability, ulong interfaceId, ushort methodId, SerializerState inArgs)
        {
            _censorCapability = censorCapability;
            _promisedAnswer = new PromisedAnswer(this);
            
            Bob = censorCapability.InterceptedCapability;
            InterfaceId = interfaceId;
            MethodId = methodId;
            InArgs = inArgs;
            State = InterceptionState.RequestedFromAlice;
        }

        static void InterceptCaps(DeserializerState state, IInterceptionPolicy policy)
        {
            if (state.Caps != null)
            {
                for (int i = 0; i < state.Caps.Count; i++)
                {
                    state.Caps[i] = policy.Attach(state.Caps[i]);
                    state.Caps[i].AddRef();
                }
            }
        }

        static void UninterceptCaps(DeserializerState state, IInterceptionPolicy policy)
        {
            if (state.Caps != null)
            {
                for (int i = 0; i < state.Caps.Count; i++)
                {
                    state.Caps[i] = policy.Detach(state.Caps[i]);
                    state.Caps[i].AddRef();
                }
            }
        }

        public void InterceptInCaps(IInterceptionPolicy policyOverride = null)
        {
            InterceptCaps(InArgs, policyOverride ?? _censorCapability.Policy);
        }

        public void InterceptOutCaps(IInterceptionPolicy policyOverride = null)
        {
            InterceptCaps(OutArgs, policyOverride ?? _censorCapability.Policy);
        }

        public void UninterceptInCaps(IInterceptionPolicy policyOverride = null)
        {
            UninterceptCaps(InArgs, policyOverride ?? _censorCapability.Policy);
        }

        public void UninterceptOutCaps(IInterceptionPolicy policyOverride = null)
        {
            UninterceptCaps(OutArgs, policyOverride ?? _censorCapability.Policy);
        }

        public void ForwardToBob(CancellationToken cancellationToken = default)
        {
            if (Bob == null)
            {
                throw new InvalidOperationException("Bob is null");
            }
            
            var answer = BobProxy.Call(InterfaceId, MethodId, InArgs.Rewrap<DynamicSerializerState>(), IsTailCall, cancellationToken);

            State = InterceptionState.ForwardedToBob;

            async void ChangeStateWhenReturned()
            {
                using (answer)
                {
                    try
                    {
                        OutArgs = await answer.WhenReturned;
                    }
                    catch (TaskCanceledException)
                    {
                        IsCanceled = true;
                    }
                    catch (System.Exception exception)
                    {
                        Exception = exception.Message;
                    }
                }

                State = InterceptionState.ReturnedFromBob;

                _censorCapability.Policy.OnReturnFromBob(this);
            }

            ChangeStateWhenReturned();
        }

        public void ReturnToAlice()
        {
            try
            {
                _promisedAnswer.Return();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The call was already returned");
            }

            State = InterceptionState.ReturnedToAlice;
        }
    }
}
