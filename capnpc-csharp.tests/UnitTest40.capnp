@0x8dd4359557611ebd;

struct Test {
	outer @0: Outer(TestInner);
}

struct Outer(V) {
	inner @0: List(Inner(V));
}

struct Inner(V) {
	field @0: V;
}

struct TestInner {}