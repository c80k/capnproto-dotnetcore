using System;

namespace Capnp
{
    /// <summary>
    /// The different kinds of Cap'n Proto objects.
    /// Despite this is a [Flags] enum, it does not make sense to mutually combine literals.
    /// </summary>
    [Flags]
    public enum ObjectKind: byte
    {
        /// <summary>
        /// The null object, obtained by decoding a null pointer.
        /// </summary>
        Nil = 0,

        /// <summary>
        /// A struct
        /// </summary>
        Struct = 1,

        /// <summary>
        /// A capability
        /// </summary>
        Capability = 2,

        /// <summary>
        /// A List(void)
        /// </summary>
        ListOfEmpty = 8,

        /// <summary>
        /// A list of bits
        /// </summary>
        ListOfBits = 9,

        /// <summary>
        /// A list of octets
        /// </summary>
        ListOfBytes = 10,

        /// <summary>
        /// A list of 16 bit words
        /// </summary>
        ListOfShorts = 11,

        /// <summary>
        /// A list of 32 bit words
        /// </summary>
        ListOfInts = 12,

        /// <summary>
        /// A list of 64 bits words
        /// </summary>
        ListOfLongs = 13,

        /// <summary>
        /// A list of pointers
        /// </summary>
        ListOfPointers = 14,

        /// <summary>
        /// A list of fixed-width composites
        /// </summary>
        ListOfStructs = 15,

        /// <summary>
        /// A value. This kind of object does not exist on the wire and is not specified by Capnp.
        /// It is an internal helper to represent lists of primitive values as lists of structs.
        /// </summary>
        Value = 16
    }
}