# schema.capnp
@0xa93fc509624c72d9;
$import "/capnp/c++.capnp".namespace("capnp::schema");
struct Node @0xe682ab4cf923a417 {  # 40 bytes, 6 ptrs
  id @0 :UInt64;  # bits[0, 64)
  displayName @1 :Text;  # ptr[0]
  displayNamePrefixLength @2 :UInt32;  # bits[64, 96)
  scopeId @3 :UInt64;  # bits[128, 192)
  parameters @32 :List(Parameter);  # ptr[5]
  isGeneric @33 :Bool;  # bits[288, 289)
  nestedNodes @4 :List(NestedNode);  # ptr[1]
  annotations @5 :List(Annotation);  # ptr[2]
  union {  # tag bits [96, 112)
    file @6 :Void;  # bits[0, 0), union tag = 0
    struct :group {  # union tag = 1
      dataWordCount @7 :UInt16;  # bits[112, 128)
      pointerCount @8 :UInt16;  # bits[192, 208)
      preferredListEncoding @9 :ElementSize;  # bits[208, 224)
      isGroup @10 :Bool;  # bits[224, 225)
      discriminantCount @11 :UInt16;  # bits[240, 256)
      discriminantOffset @12 :UInt32;  # bits[256, 288)
      fields @13 :List(Field);  # ptr[3]
    }
    enum :group {  # union tag = 2
      enumerants @14 :List(Enumerant);  # ptr[3]
    }
    interface :group {  # union tag = 3
      methods @15 :List(Method);  # ptr[3]
      superclasses @31 :List(Superclass);  # ptr[4]
    }
    const :group {  # union tag = 4
      type @16 :Type;  # ptr[3]
      value @17 :Value;  # ptr[4]
    }
    annotation :group {  # union tag = 5
      type @18 :Type;  # ptr[3]
      targetsFile @19 :Bool;  # bits[112, 113)
      targetsConst @20 :Bool;  # bits[113, 114)
      targetsEnum @21 :Bool;  # bits[114, 115)
      targetsEnumerant @22 :Bool;  # bits[115, 116)
      targetsStruct @23 :Bool;  # bits[116, 117)
      targetsField @24 :Bool;  # bits[117, 118)
      targetsUnion @25 :Bool;  # bits[118, 119)
      targetsGroup @26 :Bool;  # bits[119, 120)
      targetsInterface @27 :Bool;  # bits[120, 121)
      targetsMethod @28 :Bool;  # bits[121, 122)
      targetsParam @29 :Bool;  # bits[122, 123)
      targetsAnnotation @30 :Bool;  # bits[123, 124)
    }
  }
  struct Parameter @0xb9521bccf10fa3b1 {  # 0 bytes, 1 ptrs
    name @0 :Text;  # ptr[0]
  }
  struct NestedNode @0xdebf55bbfa0fc242 {  # 8 bytes, 1 ptrs
    name @0 :Text;  # ptr[0]
    id @1 :UInt64;  # bits[0, 64)
  }
  struct SourceInfo @0xf38e1de3041357ae {  # 8 bytes, 2 ptrs
    id @0 :UInt64;  # bits[0, 64)
    docComment @1 :Text;  # ptr[0]
    members @2 :List(Member);  # ptr[1]
    struct Member @0xc2ba9038898e1fa2 {  # 0 bytes, 1 ptrs
      docComment @0 :Text;  # ptr[0]
    }
  }
}
struct Field @0x9aad50a41f4af45f {  # 24 bytes, 4 ptrs
  name @0 :Text;  # ptr[0]
  codeOrder @1 :UInt16;  # bits[0, 16)
  annotations @2 :List(Annotation);  # ptr[1]
  discriminantValue @3 :UInt16 = 65535;  # bits[16, 32)
  union {  # tag bits [64, 80)
    slot :group {  # union tag = 0
      offset @4 :UInt32;  # bits[32, 64)
      type @5 :Type;  # ptr[2]
      defaultValue @6 :Value;  # ptr[3]
      hadExplicitDefault @10 :Bool;  # bits[128, 129)
    }
    group :group {  # union tag = 1
      typeId @7 :UInt64;  # bits[128, 192)
    }
  }
  ordinal :group {
    union {  # tag bits [80, 96)
      implicit @8 :Void;  # bits[0, 0), union tag = 0
      explicit @9 :UInt16;  # bits[96, 112), union tag = 1
    }
  }
  const noDiscriminant @0x97b14cbe7cfec712 :UInt16 = 65535;
}
struct Enumerant @0x978a7cebdc549a4d {  # 8 bytes, 2 ptrs
  name @0 :Text;  # ptr[0]
  codeOrder @1 :UInt16;  # bits[0, 16)
  annotations @2 :List(Annotation);  # ptr[1]
}
struct Superclass @0xa9962a9ed0a4d7f8 {  # 8 bytes, 1 ptrs
  id @0 :UInt64;  # bits[0, 64)
  brand @1 :Brand;  # ptr[0]
}
struct Method @0x9500cce23b334d80 {  # 24 bytes, 5 ptrs
  name @0 :Text;  # ptr[0]
  codeOrder @1 :UInt16;  # bits[0, 16)
  implicitParameters @7 :List(Node.Parameter);  # ptr[4]
  paramStructType @2 :UInt64;  # bits[64, 128)
  paramBrand @5 :Brand;  # ptr[2]
  resultStructType @3 :UInt64;  # bits[128, 192)
  resultBrand @6 :Brand;  # ptr[3]
  annotations @4 :List(Annotation);  # ptr[1]
}
struct Type @0xd07378ede1f9cc60 {  # 24 bytes, 1 ptrs
  union {  # tag bits [0, 16)
    void @0 :Void;  # bits[0, 0), union tag = 0
    bool @1 :Void;  # bits[0, 0), union tag = 1
    int8 @2 :Void;  # bits[0, 0), union tag = 2
    int16 @3 :Void;  # bits[0, 0), union tag = 3
    int32 @4 :Void;  # bits[0, 0), union tag = 4
    int64 @5 :Void;  # bits[0, 0), union tag = 5
    uint8 @6 :Void;  # bits[0, 0), union tag = 6
    uint16 @7 :Void;  # bits[0, 0), union tag = 7
    uint32 @8 :Void;  # bits[0, 0), union tag = 8
    uint64 @9 :Void;  # bits[0, 0), union tag = 9
    float32 @10 :Void;  # bits[0, 0), union tag = 10
    float64 @11 :Void;  # bits[0, 0), union tag = 11
    text @12 :Void;  # bits[0, 0), union tag = 12
    data @13 :Void;  # bits[0, 0), union tag = 13
    list :group {  # union tag = 14
      elementType @14 :Type;  # ptr[0]
    }
    enum :group {  # union tag = 15
      typeId @15 :UInt64;  # bits[64, 128)
      brand @21 :Brand;  # ptr[0]
    }
    struct :group {  # union tag = 16
      typeId @16 :UInt64;  # bits[64, 128)
      brand @22 :Brand;  # ptr[0]
    }
    interface :group {  # union tag = 17
      typeId @17 :UInt64;  # bits[64, 128)
      brand @23 :Brand;  # ptr[0]
    }
    anyPointer :group {  # union tag = 18
      union {  # tag bits [64, 80)
        unconstrained :group {  # union tag = 0
          union {  # tag bits [80, 96)
            anyKind @18 :Void;  # bits[0, 0), union tag = 0
            struct @25 :Void;  # bits[0, 0), union tag = 1
            list @26 :Void;  # bits[0, 0), union tag = 2
            capability @27 :Void;  # bits[0, 0), union tag = 3
          }
        }
        parameter :group {  # union tag = 1
          scopeId @19 :UInt64;  # bits[128, 192)
          parameterIndex @20 :UInt16;  # bits[80, 96)
        }
        implicitMethodParameter :group {  # union tag = 2
          parameterIndex @24 :UInt16;  # bits[80, 96)
        }
      }
    }
  }
}
struct Brand @0x903455f06065422b {  # 0 bytes, 1 ptrs
  scopes @0 :List(Scope);  # ptr[0]
  struct Scope @0xabd73485a9636bc9 {  # 16 bytes, 1 ptrs
    scopeId @0 :UInt64;  # bits[0, 64)
    union {  # tag bits [64, 80)
      bind @1 :List(Binding);  # ptr[0], union tag = 0
      inherit @2 :Void;  # bits[0, 0), union tag = 1
    }
  }
  struct Binding @0xc863cd16969ee7fc {  # 8 bytes, 1 ptrs
    union {  # tag bits [0, 16)
      unbound @0 :Void;  # bits[0, 0), union tag = 0
      type @1 :Type;  # ptr[0], union tag = 1
    }
  }
}
struct Value @0xce23dcd2d7b00c9b {  # 16 bytes, 1 ptrs
  union {  # tag bits [0, 16)
    void @0 :Void;  # bits[0, 0), union tag = 0
    bool @1 :Bool;  # bits[16, 17), union tag = 1
    int8 @2 :Int8;  # bits[16, 24), union tag = 2
    int16 @3 :Int16;  # bits[16, 32), union tag = 3
    int32 @4 :Int32;  # bits[32, 64), union tag = 4
    int64 @5 :Int64;  # bits[64, 128), union tag = 5
    uint8 @6 :UInt8;  # bits[16, 24), union tag = 6
    uint16 @7 :UInt16;  # bits[16, 32), union tag = 7
    uint32 @8 :UInt32;  # bits[32, 64), union tag = 8
    uint64 @9 :UInt64;  # bits[64, 128), union tag = 9
    float32 @10 :Float32;  # bits[32, 64), union tag = 10
    float64 @11 :Float64;  # bits[64, 128), union tag = 11
    text @12 :Text;  # ptr[0], union tag = 12
    data @13 :Data;  # ptr[0], union tag = 13
    list @14 :AnyPointer;  # ptr[0], union tag = 14
    enum @15 :UInt16;  # bits[16, 32), union tag = 15
    struct @16 :AnyPointer;  # ptr[0], union tag = 16
    interface @17 :Void;  # bits[0, 0), union tag = 17
    anyPointer @18 :AnyPointer;  # ptr[0], union tag = 18
  }
}
struct Annotation @0xf1c8950dab257542 {  # 8 bytes, 2 ptrs
  id @0 :UInt64;  # bits[0, 64)
  brand @2 :Brand;  # ptr[1]
  value @1 :Value;  # ptr[0]
}
enum ElementSize @0xd1958f7dba521926 {
  empty @0;
  bit @1;
  byte @2;
  twoBytes @3;
  fourBytes @4;
  eightBytes @5;
  pointer @6;
  inlineComposite @7;
}
struct CapnpVersion @0xd85d305b7d839963 {  # 8 bytes, 0 ptrs
  major @0 :UInt16;  # bits[0, 16)
  minor @1 :UInt8;  # bits[16, 24)
  micro @2 :UInt8;  # bits[24, 32)
}
struct CodeGeneratorRequest @0xbfc546f6210ad7ce {  # 0 bytes, 4 ptrs
  capnpVersion @2 :CapnpVersion;  # ptr[2]
  nodes @0 :List(Node);  # ptr[0]
  sourceInfo @3 :List(Node.SourceInfo);  # ptr[3]
  requestedFiles @1 :List(RequestedFile);  # ptr[1]
  struct RequestedFile @0xcfea0eb02e810062 {  # 8 bytes, 2 ptrs
    id @0 :UInt64;  # bits[0, 64)
    filename @1 :Text;  # ptr[0]
    imports @2 :List(Import);  # ptr[1]
    struct Import @0xae504193122357e5 {  # 8 bytes, 1 ptrs
      id @0 :UInt64;  # bits[0, 64)
      name @1 :Text;  # ptr[0]
    }
  }
}
