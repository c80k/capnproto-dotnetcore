@0xbbfd48ae4b99d012;

using Cxx = import "/capnp/c++.capnp";

$Cxx.namespace("Foo::Bar::Baz");

struct Outer {
	inner @0: import "UnitTest10b.capnp".Inner ;
}
