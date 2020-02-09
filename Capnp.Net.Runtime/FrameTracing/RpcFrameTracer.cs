using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Capnp.FrameTracing
{
    /// <summary>
    /// Default implementation of an RPC observer
    /// </summary>
    public class RpcFrameTracer : IFrameTracer
    {
        const string Header = "Ticks      | Thread     | Dir | Message";
        static readonly string HeaderSpace = new string(Enumerable.Repeat(' ', 30).ToArray()) + "|";

        readonly Stopwatch _timer = new Stopwatch();
        readonly TextWriter _traceWriter;

        /// <summary>
        /// Constructs an instance
        /// </summary>
        /// <param name="traceWriter">textual logging target</param>
        public RpcFrameTracer(TextWriter traceWriter)
        {
            _traceWriter = traceWriter ?? throw new ArgumentNullException(nameof(traceWriter));
            _traceWriter.WriteLine(Header);
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            _traceWriter.WriteLine("<end of trace>");
            _traceWriter.Dispose();
        }

        void RenderMessageTarget(MessageTarget.READER target, FrameDirection dir)
        {
            string tag;

            switch (target.which)
            {
                case MessageTarget.WHICH.ImportedCap:
                    tag = dir == FrameDirection.Tx ? "CR" : "CL";
                    _traceWriter.WriteLine($"on imported cap {tag}{target.ImportedCap}");
                    break;

                case MessageTarget.WHICH.PromisedAnswer:
                    tag = dir == FrameDirection.Tx ? "Q" : "A";
                    _traceWriter.Write($"on promised answer {tag}{target.PromisedAnswer.QuestionId}");
                    if (target.PromisedAnswer.Transform != null && target.PromisedAnswer.Transform.Count > 0)
                    {
                        _traceWriter.Write(": ");
                        _traceWriter.Write(string.Join(".", target.PromisedAnswer.Transform.Select(t => t.GetPointerField)));
                    }
                    _traceWriter.WriteLine();
                    break;
            }
        }

        void RenderCapDescriptor(CapDescriptor.READER desc, FrameDirection dir)
        {
            string tag;

            _traceWriter.Write($" {desc.which, 14}");
            switch (desc.which)
            {
                case CapDescriptor.WHICH.ReceiverAnswer:
                    tag = dir == FrameDirection.Tx ? "Q" : "A";
                    _traceWriter.Write($" {tag}{desc.ReceiverAnswer}");
                    break;

                case CapDescriptor.WHICH.ReceiverHosted:
                    tag = dir == FrameDirection.Tx ? "CR" : "CL";
                    _traceWriter.Write($" {tag}{desc.ReceiverHosted}");
                    break;

                case CapDescriptor.WHICH.SenderPromise:
                    tag = dir == FrameDirection.Tx ? "CL" : "CR";
                    _traceWriter.Write($" {tag}{desc.SenderPromise}");
                    break;

                case CapDescriptor.WHICH.SenderHosted:
                    tag = dir == FrameDirection.Tx ? "CL" : "CR";
                    _traceWriter.Write($" {tag}{desc.SenderHosted}");
                    break;
            }
        }

        void RenderCapTable(IEnumerable<CapDescriptor.READER> caps, FrameDirection dir)
        {
            foreach (var cap in caps)
            {
                _traceWriter.Write(HeaderSpace);
                RenderCapDescriptor(cap, dir);
                _traceWriter.WriteLine();
            }
        }

        /// <summary>
        /// Processes a sent or received RPC frame
        /// </summary>
        /// <param name="dir">frame direction</param>
        /// <param name="frame">actual frame</param>
        public void TraceFrame(FrameDirection dir, WireFrame frame)
        {
            if (!_timer.IsRunning)
            {
                _timer.Start();
            }

            _traceWriter.Write($@"{_timer.ElapsedTicks, 10} | {Thread.CurrentThread.ManagedThreadId, 10} | ");
            _traceWriter.Write(dir == FrameDirection.Tx ? "Tx  |" : "Rx  |");

            var dec = DeserializerState.CreateRoot(frame);
            var msg = Message.READER.create(dec);
            string tag;

            switch (msg.which)
            {
                case Message.WHICH.Abort:
                    _traceWriter.WriteLine($"ABORT {msg.Abort.Reason}");
                    break;

                case Message.WHICH.Bootstrap:
                    tag = dir == FrameDirection.Tx ? "Q" : "A";
                    _traceWriter.WriteLine($"BOOTSTRAP {tag}{msg.Bootstrap.QuestionId}");
                    break;

                case Message.WHICH.Call:
                    tag = dir == FrameDirection.Tx ? "Q" : "A";
                    _traceWriter.Write($"CALL {tag}{msg.Call.QuestionId}, I: {msg.Call.InterfaceId:x} M: {msg.Call.MethodId} ");
                    RenderMessageTarget(msg.Call.Target, dir);
                    _traceWriter.Write(HeaderSpace);
                    _traceWriter.WriteLine($"Send results to {msg.Call.SendResultsTo.which}");
                    RenderCapTable(msg.Call.Params.CapTable, dir);
                    break;

                case Message.WHICH.Disembargo:
                    _traceWriter.Write($"DISEMBARGO {msg.Disembargo.Context.which}");
                    switch (msg.Disembargo.Context.which)
                    {
                        case Disembargo.context.WHICH.Provide:
                            _traceWriter.Write($" {msg.Disembargo.Context.Provide}");
                            break;

                        case Disembargo.context.WHICH.ReceiverLoopback:
                            _traceWriter.Write($" E{msg.Disembargo.Context.ReceiverLoopback}");
                            break;

                        case Disembargo.context.WHICH.SenderLoopback:
                            _traceWriter.Write($" E{msg.Disembargo.Context.SenderLoopback}");
                            break;
                    }
                    _traceWriter.WriteLine(".");
                    _traceWriter.Write(HeaderSpace);
                    RenderMessageTarget(msg.Disembargo.Target, dir);
                    break;

                case Message.WHICH.Finish:
                    tag = dir == FrameDirection.Tx ? "Q" : "A";
                    _traceWriter.WriteLine($"FINISH {tag}{msg.Finish.QuestionId}, release: {msg.Finish.ReleaseResultCaps}");
                    break;

                case Message.WHICH.Release:
                    tag = dir == FrameDirection.Tx ? "CR" : "CL";
                    _traceWriter.WriteLine($"RELEASE {tag}{msg.Release.Id}, count: {msg.Release.ReferenceCount}");
                    break;

                case Message.WHICH.Resolve:
                    tag = dir == FrameDirection.Tx ? "CL" : "CR";
                    _traceWriter.Write($"RESOLVE {tag}{msg.Resolve.PromiseId}: {msg.Resolve.which}");
                    switch (msg.Resolve.which)
                    {
                        case Resolve.WHICH.Cap:
                            RenderCapDescriptor(msg.Resolve.Cap, dir);
                            _traceWriter.WriteLine(".");
                            break;

                        case Resolve.WHICH.Exception:
                            _traceWriter.WriteLine($" {msg.Resolve.Exception.Reason}");
                            break;
                    }
                    break;

                case Message.WHICH.Return:
                    tag = dir == FrameDirection.Tx ? "A" : "Q";
                    _traceWriter.Write($"RETURN {tag}{msg.Return.AnswerId} {msg.Return.which}");
                    switch (msg.Return.which)
                    {
                        case Return.WHICH.Exception:
                            _traceWriter.WriteLine($" {msg.Return.Exception.Reason}");
                            break;

                        case Return.WHICH.Results:
                            _traceWriter.WriteLine($", release: {msg.Return.ReleaseParamCaps}");
                            RenderCapTable(msg.Return.Results.CapTable, dir);
                            break;

                        case Return.WHICH.TakeFromOtherQuestion:
                            tag = dir == FrameDirection.Tx ? "Q" : "A";
                            _traceWriter.WriteLine($" {tag}{msg.Return.TakeFromOtherQuestion}");
                            break;

                        default:
                            _traceWriter.WriteLine();
                            break;
                    }
                    break;

                case Message.WHICH.Unimplemented:
                    _traceWriter.WriteLine($"UNIMPLEMENTED {msg.Unimplemented.which}");
                    break;

                case Message.WHICH.Accept:
                    _traceWriter.WriteLine("ACCEPT");
                    break;

                case Message.WHICH.Join:
                    _traceWriter.WriteLine("JOIN");
                    break;

                case Message.WHICH.Provide:
                    _traceWriter.WriteLine($"PROVIDE {msg.Provide.QuestionId}");
                    RenderMessageTarget(msg.Provide.Target, dir);
                    break;

                case Message.WHICH.ObsoleteDelete:
                    _traceWriter.WriteLine("OBSOLETEDELETE");
                    break;

                case Message.WHICH.ObsoleteSave:
                    _traceWriter.WriteLine("OBSOLETESAVE");
                    break;

                default:
                    _traceWriter.WriteLine($"Unknown message {msg.which}");
                    break;

            }
        }
    }
}