@0xa53fcfef8b306bfb;

interface Issue25A {
  methodA @0 () -> (result :Int64);
}

interface CapHolder(CapType) {
  cap @0 () -> (cap :CapType);
}

interface CapHolderA {
  cap @0 () -> (cap :Issue25A);
}

interface Issue25B {
  getAinCapHolderAnyPointer @0 () -> (aInCapHolder :CapHolder);
  getAinCapHolderGenericA @1 () -> (aInCapHolder :CapHolder(Issue25A));
  getAinCapHolderNonGenericA @2 () -> (aInCapHolder :CapHolderA);
}
