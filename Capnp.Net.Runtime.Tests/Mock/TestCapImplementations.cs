using Capnp.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Capnproto_test.Capnp.Test;
using CapnpGen;

namespace Capnp.Net.Runtime.Tests.GenImpls
{
    class Common
    {
        static byte[] Data(string s) => System.Text.Encoding.UTF8.GetBytes(s);

        static bool DataSequenceEqual(IEnumerable<IReadOnlyList<byte>> seq1, IEnumerable<IReadOnlyList<byte>> seq2)
        {
            return seq1.Zip(seq2, (s1, s2) => s1.SequenceEqual(s2)).All(_ => _);
        }

        public static void InitTestMessage(TestAllTypes s)
        {
            s.BoolField = true;
            s.Int8Field = -123;
            s.Int16Field = -12345;
            s.Int32Field = -12345678;
            s.Int64Field = -123456789012345;
            s.UInt8Field = 234;
            s.UInt16Field = 45678;
            s.UInt32Field = 3456789012;
            s.UInt64Field = 12345678901234567890;
            s.Float32Field = 1234.5f;
            s.Float64Field = -123e45;
            s.TextField = "foo";
            s.DataField = Data("bar");
            {
                s.StructField = new TestAllTypes();
                var sub = s.StructField;
                sub.BoolField = true;
                sub.Int8Field = -12;
                sub.Int16Field = 3456;
                sub.Int32Field = -78901234;
                sub.Int64Field = 56789012345678;
                sub.UInt8Field = 90;
                sub.UInt16Field = 1234;
                sub.UInt32Field = 56789012;
                sub.UInt64Field = 345678901234567890;
                sub.Float32Field = -1.25e-10f;
                sub.Float64Field = 345;
                sub.TextField = "baz";
                sub.DataField = Data("qux");
                {
                    sub.StructField = new TestAllTypes()
                    {
                        TextField = "nested",
                        StructField = new TestAllTypes()
                        {
                            TextField = "really nested"
                        }
                    };
                }
                sub.EnumField = TestEnum.baz;
                sub.VoidList = 3;
                sub.BoolList = new bool[] { false, true, false, true, true };
                sub.Int8List = new sbyte[] { 12, -34, -0x80, 0x7f };
                sub.Int16List = new short[] { 1234, -5678, -0x8000, 0x7fff };
                sub.Int32List = new int[] { 12345678, -90123456, -0x7fffffff - 1, 0x7fffffff };
                sub.Int64List = new long[] { 123456789012345, -678901234567890, -0x7fffffffffffffff - 1, 0x7fffffffffffffff };
                sub.UInt8List = new byte[] { 12, 34, 0, 0xff };
                sub.UInt16List = new ushort[] { 1234, 5678, 0, 0xffff };
                sub.UInt32List = new uint[] { 12345678u, 90123456u, 0u, 0xffffffffu };
                sub.UInt64List = new ulong[] { 123456789012345, 678901234567890, 0, 0xffffffffffffffff };
                sub.Float32List = new float[] { 0, 1234567, 1e37f, -1e37f, 1e-37f, -1e-37f };
                sub.Float64List = new double[] { 0, 123456789012345, 1e306, -1e306, 1e-306, -1e-306 };
                sub.TextList = new string[] { "quux", "corge", "grault" };
                sub.DataList = new byte[][] { Data("garply"), Data("waldo"), Data("fred") };
                sub.StructList = new TestAllTypes[]
                {
                    new TestAllTypes() { TextField = "x structlist 1" },
                    new TestAllTypes() { TextField = "x structlist 2" },
                    new TestAllTypes() { TextField = "x structlist 3" }
                };
                sub.EnumList = new TestEnum[] { TestEnum.qux, TestEnum.bar, TestEnum.grault };
            }
            s.EnumField = TestEnum.corge;
            s.VoidList = 6;
            s.BoolList = new bool[] { true, false, false, true };
            s.Int8List = new sbyte[] { 111, -111 };
            s.Int16List = new short[] { 11111, -11111 };
            s.Int32List = new int[] { 111111111, -111111111 };
            s.Int64List = new long[] { 1111111111111111111, -1111111111111111111 };
            s.UInt8List = new byte[] { 111, 222 };
            s.UInt16List = new ushort[] { 33333, 44444 };
            s.UInt32List = new uint[] { 3333333333 };
            s.UInt64List = new ulong[] { 11111111111111111111 };
            s.Float32List = new float[] { 5555.5f, float.PositiveInfinity, float.NegativeInfinity, float.NaN };
            s.Float64List = new double[] { 7777.75, double.PositiveInfinity, double.NegativeInfinity, double.NaN };
            s.TextList = new string[] { "plugh", "xyzzy", "thud" };
            s.DataList = new byte[][] { Data("oops"), Data("exhausted"), Data("rfc3092") };
            {
                s.StructList = new TestAllTypes[]
                {
                    new TestAllTypes() { TextField = "structlist 1" },
                    new TestAllTypes() { TextField = "structlist 2" },
                    new TestAllTypes() { TextField = "structlist 3" }
                };
            }
            s.EnumList = new TestEnum[] { TestEnum.foo, TestEnum.garply };
        }

