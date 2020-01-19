@0xeb0d831668c6edab;
$csNamespace("Capnp.Annotations");

annotation csNamespace @0xeb0d831668c6eda0 (file) : Text;
# C# namespace for code generation

annotation csNullableEnable @0xeb0d831668c6eda1 (file) : Bool;
# Whether to generate C# nullable reference types

annotation csName @0xeb0d831668c6eda2 (field, enumerant, struct, enum, interface, method, param, group, union) : Text;
# C# member name for code generation
