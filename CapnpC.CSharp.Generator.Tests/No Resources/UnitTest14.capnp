@0xbbfd48ae4b99d014;

using Cxx = import "/capnp/c++.capnp";
using CSharp = import "/csharp.capnp";

$CSharp.namespace("Foo.Bar.Baz");
$Cxx.namespace("X::Y::Z");

struct SomeStruct {
}