        public static void CheckTestMessage(TestAllTypes s)
        {
            var sub = s.StructField;
            Assert.IsTrue(sub.BoolField);
            Assert.AreEqual(-12, sub.Int8Field);
            Assert.AreEqual(3456, sub.Int16Field);
            Assert.AreEqual(-78901234, sub.Int32Field);
            Assert.AreEqual(56789012345678, sub.Int64Field);
            Assert.AreEqual(90, sub.UInt8Field);
            Assert.AreEqual(1234, sub.UInt16Field);
            Assert.AreEqual(56789012u, sub.UInt32Field);
            Assert.AreEqual(345678901234567890ul, sub.UInt64Field);
            Assert.AreEqual(-1.25e-10f, sub.Float32Field);
            Assert.AreEqual(345.0, sub.Float64Field);
            Assert.AreEqual("baz", sub.TextField);
            Assert.IsTrue(Data("qux").SequenceEqual(sub.DataField));
            {
                var subsub = sub.StructField;
                Assert.AreEqual("nested", subsub.TextField);
                Assert.AreEqual("really nested", subsub.StructField.TextField);
            }
            Assert.AreEqual(TestEnum.baz, sub.EnumField);
            Assert.AreEqual(3, sub.VoidList);
            Assert.IsTrue(sub.BoolList.SequenceEqual(new bool[] { false, true, false, true, true }));
            Assert.IsTrue(sub.Int8List.SequenceEqual(new sbyte[] { 12, -34, -0x80, 0x7f }));
            Assert.IsTrue(sub.Int16List.SequenceEqual(new short[] { 1234, -5678, -0x8000, 0x7fff }));
            Assert.IsTrue(sub.Int32List.SequenceEqual(new int[] { 12345678, -90123456, -0x7fffffff - 1, 0x7fffffff }));
            Assert.IsTrue(sub.Int64List.SequenceEqual(new long[] { 123456789012345, -678901234567890, -0x7fffffffffffffff - 1, 0x7fffffffffffffff }));
            Assert.IsTrue(sub.UInt8List.SequenceEqual(new byte[] { 12, 34, 0, 0xff }));
            Assert.IsTrue(sub.UInt16List.SequenceEqual(new ushort[] { 1234, 5678, 0, 0xffff }));
            Assert.IsTrue(sub.UInt32List.SequenceEqual(new uint[] { 12345678, 90123456, 0u, 0xffffffff }));
            Assert.IsTrue(sub.UInt64List.SequenceEqual(new ulong[] { 123456789012345, 678901234567890, 0, 0xffffffffffffffff }));
            Assert.IsTrue(sub.Float32List.SequenceEqual(new float[] { 0.0f, 1234567.0f, 1e37f, -1e37f, 1e-37f, -1e-37f }));
            Assert.IsTrue(sub.Float64List.SequenceEqual(new double[] { 0.0, 123456789012345.0, 1e306, -1e306, 1e-306, -1e-306 }));
            Assert.IsTrue(sub.TextList.SequenceEqual(new string[] { "quux", "corge", "grault" }));
            Assert.IsTrue(DataSequenceEqual(sub.DataList, new byte[][] { Data("garply"), Data("waldo"), Data("fred") }));
            {
                var list = sub.StructList;
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual("x structlist 1", list[0].TextField);
                Assert.AreEqual("x structlist 2", list[1].TextField);
                Assert.AreEqual("x structlist 3", list[2].TextField);
            }
            Assert.IsTrue(sub.EnumList.SequenceEqual(new TestEnum[] { TestEnum.qux, TestEnum.bar, TestEnum.grault }));
            Assert.AreEqual(TestEnum.corge, s.EnumField);

            Assert.AreEqual(6, s.VoidList);
            Assert.IsTrue(s.BoolList.SequenceEqual(new bool[] { true, false, false, true }));
            Assert.IsTrue(s.Int8List.SequenceEqual(new sbyte[] { 111, -111 }));
            Assert.IsTrue(s.Int16List.SequenceEqual(new short[] { 11111, -11111 }));
            Assert.IsTrue(s.Int32List.SequenceEqual(new int[] { 111111111, -111111111 }));
            Assert.IsTrue(s.Int64List.SequenceEqual(new long[] { 1111111111111111111, -1111111111111111111 }));
            Assert.IsTrue(s.UInt8List.SequenceEqual(new byte[] { 111, 222 }));
            Assert.IsTrue(s.UInt16List.SequenceEqual(new ushort[] { 33333, 44444 }));
            Assert.IsTrue(s.UInt32List.SequenceEqual(new uint[] { 3333333333 }));
            Assert.IsTrue(s.UInt64List.SequenceEqual(new ulong[] { 11111111111111111111 }));
            {
                var list = s.Float32List;
                Assert.AreEqual(4, list.Count);
                Assert.AreEqual(5555.5f, list[0]);
                Assert.AreEqual(float.PositiveInfinity, list[1]);
                Assert.AreEqual(float.NegativeInfinity, list[2]);
                Assert.IsTrue(float.IsNaN(list[3]));
            }
            {
                var list = s.Float64List;
                Assert.AreEqual(4, list.Count);
                Assert.AreEqual(7777.75, list[0]);
                Assert.IsTrue(double.IsPositiveInfinity(list[1]));
                Assert.IsTrue(double.IsNegativeInfinity(list[2]));
                Assert.IsTrue(double.IsNaN(list[3]));
            }
            Assert.IsTrue(s.TextList.SequenceEqual(new string[] { "plugh", "xyzzy", "thud" }));
            Assert.IsTrue(DataSequenceEqual(s.DataList, new byte[][] { Data("oops"), Data("exhausted"), Data("rfc3092") }));
            {
                var list = s.StructList;
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual("structlist 1", list[0].TextField);
                Assert.AreEqual("structlist 2", list[1].TextField);
                Assert.AreEqual("structlist 3", list[2].TextField);
            }
            Assert.IsTrue(s.EnumList.SequenceEqual(new TestEnum[] { TestEnum.foo, TestEnum.garply }));
        }

