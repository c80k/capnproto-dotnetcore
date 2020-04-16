using System;
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
            readonly CallContext _callContext;
            readonly TaskCompletionSource<DeserializerState> _futureResult = new TaskCompletionSource<DeserializerState>();
            readonly CancellationTokenSource _cancelFromAlice = new CancellationTokenSource();

            public PromisedAnswer(CallContext callContext)
            {
                _callContext = callContext;
            }

            public Task<DeserializerState> WhenReturned => _futureResult.Task;
            public CancellationToken CancelFromAlice => _cancelFromAlice.Token;

            public ConsumedCapability Access(MemberAccessPath access)
            {
                return _callContext._censorCapability.Policy.Attach<ConsumedCapability>(new LocalAnswerCapability(_futureResult.Task, access));
            }

            public ConsumedCapability Access(MemberAccessPath _, Task<IDisposable?> task)
            {
                var proxyTask = task.AsProxyTask();
                return _callContext._censorCapability.Policy.Attach<ConsumedCapability>(new LocalAnswerCapability(proxyTask));
            }

            public void Dispose()
            {
                try
                {
                    _cancelFromAlice.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // May happen when cancellation request from Alice arrives after return.
                }
            }

            public void Return()
            {
                try
                {
                    if (_callContext.ReturnCanceled)
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
                finally
                {
                    _cancelFromAlice.Dispose();
                }
            }

            public bool IsTailCall => false;
        }

        /// <summary>
        /// Target interface ID of this call
        /// </summary>
        public ulong InterfaceId { get; }

        /// <summary>
        /// Target method ID of this call
        /// </summary>
        public ushort MethodId { get; }

        /// <summary>
        /// Lifecycle state of this call
        /// </summary>
        public InterceptionState State { get; private set; }

        SerializerState _inArgs;

        /// <summary>
        /// Input arguments
        /// </summary>
        public SerializerState InArgs 
        {
            get => _inArgs;
            set { _inArgs = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        /// <summary>
        /// Output arguments ("return value")
        /// </summary>
        public DeserializerState OutArgs { get; set; }

        /// <summary>
        /// Exception text, or null if there is no exception
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// Whether the call should return in canceled state to Alice (the original caller).
        /// In case of forwarding (<see cref="ForwardToBob()"/>) the property is automatically set according 
        /// to the cancellation state of Bob's answer. However, you may override it:
        /// <list type="bullet">
        /// <item><description>Setting it from 'false' to 'true' means that we pretend Alice a canceled call.
        /// If Alice never requested cancellation this will surprise her pretty much.</description></item>
        /// <item><description>Setting it from 'true' to 'false' overrides an existing cancellation. Since
        /// we did not receive any output arguments from Bob (due to the cancellation), you *must* provide
        /// either <see cref="OutArgs"/> or <see cref="Exception"/>.</description></item>
        /// </list>
        /// </summary>
        public bool ReturnCanceled { get; set; }

        /// <summary>
        /// The cancellation token *from Alice* tells us when the original caller resigns from the call.
        /// </summary>
        public CancellationToken CancelFromAlice { get; private set; }

        /// <summary>
        /// The cancellation token *to Bob* tells the target capability when we resign from the forwarded call.
        /// It is initialized with <seealso cref="CancelFromAlice"/>. Override it to achieve different behaviors:
        /// E.g. set it to <code>CancellationToken.None</code> for "hiding" any cancellation request from Alice.
        /// Set it to <code>new CancellationToken(true)</code> to pretend Bob a cancellation request.
        /// </summary>
        public CancellationToken CancelToBob { get; set; }

        /// <summary>
        /// Target capability. May be one of the following:
        /// <list type="bullet">
        /// <item><description>Capability interface implementation</description></item>
        /// <item><description>A <see cref="Proxy"/>-derived object</description></item>
        /// <item><description>A <see cref="Skeleton"/>-derived object</description></item>
        /// <item><description>A <see cref="ConsumedCapability"/>-derived object (low level capability)</description></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Note that getting/setting this property does NOT transfer ownership.
        /// </remarks>
        public object Bob 
        {
            get => _bob;
            set
            {
                if (value != _bob)
                {
                    switch (value)
                    {
                        case null:
                            throw new ArgumentNullException(nameof(value));

                        case Proxy proxy:
                            BobProxy = proxy.Cast<BareProxy>(false);
                            break;

                        case ConsumedCapability cap: 
                            using (var temp = CapabilityReflection.CreateProxy<object>(cap)) 
                            {
                                Bob = temp; 
                            }
                            break;

                        case Skeleton skeleton:
                            using (var nullProxy = new Proxy())
                            {
                                Bob = (object?)skeleton.AsCapability() ?? nullProxy;
                            }
                            break;

                        default:
                            Bob = CapabilityReflection.CreateSkeletonInternal(value);
                            break;
                    }

                    _bob = value;
                }
            }
        }

        internal Proxy? BobProxy { get; private set; }

        readonly CensorCapability _censorCapability;
        PromisedAnswer _promisedAnswer;
        object _bob;

        internal IPromisedAnswer Answer => _promisedAnswer;

        internal CallContext(CensorCapability censorCapability, ulong interfaceId, ushort methodId, SerializerState inArgs)
        {
            _censorCapability = censorCapability;
            _promisedAnswer = new PromisedAnswer(this);
            _inArgs = inArgs;
            _bob = null!; // Will be initialized later here

            CancelFromAlice = _promisedAnswer.CancelFromAlice;
            CancelToBob = CancelFromAlice;
            Bob = censorCapability.InterceptedCapability;
            InterfaceId = interfaceId;
            MethodId = methodId;
            State = InterceptionState.RequestedFromAlice;
        }

        static void InterceptCaps(DeserializerState state, IInterceptionPolicy policy)
        {
            if (state.Caps != null)
            {
                for (int i = 0; i < state.Caps.Count; i++)
                {
                    var cap = state.Caps[i];
                    if (cap != null)
                    {
                        cap = policy.Attach(cap);
                        state.Caps[i] = cap;
                        cap.AddRef();
                    }
                }
            }
        }

        static void UninterceptCaps(DeserializerState state, IInterceptionPolicy policy)
        {
            if (state.Caps != null)
            {
                for (int i = 0; i < state.Caps.Count; i++)
                {
                    var cap = state.Caps[i];
                    if (cap != null)
                    {
                        cap = policy.Detach(cap);
                        state.Caps[i] = cap;
                        cap.AddRef();
                    }
                }
            }
        }

        /// <summary>
        /// Intercepts all capabilies inside the input arguments
        /// </summary>
        /// <param name="policyOverride">Policy to use, or null to further use present policy</param>
        public void InterceptInCaps(IInterceptionPolicy? policyOverride = null)
        {
            InterceptCaps(InArgs, policyOverride ?? _censorCapability.Policy);
        }

        /// <summary>
        /// Intercepts all capabilies inside the output arguments
        /// </summary>
        /// <param name="policyOverride">Policy to use, or null to further use present policy</param>
        public void InterceptOutCaps(IInterceptionPolicy? policyOverride = null)
        {
            InterceptCaps(OutArgs, policyOverride ?? _censorCapability.Policy);
        }

        /// <summary>
        /// Unintercepts all capabilies inside the input arguments
        /// </summary>
        /// <param name="policyOverride">Policy to remove, or null to remove present policy</param>
        public void UninterceptInCaps(IInterceptionPolicy? policyOverride = null)
        {
            UninterceptCaps(InArgs, policyOverride ?? _censorCapability.Policy);
        }

        /// <summary>
        /// Unintercepts all capabilies inside the output arguments
        /// </summary>
        /// <param name="policyOverride">Policy to remove, or null to remove present policy</param>
        public void UninterceptOutCaps(IInterceptionPolicy? policyOverride = null)
        {
            UninterceptCaps(OutArgs, policyOverride ?? _censorCapability.Policy);
        }

        /// <summary>
        /// Forwards this intercepted call to the target capability ("Bob").
        /// </summary>
        public void ForwardToBob()
        {
            var answer = BobProxy!.Call(InterfaceId, MethodId, InArgs.Rewrap<DynamicSerializerState>(), default, CancelToBob);

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
                        ReturnCanceled = true;
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

        /// <summary>
        /// Returns this intercepted call to the caller ("Alice").
        /// </summary>
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
