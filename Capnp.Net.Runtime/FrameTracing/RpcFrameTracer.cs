using Capnp.Rpc;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Capnp.FrameTracing
{
    public class RpcFrameTracer : IFrameTracer
    {
        const string Header = "Ticks      | Thread     | Dir | Message";
        static readonly string HeaderSpace = new string(Enumerable.Repeat(' ', Header.Length).ToArray());

        readonly Stopwatch _timer = new Stopwatch();
        readonly StreamWriter _traceWriter;

        public RpcFrameTracer(StreamWriter traceWriter)
        {
            _traceWriter = traceWriter ?? throw new ArgumentNullException(nameof(traceWriter));
            _traceWriter.WriteLine(Header);
        }

        public void Dispose()
        {
            _traceWriter.WriteLine("<end of trace>");
            _traceWriter.Dispose();
        }

        void RenderMessageTarget(MessageTarget.READER target)
        {
            switch (target.which)
            {
                case MessageTarget.WHICH.ImportedCap:
                    _traceWriter.WriteLine($"on imported cap {target.ImportedCap}");
                    break;

                case MessageTarget.WHICH.PromisedAnswer:
                    _traceWriter.Write($"on promised answer {target.PromisedAnswer.QuestionId}");
                    if (target.PromisedAnswer.Transform != null)
                    {
                        _traceWriter.Write(": ");
                        _traceWriter.Write(string.Join(".", target.PromisedAnswer.Transform.Select(t => t.GetPointerField)));
                    }
                    _traceWriter.WriteLine();
                    break;
            }
        }

        void RenderCapDescriptor(CapDescriptor.READER desc)
        {
            _traceWriter.Write($" {desc.which : ##############}");
            switch (desc.which)
            {
                case CapDescriptor.WHICH.ReceiverAnswer:
                    _traceWriter.Write($" {desc.ReceiverAnswer}");
                    break;

                case CapDescriptor.WHICH.ReceiverHosted:
                    _traceWriter.Write($" {desc.SenderHosted}");
                    break;

                case CapDescriptor.WHICH.SenderPromise:
                    _traceWriter.Write($" {desc.SenderPromise}");
                    break;
            }
        }

        public void TraceFrame(FrameDirection direction, WireFrame frame)
        {
            if (!_timer.IsRunning)
            {
                _timer.Start();
            }

            _traceWriter.Write($@"{_timer.ElapsedTicks : ##########} | {Thread.CurrentThread.ManagedThreadId : ##########} | ");
            _traceWriter.Write(direction == FrameDirection.Tx ? "Tx  |" : "Rx  |");

            var dec = DeserializerState.CreateRoot(frame);
            var msg = Message.READER.create(dec);

            switch (msg.which)
            {
                case Message.WHICH.Abort:
                    _traceWriter.WriteLine($"ABORT {msg.Abort.Reason}");
                    break;

                case Message.WHICH.Bootstrap:
                    _traceWriter.WriteLine($"BOOTSTRAP {msg.Bootstrap.QuestionId}");

                    break;

                case Message.WHICH.Call:
                    _traceWriter.WriteLine($"CALL {msg.Call.QuestionId}, I: {msg.Call.InterfaceId:x} M: {msg.Call.MethodId}");
                    _traceWriter.Write(HeaderSpace);
                    RenderMessageTarget(msg.Call.Target);
                    _traceWriter.Write(HeaderSpace);
                    _traceWriter.WriteLine($"Send results to {msg.Call.SendResultsTo.which}");
                    break;

                case Message.WHICH.Disembargo:
                    _traceWriter.Write($"DISEMBARGO {msg.Disembargo.Context.which}");
                    switch (msg.Disembargo.Context.which)
                    {
                        case Disembargo.context.WHICH.Provide:
                            _traceWriter.Write($" {msg.Disembargo.Context.Provide}");
                            break;

                        case Disembargo.context.WHICH.ReceiverLoopback:
                            _traceWriter.Write($" {msg.Disembargo.Context.ReceiverLoopback}");
                            break;

                        case Disembargo.context.WHICH.SenderLoopback:
                            _traceWriter.Write($" {msg.Disembargo.Context.SenderLoopback}");
                            break;
                    }
                    _traceWriter.WriteLine(".");
                    RenderMessageTarget(msg.Disembargo.Target);
                    break;

                case Message.WHICH.Finish:
                    _traceWriter.WriteLine($"FINISH {msg.Finish.QuestionId}, release: {msg.Finish.ReleaseResultCaps}");
                    break;

                case Message.WHICH.Release:
                    _traceWriter.WriteLine($"RELEASE {msg.Release.Id}, count: {msg.Release.ReferenceCount}");
                    break;

                case Message.WHICH.Resolve:
                    _traceWriter.Write($"RESOLVE {msg.Resolve.PromiseId}: {msg.Resolve.which}");
                    switch (msg.Resolve.which)
                    {
                        case Resolve.WHICH.Cap:
                            RenderCapDescriptor(msg.Resolve.Cap);
                            _traceWriter.WriteLine(".");
                            break;

                        case Resolve.WHICH.Exception:
                            _traceWriter.WriteLine($" {msg.Resolve.Exception.Reason}");
                            break;
                    }
                    break;

                case Message.WHICH.Return:
                    _traceWriter.Write($"RETURN {msg.Return.AnswerId} {msg.Return.which}");
                    switch (msg.Return.which)
                    {
                        case Return.WHICH.Exception:
                            _traceWriter.Write($" {msg.Return.Exception.Reason}");
                            break;

                        case Return.WHICH.Results:
                            _traceWriter.Write($" {msg.Return.Results.}")
                    }
                    break;

                case Message.WHICH.Unimplemented:
                    break;

                case Message.WHICH.Accept:
                case Message.WHICH.Join:
                case Message.WHICH.Provide:
                    throw new RpcUnimplementedException();

                case Message.WHICH.ObsoleteDelete:
                case Message.WHICH.ObsoleteSave:
                default:
                    throw new RpcUnimplementedException();
            }
        }
    }
}