        public static void CheckTestMessageAllZero(TestAllTypes s)
        {
            Assert.IsFalse(s.BoolField);
            Assert.AreEqual(0, s.Int8Field);
            Assert.AreEqual(0, s.Int16Field);
            Assert.AreEqual(0, s.Int32Field);
            Assert.AreEqual(0, s.Int64Field);
            Assert.AreEqual(0, s.UInt8Field);
            Assert.AreEqual(0, s.UInt16Field);
            Assert.AreEqual(0, s.UInt32Field);
            Assert.AreEqual(0, s.UInt64Field);
            Assert.AreEqual(0f, s.Float32Field);
            Assert.AreEqual(0.0, s.Float64Field);
            Assert.AreEqual(string.Empty, s.TextField);
            Assert.IsTrue(Data(string.Empty).SequenceEqual(s.DataField));
            {
                var sub = s.StructField;
                Assert.IsFalse(sub.BoolField);
                Assert.AreEqual(0, sub.Int8Field);
                Assert.AreEqual(0, sub.Int16Field);
                Assert.AreEqual(0, sub.Int32Field);
                Assert.AreEqual(0, sub.Int64Field);
                Assert.AreEqual(0, sub.UInt8Field);
                Assert.AreEqual(0, sub.UInt16Field);
                Assert.AreEqual(0, sub.UInt32Field);
                Assert.AreEqual(0, sub.UInt64Field);
                Assert.AreEqual(0f, sub.Float32Field);
                Assert.AreEqual(0.0, sub.Float64Field);
                Assert.AreEqual(string.Empty, sub.TextField);
                Assert.AreEqual(Data(string.Empty), sub.DataField);
                {
                    var subsub = sub.StructField;
                    Assert.AreEqual(string.Empty, subsub.TextField);
                    Assert.AreEqual(string.Empty, subsub.StructField.TextField);
                }
                Assert.AreEqual(0, sub.VoidList);
                Assert.AreEqual(0, sub.BoolList.Count);
                Assert.AreEqual(0, sub.Int8List.Count);
                Assert.AreEqual(0, sub.Int16List.Count);
                Assert.AreEqual(0, sub.Int32List.Count);
                Assert.AreEqual(0, sub.Int64List.Count);
                Assert.AreEqual(0, sub.UInt8List.Count);
                Assert.AreEqual(0, sub.UInt16List.Count);
                Assert.AreEqual(0, sub.UInt32List.Count);
                Assert.AreEqual(0, sub.UInt64List.Count);
                Assert.AreEqual(0, sub.Float32List.Count);
                Assert.AreEqual(0, sub.Float64List.Count);
                Assert.AreEqual(0, sub.TextList.Count);
                Assert.AreEqual(0, sub.DataList.Count);
                Assert.AreEqual(0, sub.StructList.Count);
            }

            Assert.AreEqual(0, s.VoidList);
            Assert.AreEqual(0, s.BoolList.Count);
            Assert.AreEqual(0, s.Int8List.Count);
            Assert.AreEqual(0, s.Int16List.Count);
            Assert.AreEqual(0, s.Int32List.Count);
            Assert.AreEqual(0, s.Int64List.Count);
            Assert.AreEqual(0, s.UInt8List.Count);
            Assert.AreEqual(0, s.UInt16List.Count);
            Assert.AreEqual(0, s.UInt32List.Count);
            Assert.AreEqual(0, s.UInt64List.Count);
            Assert.AreEqual(0, s.Float32List.Count);
            Assert.AreEqual(0, s.Float64List.Count);
            Assert.AreEqual(0, s.TextList.Count);
            Assert.AreEqual(0, s.DataList.Count);
            Assert.AreEqual(0, s.StructList.Count);
        }

