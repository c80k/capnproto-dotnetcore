@0xeb0d831668c6edab;
$namespace("Capnp.Annotations");

enum TypeVisibility @0xeb0d831668c6eda5 {
  public @0;
  internal @1;
}

annotation namespace @0xeb0d831668c6eda0 (file) : Text;
# C# namespace for code generation

annotation nullableEnable @0xeb0d831668c6eda1 (file) : Bool;
# Whether to generate C# nullable reference types

annotation emitNullableDirective @0xeb0d831668c6eda3 (file) : Bool;
# Whether to surround the generated code with #nullable enable/disable ... #nullable restore

annotation emitDomainClassesAndInterfaces @0xeb0d831668c6eda4 (file) : Bool;
# Whether generate domain classes and interfaces (default is 'true' if annotation is missing)

annotation typeVisibility @0xeb0d831668c6eda6 (file) : TypeVisibility;
# Visibility of generated types

annotation name @0xeb0d831668c6eda2 (field, enumerant, struct, enum, interface, method, param, group, union) : Text;
# C# member name for code generation
