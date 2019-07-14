# capnproto-dotnetcore
A Cap'n Proto implementation for .NET Standard 2.0 (credits to [lostinplace](https://github.com/lostinplace)) and .NET Core 2.1.

["Cap'n Proto is an insanely fast data interchange format and capability-based RPC system."](https://capnproto.org/) Whilst the original implementation is written in C++ there are several ports to other languages. This is a C# implementation for .NET Core.

Disclaimer: Neither this project nor its author are affiliated with Cap'n Proto. This is just yet another independent implementation of the specification. The following sections assume that you are familiar with [Cap'n Proto](https://capnproto.org/) and probably its [GitHub project](https://github.com/capnproto/capnproto).

## Getting started: Users

The overall deployment consists of two independent binaries:
- The C# code generator back end is required for generating `.cs` serialization classes from `.capnp` schema files. It is designed to be used in conjunction with the Cap'n Proto tool set which is maintained at the original site. The tool set is required at compile time.
- The `Capnp.Net.Runtime` assembly is to be included as a reference into your particular application (or assembly).

### Code generator back end: Windows

The C# code generator back end will be available as [Chocolatey](https://chocolatey.org/) package. You may choose between two flavors: The portable version requires a .NET Core 2.1 (or higher) runtime or SDK (type `dotnet` at command line prompt to check whether you already have one). This is the recommended variant. To install, type

```
choco install capnpc-csharp
```

The self-contained version does not require .NET Core but runs only on a x86-compatible Windows machine. To install, type

```
choco install capnpc-csharp-win-x86
```

Both versions will also download and install the [Cap'n Proto tool set Chocolatey package](https://www.chocolatey.org/packages/capnproto). Note that the author does not maintain this package and has no influence on its contents.

### Code generator back end: Other OSes

Currently, you are on yourself. Compile the `capnpc-csharp` VS project and install the resulting .NET application manually on your system. This should not be that complicated, see also the [Wiki](https://github.com/c80k/capnproto-dotnetcore/wiki). It would be great to support other package managers, especially [APT](https://wiki.debian.org/Apt). Consider contributing? Author would be happy!

### Runtime assembly

The `Capnp.Net.Runtime` assembly is available as [Nuget package](https://www.nuget.org/packages?q=Capnp.Net.Runtime). E.g. within VS package manage console, type

```
Install-Package Capnp.Net.Runtime
```

## Getting started: Developers

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

## Features

The following Cap'n Proto features are currently implemented:
- Serialization/deserialization of all kinds ofdata (structs, groups, unions, lists, capabilities, data, text, enums, even primitives)
- Generics
- Level 1 RPC, including promise pipelining, embargos, and automatic tail calls
- Security (pointer validation, protection against amplification and stack overflow DoS attacks)
- Compiler backend generates reader/writer classes, interfaces, proxies, skeletons (as you know it from the C++ implementation), and additionally so-called "domain classes" for all struct types. A domain class is like a "plain old C# class" for representing a schema-defined struct, but it is decoupled from any underlying message. It provides serialize/deserialize methods for assembling/disassembling the actual message. This provides more convenience, but comes at the price of non-zero serialization overhead (not "infinitely" faster anymore).

These features are not yet implemented:
- Level N RPC with N >= 2
- Packing
- Compression
- Canonicalization
- Dynamic Reflection
- mmap
