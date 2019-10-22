@0xae5ab8efc527d253;

struct Outer {

	interface A {
		methodA @0 (param1 :Int64) -> ();
	}
	   
	interface B {
		methodB @0 (param1 :Int64) -> (a :A);
	}

}
