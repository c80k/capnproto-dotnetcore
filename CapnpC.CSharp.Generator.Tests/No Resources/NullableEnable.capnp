﻿@0xbbfd48ae4b99d016;

using CSharp = import "/csharp.capnp";

$CSharp.nullableEnable(true);

struct SomeStruct {
  strings @0 : List(Text);
}
