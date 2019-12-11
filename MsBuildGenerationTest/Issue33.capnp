@0xaa62d76b329585e5;

interface Frobnicator(T)
{
  frobnicate @0 (value: T);
}

interface FrobnicatorFactory
{
  createFrobnicator @0 [T] (id: Text) -> (result: Frobnicator(T));
}
