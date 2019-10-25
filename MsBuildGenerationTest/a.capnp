@0x8e165bd19455e1ad;

using Common = import "common.capnp".Common;
struct OuterA {
	interface B {
		methodB @0 () -> (a :Common.A);
	}
}