        public static void InitListDefaults(TestLists lists)
        {
            lists.List0 = new TestLists.Struct0[]
            {
                new TestLists.Struct0(),
                new TestLists.Struct0()
            };
            lists.List1 = new TestLists.Struct1[]
            {
                new TestLists.Struct1() { F = true },
                new TestLists.Struct1() { F = false },
                new TestLists.Struct1() { F = true },
                new TestLists.Struct1() { F = true }
            };
            lists.List8 = new TestLists.Struct8[]
            {
                new TestLists.Struct8() { F = 123 },
                new TestLists.Struct8() { F = 45 }
            };
            lists.List16 = new TestLists.Struct16[]
            {
                new TestLists.Struct16() { F = 12345 },
                new TestLists.Struct16() { F = 6789 }
            };
            lists.List32 = new TestLists.Struct32[]
            {
                new TestLists.Struct32() { F = 123456789 },
                new TestLists.Struct32() { F = 234567890 }
            };
            lists.List64 = new TestLists.Struct64[]
            {
                new TestLists.Struct64() { F = 1234567890123456 },
                new TestLists.Struct64() { F = 2345678901234567}
            };
            lists.ListP = new TestLists.StructP[]
            {
                new TestLists.StructP() { F = "foo" },
                new TestLists.StructP() { F = "bar" }
            };

            lists.Int32ListList = new int[][]
            {
                new int[] { 1, 2, 3 },
                new int[] { 4, 5 },
                new int[] { 12341234 }
            };

            lists.TextListList = new string[][]
            {
                new string[] { "foo", "bar" },
                new string[] { "baz" },
                new string[] { "qux", "corge" }
            };

            lists.StructListList = new TestAllTypes[][]
            {
                new TestAllTypes[]
                {
                    new TestAllTypes() { Int32Field = 123 },
                    new TestAllTypes() { Int32Field = 456 }
                },
                new TestAllTypes[]
                {
                    new TestAllTypes() { Int32Field = 789 }
                }
            };
        }

