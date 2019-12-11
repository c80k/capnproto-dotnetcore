@0xb7069c462537ddd6;

enum TestEnum {
    a @0;
    b @1;
}

const globalConstant: TestEnum = b;

struct Struct {
    const structConstant: TestEnum = a;

    union {
        enumValue @0: TestEnum;
        intValue @1: Int64;
    }
}
