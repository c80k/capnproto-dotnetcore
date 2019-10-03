@0xe4369c441ab8df19;

using Imported = import "UnitTest11b.capnp";

struct OuterStruct {
	innerStruct @0: Imported.InnerStruct;
	innerInterface @1: Imported.InnerInterface;
}

interface OuterInterface {
	struct Wrapper {
		innerStruct @0: Imported.InnerStruct;
		innerInterface @1: Imported.InnerInterface;
	}
}
