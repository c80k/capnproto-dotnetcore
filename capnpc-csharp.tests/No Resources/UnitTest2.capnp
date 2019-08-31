@0xf6041efc5e8b1e59;

interface Interface1(V1) {}

interface Interface  {
  method @0 () -> (arg : AnyPointer);
  method1 @1 () -> (arg : Interface1(AnyPointer));
  method2 @2 () -> (arg : Interface2(AnyPointer));
}

interface Interface2(V2) {}
