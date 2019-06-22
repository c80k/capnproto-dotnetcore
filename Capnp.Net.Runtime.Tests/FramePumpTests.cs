using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class FramePumpTests
    {
        class MyStruct : SerializerState
        {
            public MyStruct()
            {
                SetStruct(0, 1);
            }
        }

        [TestMethod]
        public void PipedFramePump()
        {
            int UnpackFrame(WireFrame frame)
            {
                int count = frame.Segments.Count;

                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual(i + 1, frame.Segments[i].Length);
                }

                return count;
            }

            WireFrame PackFrame(int value)
            {
                var segments = new Memory<ulong>[value];

                for (int i = 0; i < value; i++)
                {
                    ulong[] a = new ulong[i + 1];
                    segments[i] = new Memory<ulong>(a);
                }

                return new WireFrame(segments);
            }

            Thread rxRunner = null;

            using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.None))
            using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.ClientSafePipeHandle))
            using (var bc = new BlockingCollection<int>(8))
            {
                server.ReadMode = PipeTransmissionMode.Byte;
                client.ReadMode = PipeTransmissionMode.Byte;

                using (var txPump = new FramePump(server))
                using (var rxPump = new FramePump(client))
                {
                    rxRunner = new Thread(() =>
                    {
                        rxPump.Run();
                    });

                    rxPump.FrameReceived += f => bc.Add(UnpackFrame(f));

                    rxRunner.Start();

                    for (int i = 0; i < 100; i++)
                    {
                        txPump.Send(PackFrame(1));
                        txPump.Send(PackFrame(8));
                        txPump.Send(PackFrame(2));
                        txPump.Send(PackFrame(7));
                        txPump.Send(PackFrame(3));
                        txPump.Send(PackFrame(6));
                        txPump.Send(PackFrame(4));
                        txPump.Send(PackFrame(5));

                        Assert.IsTrue(SpinWait.SpinUntil(() => bc.Count == 8, 500));

                        Assert.AreEqual(1, bc.Take());
                        Assert.AreEqual(8, bc.Take());
                        Assert.AreEqual(2, bc.Take());
                        Assert.AreEqual(7, bc.Take());
                        Assert.AreEqual(3, bc.Take());
                        Assert.AreEqual(6, bc.Take());
                        Assert.AreEqual(4, bc.Take());
                        Assert.AreEqual(5, bc.Take());
                    }
                }
            }

            Assert.IsTrue(rxRunner.Join(500));
        }

        [TestMethod]
        public void FramePumpDeferredProcessing()
        {
            int UnpackAndVerifyFrame(WireFrame frame, int expectedCount)
            {
                int count = frame.Segments.Count;
                Assert.AreEqual(expectedCount, count);

                for (int i = 0; i < count; i++)
                {
                    int length = frame.Segments[i].Length;
                    Assert.AreEqual(expectedCount - i, length);
                    for (int j = 0; j < length; j++)
                    {
                        var expected = (ulong) (length - j);
                        var actual = frame.Segments[i].Span[j];
                        Assert.AreEqual(expected, actual);
                    }
                }

                return count;
            }

            WireFrame PackFrame(int value)
            {
                var segments = new Memory<ulong>[value];

                for (int i = 0; i < value; i++)
                {
                    ulong[] a = new ulong[value - i];
                    segments[i] = new Memory<ulong>(a);
                    for (int j = 0; j < a.Length; j++)
                    {
                        a[j] = (ulong)(a.Length - j);
                    }
                }

                return new WireFrame(segments);
            }

            Thread rxRunner = null;

            using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.None))
            using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.ClientSafePipeHandle))
            using (var bc = new BlockingCollection<WireFrame>(8))
            {
                server.ReadMode = PipeTransmissionMode.Byte;
                client.ReadMode = PipeTransmissionMode.Byte;

                using (var txPump = new FramePump(server))
                using (var rxPump = new FramePump(client))
                {
                    rxRunner = new Thread(() =>
                    {
                        rxPump.Run();
                    });

                    rxPump.FrameReceived += bc.Add;

                    rxRunner.Start();

                    txPump.Send(PackFrame(1));
                    txPump.Send(PackFrame(8));
                    txPump.Send(PackFrame(2));
                    txPump.Send(PackFrame(7));
                    txPump.Send(PackFrame(3));
                    txPump.Send(PackFrame(6));
                    txPump.Send(PackFrame(4));
                    txPump.Send(PackFrame(5));

                    Assert.IsTrue(SpinWait.SpinUntil(() => bc.Count == 8, 50000));

                    UnpackAndVerifyFrame(bc.Take(), 1);
                    UnpackAndVerifyFrame(bc.Take(), 8);
                    UnpackAndVerifyFrame(bc.Take(), 2);
                    UnpackAndVerifyFrame(bc.Take(), 7);
                    UnpackAndVerifyFrame(bc.Take(), 3);
                    UnpackAndVerifyFrame(bc.Take(), 6);
                    UnpackAndVerifyFrame(bc.Take(), 4);
                    UnpackAndVerifyFrame(bc.Take(), 5);
                }
            }

            Assert.IsTrue(rxRunner.Join(500));
        }
    }
}