        public static void CheckListDefault(TestLists lists)
        {
            Assert.AreEqual(2, lists.List0.Count);
            Assert.AreEqual(4, lists.List1.Count);
            Assert.AreEqual(2, lists.List8.Count);
            Assert.AreEqual(2, lists.List16.Count);
            Assert.AreEqual(2, lists.List32.Count);
            Assert.AreEqual(2, lists.List64.Count);
            Assert.AreEqual(2, lists.ListP.Count);

            Assert.IsTrue(lists.List1[0].F);
            Assert.IsFalse(lists.List1[1].F);
            Assert.IsTrue(lists.List1[2].F);
            Assert.IsTrue(lists.List1[3].F);

            Assert.AreEqual(123, lists.List8[0].F);
            Assert.AreEqual(45, lists.List8[1].F);

            Assert.AreEqual(12345, lists.List16[0].F);
            Assert.AreEqual(6789, lists.List16[1].F);

            Assert.AreEqual(123456789, lists.List32[0].F);
            Assert.AreEqual(234567890, lists.List32[1].F);

            Assert.AreEqual(1234567890123456, lists.List64[0].F);
            Assert.AreEqual(2345678901234567, lists.List64[1].F);

            Assert.AreEqual("foo", lists.ListP[0].F);
            Assert.AreEqual("bar", lists.ListP[1].F);

            Assert.AreEqual(3, lists.Int32ListList.Count);
            Assert.IsTrue(lists.Int32ListList[0].SequenceEqual(new int[] { 1, 2, 3 }));
            Assert.IsTrue(lists.Int32ListList[1].SequenceEqual(new int[] { 4, 5 }));
            Assert.IsTrue(lists.Int32ListList[2].SequenceEqual(new int[] { 12341234 }));

            Assert.AreEqual(3, lists.TextListList.Count);
            Assert.IsTrue(lists.TextListList[0].SequenceEqual(new string[] { "foo", "bar" }));
            Assert.IsTrue(lists.TextListList[1].SequenceEqual(new string[] { "baz" }));
            Assert.IsTrue(lists.TextListList[2].SequenceEqual(new string[] { "qux", "corge" }));

            Assert.AreEqual(2, lists.StructListList.Count);
            Assert.AreEqual(2, lists.StructListList[0].Count);
            Assert.AreEqual(123, lists.StructListList[0][0]);
            Assert.AreEqual(456, lists.StructListList[0][1]);
            Assert.AreEqual(1, lists.StructListList[1].Count);
            Assert.AreEqual(789, lists.StructListList[1][0]);
        }
    }

    class Counters
    {
        public int CallCount;
        public int HandleCount;
    }

    #region TestInterface
    class TestInterfaceImpl : ITestInterface
    {
        readonly TaskCompletionSource<int> _tcs;
        protected readonly Counters _counters;

        public TestInterfaceImpl(Counters counters, TaskCompletionSource<int> tcs)
        {
            _tcs = tcs;
            _counters = counters;
        }

