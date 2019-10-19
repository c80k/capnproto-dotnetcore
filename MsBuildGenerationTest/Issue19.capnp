@0xe169e9301753ca94;

interface GenericA(T) {
	methodA @0 (param1 :T) -> ();
}

interface B2 extends (GenericA(Text)) {
	methodB @0 (param1 :Int64) -> (res :Text);
}
