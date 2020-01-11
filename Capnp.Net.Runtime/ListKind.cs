namespace Capnp
{
    /// <summary>
    /// Enumerates the list element categories which are defined by Cap'n Proto.
    /// </summary>
    public enum ListKind: byte
    {
        /// <summary>
        /// List(Void)
        /// </summary>
        ListOfEmpty = 0,

        /// <summary>
        /// List(Bool)
        /// </summary>
        ListOfBits = 1,

        /// <summary>
        /// List(Int8) or List(UInt8)
        /// </summary>
        ListOfBytes = 2,

        /// <summary>
        /// List(Int16), List(UInt16), or List(Enum)
        /// </summary>
        ListOfShorts = 3,

        /// <summary>
        /// List(Int32), List(UInt32), or List(Float32)
        /// </summary>
        ListOfInts = 4,

        /// <summary>
        /// List(Int64), List(UInt64), or List(Float64)
        /// </summary>
        ListOfLongs = 5,

        /// <summary>
        /// A list of pointers
        /// </summary>
        ListOfPointers = 6,

        /// <summary>
        /// A list of fixed-size composites (i.e. structs)
        /// </summary>
        ListOfStructs = 7
    }
}