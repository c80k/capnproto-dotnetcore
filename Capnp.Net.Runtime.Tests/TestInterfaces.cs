using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests.ManualImpls
{
   // [Skeleton(typeof(TestInterfaceSkeleton))]
   // [Proxy(typeof(TestInterfaceProxy))]
   // interface ITestInterface: IDisposable
   // {
   //     Task<string> Foo(uint i, bool j);
   //     Task Bar();
   //     Task<int> Baz(int s);
   // }

   // [Skeleton(typeof(TestExtendsSkeleton))]
   // [Proxy(typeof(TestExtendsProxy))]
   // interface ITestExtends: ITestInterface, IDisposable
   // {
   //     void Qux();
   //     Task Corge(int x);
   //     Task<int> Grault();
   // }

   // interface ITestExtends2: ITestExtends, IDisposable
   // {
   // }

   // struct Box
   // {
   //     public ITestExtends Cap { get; set; }
   // }

   // struct AnyBox
   // {
   //     public object Cap { get; set; }
   // }

   // [Skeleton(typeof(TestPipelineSkeleton))]
   // [Proxy(typeof(TestPipelineProxy))]
   // interface ITestPipeline: IDisposable
   // {
   //     Task<(string, Box)> GetCap(uint n, ITestInterface inCap);
   //     Task TestPointers(ITestExtends cap, DeserializerState obj, IReadOnlyList<ITestExtends> list);
   //     Task<(string, AnyBox)> GetAnyCap(uint n, object inCap);
   // }

   // [Skeleton(typeof(TestCallOrderSkeleton))]
   // [Proxy(typeof(TestCallOrderProxy))]
   // interface ITestCallOrder : IDisposable
   // {
   //     Task<uint> GetCallSequence(uint expected);
   // }

   // struct TailResult
   // {
   //     public uint I { get; set; }
   //     public string T { get; set; }
   //     public ITestCallOrder C { get; set; }
   //}

   // [Skeleton(typeof(TestTailCalleeSkeleton))]
   // [Proxy(typeof(TestTailCalleeProxy))]
   // interface ITestTailCallee: IDisposable
   // {
   //     Task<TailResult> Foo(int i, string t);
   // }

   // [Skeleton(typeof(TestTailCallerSkeleton))]
   // [Proxy(typeof(TestTailCallerProxy))]
   // interface ITestTailCaller: IDisposable
   // {
   //     Task<TailResult> Foo(int i, ITestTailCallee c);
   // }

   // [Skeleton(typeof(TestHandleSkeleton))]
   // [Proxy(typeof(TestHandleProxy))]
   // interface ITestHandle: IDisposable { }


   // [Skeleton(typeof(TestMoreStuffSkeleton))]
   // [Proxy(typeof(TestMoreStuffProxy))]
   // interface ITestMoreStuff: ITestCallOrder
   // {
   //     Task<string> CallFoo(ITestInterface cap);
   //     Task<string> CallFooWhenResolved(ITestInterface cap);
   //     Task<ITestInterface> NeverReturn(ITestInterface cap, CancellationToken ct);
   //     Task Hold(ITestInterface cap);
   //     Task<string> CallHeld();
   //     Task<ITestInterface> GetHeld();
   //     Task<ITestCallOrder> Echo(ITestCallOrder cap);
   //     Task ExpectCancel(ITestInterface cap, CancellationToken ct);
   //     Task<(string, string)> MethodWithDefaults(string a, uint b, string c);
   //     void MethodWithNullDefault(string a, ITestInterface b);
   //     Task<ITestHandle> GetHandle();
   //     Task<ITestMoreStuff> GetNull();
   //     Task<string> GetEnormousString();
   // }
}

