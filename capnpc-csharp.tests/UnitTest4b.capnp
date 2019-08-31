@0x8151238e9f9884c8;
$import "/capnp/c++.capnp".namespace("UnitTest4");

using Base = import "UnitTest4.capnp";

interface I1 {
	interface Node extends (Base.Node) {}
	struct Classes {
		sub @0: Sub;
	}
	struct Sub {
		const prototype :Base.Classes = ( i1 = (sub = ()) );
		data @0: Bool;
	}
}
