@0xf463d204f5208b43;
$import "/capnp/c++.capnp".namespace("UnitTest4");

interface Node {
	getInfo @0 () -> Info;
}

struct Info {
	node @0 :Node;
	classes @1 :Classes;
}

struct Classes {
	i1 @0 :import "UnitTest4b.capnp".I1.Classes;
	i2 @1: Void;
}