        public TestInterfaceImpl(Counters counters)
        {
            _counters = counters;
        }

        public Task Bar(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Baz(TestAllTypes s, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _counters.CallCount);
            Common.CheckTestMessage(s);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tcs?.SetResult(0);
        }

        public virtual Task<string> Foo(uint i, bool j, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _counters.CallCount);
            Assert.AreEqual(123u, i);
            Assert.IsTrue(j);
            return Task.FromResult("foo");
        }
    }

    class TestInterfaceImpl2 : ITestInterface
    {
        public Task Bar(CancellationToken cancellationToken_ = default)
        {
            throw new NotImplementedException();
        }

        public Task Baz(TestAllTypes s, CancellationToken cancellationToken_ = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }

        public Task<string> Foo(uint i, bool j, CancellationToken cancellationToken_ = default)
        {
            Assert.AreEqual(123u, i);
            Assert.IsTrue(j);
            return Task.FromResult("bar");
        }
    }

    #endregion TestInterface

    #region TestExtends
    class TestExtendsImpl : TestInterfaceImpl, ITestExtends
    {
        public TestExtendsImpl(Counters counters) : base(counters)
        {
        }

        public override Task<string> Foo(uint i, bool j, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _counters.CallCount);
            Assert.AreEqual(321u, i);
            Assert.IsFalse(j);
            return Task.FromResult("bar");
        }

        public Task Corge(TestAllTypes s, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TestAllTypes> Grault(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _counters.CallCount);
            var result = new TestAllTypes();
            Common.InitTestMessage(result);
            return Task.FromResult(result);
        }

        public Task Qux(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    #endregion TestExtends

    #region TestPipeline
    class TestPipelineImpl : ITestPipeline
    {
        protected readonly Counters _counters;

        public TestPipelineImpl(Counters counters)
        {
            _counters = counters;
        }

        public void Dispose()
        {
        }

        public async Task<(string, TestPipeline.AnyBox)> GetAnyCap(uint n, BareProxy inCap, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);
            Assert.AreEqual(234u, n);
            var s = await inCap.Cast<ITestInterface>(true).Foo(123, true, cancellationToken_);
            Assert.AreEqual("foo", s);
            return ("bar", new TestPipeline.AnyBox() { Cap = BareProxy.FromImpl(new TestExtendsImpl(_counters)) });
        }

        public async Task<(string, TestPipeline.Box)> GetCap(uint n, ITestInterface inCap, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);
            Assert.AreEqual(234u, n);
            var s = await inCap.Foo(123, true, cancellationToken_);
            Assert.AreEqual("foo", s);
            return ("bar", new TestPipeline.Box() { Cap = new TestExtendsImpl(_counters) });
        }

        public Task TestPointers(ITestInterface cap, object obj, IReadOnlyList<ITestInterface> list, CancellationToken cancellationToken_)
        {
            throw new NotImplementedException();
        }
    }

    class TestPipelineImpl2 : ITestPipeline
    {
        readonly Task _deblock;
        readonly TestInterfaceImpl2 _timpl2;

        public TestPipelineImpl2(Task deblock)
        {
            _deblock = deblock;
            _timpl2 = new TestInterfaceImpl2();
        }

        public void Dispose()
        {
        }

        public bool IsChildCapDisposed => _timpl2.IsDisposed;

        public Task<(string, TestPipeline.AnyBox)> GetAnyCap(uint n, BareProxy inCap, CancellationToken cancellationToken_ = default)
        {
            throw new NotImplementedException();
        }

        public async Task<(string, TestPipeline.Box)> GetCap(uint n, ITestInterface inCap, CancellationToken cancellationToken_ = default)
        {
            await _deblock;
            return ("hello", new TestPipeline.Box() { Cap = _timpl2 });
        }

        public Task TestPointers(ITestInterface cap, object obj, IReadOnlyList<ITestInterface> list, CancellationToken cancellationToken_ = default)
        {
            throw new NotImplementedException();
        }
    }

    #endregion TestPipeline

    #region TestCallOrder
    class TestCallOrderImpl : ITestCallOrder
    {
        readonly object _lock = new object();
        uint _counter;

        ILogger Logger { get; } = Logging.CreateLogger<TestCallOrderImpl>();

        public uint? CountToDispose { get; set; }

        public void Dispose()
        {
            lock (_lock)
            {                
                Assert.IsTrue(!CountToDispose.HasValue || _counter == CountToDispose, $"Must not dispose at this point: {_counter} {Thread.CurrentThread.Name}");
            }
        }

        public Task<uint> GetCallSequence(uint expected, CancellationToken cancellationToken_)
        {
            lock (_lock)
            {
                return Task.FromResult(_counter++);
            }
        }

        public uint Count
        {
            get
            {
                lock (_lock)
                {
                    return _counter;
                }
            }
        }
    }
    #endregion TestCallOrder

    #region TestTailCaller
    class TestTailCaller : ITestTailCaller
    {
        public void Dispose()
        {
        }

        public Task<TestTailCallee.TailResult> Foo(int i, ITestTailCallee callee, CancellationToken cancellationToken_)
        {
            return callee.Foo(i, "from TestTailCaller", cancellationToken_);
        }
    }
    #endregion TestTailCaller

    #region TestTailCaller
    class TestTailCallerImpl : ITestTailCaller
    {
        readonly Counters _counters;

        public TestTailCallerImpl(Counters counters)
        {
            _counters = counters;
        }

        public void Dispose()
        {
        }

        public Task<TestTailCallee.TailResult> Foo(int i, ITestTailCallee callee, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);

            using (callee)
            {
                return callee.Foo(i, "from TestTailCaller", cancellationToken_);
            }
        }
    }
    #endregion TestTailCaller

    #region TestTailCallee
    class TestTailCalleeImpl : ITestTailCallee
    {
        readonly Counters _counters;

        public TestTailCalleeImpl(Counters counters)
        {
            _counters = counters;
        }

        public void Dispose()
        {
        }

        public Task<TestTailCallee.TailResult> Foo(int i, string t, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);

            var result = new TestTailCallee.TailResult()
            {
                I = (uint)i,
                T = t,
                C = new TestCallOrderImpl()
            };
            return Task.FromResult(result);
        }
    }
    #endregion TestTailCallee

    #region TestMoreStuff
    class TestMoreStuffImpl : ITestMoreStuff
    {
        readonly TaskCompletionSource<int> _echoAllowed = new TaskCompletionSource<int>();
        readonly Counters _counters;

        public TestMoreStuffImpl(Counters counters)
        {
            _counters = counters;
        }

        public ITestInterface ClientToHold { get; set; }

        public void EnableEcho()
        {
            _echoAllowed.SetResult(0);
        }

        public async Task<string> CallFoo(ITestInterface cap, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);
            using (cap)
            {
                string s = await cap.Foo(123, true, cancellationToken_);
                Assert.AreEqual("foo", s);
            }
            return "bar";
        }

        public async Task<string> CallFooWhenResolved(ITestInterface cap, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);
            await ((Proxy)cap).WhenResolved;
            string s = await cap.Foo(123, true, cancellationToken_);
            Assert.AreEqual("foo", s);
            return "bar";
        }

        public async Task<string> CallHeld(CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);

            string s = await ClientToHold.Foo(123, true, cancellationToken_);
            Assert.AreEqual("foo", s);
            return "bar";
        }

        public void Dispose()
        {
            ClientToHold?.Dispose();
        }

        public Task<ITestCallOrder> Echo(ITestCallOrder cap, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);
            return Task.FromResult(cap);
        }

        public Task ExpectCancel(ITestInterface cap, CancellationToken cancellationToken_)
        {
            return NeverReturn(cap, cancellationToken_);
        }

        public Task<uint> GetCallSequence(uint expected, CancellationToken cancellationToken_)
        {
            return Task.FromResult((uint)(Interlocked.Increment(ref _counters.CallCount) - 1));
        }

        public Task<string> GetEnormousString(CancellationToken cancellationToken_)
        {
            return Task.FromResult(new string(new char[100000000]));
        }

        public Task<ITestHandle> GetHandle(CancellationToken cancellationToken_)
        {
            return Task.FromResult((ITestHandle)new TestHandleImpl(_counters));
        }

        public Task<ITestInterface> GetHeld(CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);
            return Task.FromResult(Proxy.Share(ClientToHold));
        }

        public Task<ITestMoreStuff> GetNull(CancellationToken cancellationToken_)
        {
            return Task.FromResult(default(ITestMoreStuff));
        }

        public Task Hold(ITestInterface cap, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);

            ClientToHold?.Dispose();
            ClientToHold = cap;

            return Task.CompletedTask;
        }

        public Task<(string, string)> MethodWithDefaults(string a, uint b, string c, CancellationToken cancellationToken_)
        {
            throw new NotImplementedException();
        }

        public Task MethodWithNullDefault(string a, ITestInterface b, CancellationToken cancellationToken_)
        {
            throw new NotImplementedException();
        }

        public async Task<ITestInterface> NeverReturn(ITestInterface cap, CancellationToken cancellationToken_)
        {
            Interlocked.Increment(ref _counters.CallCount);

            try
            {
                var tcs = new TaskCompletionSource<int>();
                using (cancellationToken_.Register(() => tcs.SetResult(0)))
                {
                    await tcs.Task;
                    throw new TaskCanceledException();
                }
            }
            finally
            {
                cap.Dispose();
            }
        }
    }

    #endregion TestMoreStuff

    #region TestHandle
    class TestHandleImpl : ITestHandle, IDisposable
    {
        readonly Counters _counters;

        public TestHandleImpl(Counters counters)
        {
            _counters = counters;
            Interlocked.Increment(ref _counters.HandleCount);
        }

        public void Dispose()
        {
            Interlocked.Decrement(ref _counters.HandleCount);
        }
    }
    #endregion TestHandle

    #region B2

    class B2Impl : CapnpGen.IB2
    {
        string _s;

        public void Dispose()
        {
        }

        public Task MethodA(string param1, CancellationToken cancellationToken_ = default)
        {
            _s = param1;
            return Task.CompletedTask;
        }

        public Task<string> MethodB(long param1, CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult(_s);
        }
    }
    #endregion B2

    #region Issue25

    class Issue25AImpl : CapnpGen.IIssue25A
    {
        public void Dispose()
        {
        }

        public Task<long> MethodA(CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult(123L);
        }
    }

    class CapHolderImpl<T> : CapnpGen.ICapHolder<T>
        where T: class
    {
        T _cap;

        public CapHolderImpl(T cap)
        {
            _cap = cap;
        }

        public Task<T> Cap(CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult(_cap);
        }

        public void Dispose()
        {
        }
    }

    class CapHolderAImpl : CapnpGen.ICapHolderA
    {
        IIssue25A _a;

        public CapHolderAImpl(IIssue25A a)
        {
            _a = a;
        }

        public Task<IIssue25A> Cap(CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult(_a);
        }

        public void Dispose()
        {
        }
    }

    class Issue25BImpl : CapnpGen.IIssue25B
    {
        Issue25AImpl _a = new Issue25AImpl();

        public void Dispose()
        {
        }

        public Task<ICapHolder<object>> GetAinCapHolderAnyPointer(CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult<ICapHolder<object>>(new CapHolderImpl<object>(_a));
        }

        public Task<ICapHolder<IIssue25A>> GetAinCapHolderGenericA(CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult<ICapHolder<IIssue25A>>(new CapHolderImpl<CapnpGen.IIssue25A>(_a));
        }

        public Task<ICapHolderA> GetAinCapHolderNonGenericA(CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult<ICapHolderA>(new CapHolderAImpl(_a));
        }
    }

    #endregion
}
