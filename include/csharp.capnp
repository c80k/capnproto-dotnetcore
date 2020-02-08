@0xeb0d831668c6edab;
$namespace("Capnp.Annotations");

annotation namespace @0xeb0d831668c6eda0 (file) : Text;
# C# namespace for code generation

annotation nullableEnable @0xeb0d831668c6eda1 (file) : Bool;
# Whether to generate C# nullable reference types

annotation emitNullableDirective @0xeb0d831668c6eda3 (file) : Bool;
# Whether to surround the generated with with #nullable enable/disable ... #nullable restore

annotation name @0xeb0d831668c6eda2 (field, enumerant, struct, enum, interface, method, param, group, union) : Text;
# C# member name for code generation
