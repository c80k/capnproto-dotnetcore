@0xbbfd48ae4b99d013;

using CSharp = import "/csharp.capnp";

struct SomeStruct $CSharp.name("CsStruct") {
  someField @0 : Int32 $CSharp.name("CsField");

  someUnion : union $CSharp.name("CsUnion") {
    u0 @1 : Int32;
	u1 @2 : Int32;
  }

  someGroup : group $CSharp.name("CsGroup") {
    g0 @3 : Int32;
	g1 @4 : Int32;
  }
}

enum SomeEnum $CSharp.name("CsEnum") {
  someEnumerant @0 $CSharp.name("CsEnumerant");
}

interface SomeInterface $CSharp.name("CsInterface") {
  someMethod @0 () -> (someResult :Bool $CSharp.name("CsResult") ) $CSharp.name("CsMethod");
}
