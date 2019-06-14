# capnproto-dotnetcore
A Cap'n Proto implementation for .NET Core

["Cap'n Proto is an insanely fast data interchange format and capability-based RPC system."](https://capnproto.org/) Whilst the original implementation is written in C++ there are several ports to other languages. This is a C# implementation for .NET Core.

Disclaimer: Neither this project nor its author are affiliated with Cap'n Proto. This is just yet another independent implementation of the specification. The following sections assume that you are familiar with [Cap'n Proto](https://capnproto.org/) and probably its [GitHub project](https://github.com/capnproto/capnproto).

## Getting started

For building from scratch you will need Visual Studio >= 2019 (e.g. Community Edition) with suitable workloads for C# / .NET Core (currently .NET Core 2.1) development. For the test suite, you will also need the C++ native workload, [vcpkg](https://github.com/microsoft/vcpkg) and Cap'n Proto release 0.7.0:
```
vcpkg install capnproto
```

Solution/project structure is as follows:
- Capnp.Net.sln contains three projects:
  * Capnp.Net.Runtime is the runtime implementation, a .NET assembly.
  * capnpc-csharp is the compiler backend for C# language
  * Capnp.Net.Runtime.Tests is an MS Unit Testing assembly, containing - you guessed it - the test suite
- CapnpCompatTest.sln compiles to a native x86 executable which depends on the original Cap'n Proto C++ implementation. It is (partially) required by the test suite for interoperability testing.
