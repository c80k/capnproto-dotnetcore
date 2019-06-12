using System;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// Base class for interpreting a <see cref="DeserializerState"/> as List(T).
    /// </summary>
    public abstract class ListDeserializer
    {
        static class GenericCasts<T>
        {
            public static Func<ListDeserializer, T> CastFunc;
        }

        static ListDeserializer()
        {
            GenericCasts<IReadOnlyList<bool>>.CastFunc = _ => _.CastBool();
            GenericCasts<IReadOnlyList<sbyte>>.CastFunc = _ => _.CastSByte();
            GenericCasts<IReadOnlyList<byte>>.CastFunc = _ => _.CastByte();
            GenericCasts<IReadOnlyList<short>>.CastFunc = _ => _.CastShort();
            GenericCasts<IReadOnlyList<ushort>>.CastFunc = _ => _.CastUShort();
            GenericCasts<IReadOnlyList<int>>.CastFunc = _ => _.CastInt();
            GenericCasts<IReadOnlyList<uint>>.CastFunc = _ => _.CastUInt();
            GenericCasts<IReadOnlyList<long>>.CastFunc = _ => _.CastLong();
            GenericCasts<IReadOnlyList<ulong>>.CastFunc = _ => _.CastULong();
            GenericCasts<IReadOnlyList<float>>.CastFunc = _ => _.CastFloat();
            GenericCasts<IReadOnlyList<double>>.CastFunc = _ => _.CastDouble();
            GenericCasts<string>.CastFunc = _ => _.CastText();
        }

        /// <summary>
        /// Underlying deserializer state
        /// </summary>
        protected readonly DeserializerState State;

        internal ListDeserializer(ref DeserializerState state)
        {
            State = state;
        }

        internal ListDeserializer()
        {
        }

        T Cast<T>()
        {
            var func = GenericCasts<T>.CastFunc;

            if (func == null)
                throw new NotSupportedException("Requested cast is not supported");

            return func(this);
        }

        /// <summary>
        /// This list's element count
        /// </summary>
        public int Count => State.ListElementCount;

        /// <summary>
        /// The list's element category
        /// </summary>
        public abstract ListKind Kind { get; }

        /// <summary>
        /// Represents this list by applying a selector function to each element's deserializer state.
        /// This operator is only supported by certain specializations.
        /// </summary>
        /// <typeparam name="T">Target element type</typeparam>
        /// <param name="cons">Selector function</param>
        /// <returns>The desired representation</returns>
        public abstract IReadOnlyList<T> Cast<T>(Func<DeserializerState, T> cons);

        /// <summary>
        /// Represents this list as a list of lists.
        /// </summary>
        /// <returns>The list of lists representation, each element being a <see cref="ListDeserializer"/> on its own.</returns>
        /// <exception cref="NotSupportedException">If this kind of list cannot be represented as list of lists (because it is a list of non-pointers)</exception>
        public virtual IReadOnlyList<ListDeserializer> CastList()
        {
            throw new NotSupportedException("This kind of list does not contain nested lists");
        }

        /// <summary>
        /// Represents this list as a list of capabilities.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <returns>Capability list representation</returns>
        /// <exception cref="NotSupportedException">If this kind of list cannot be represented as list of capabilities (because it is a list of non-pointers)</exception>
        /// <exception cref="Rpc.InvalidCapabilityInterfaceException">If <typeparamref name="T"/> does not qualify as capability interface.</exception>
        public virtual IReadOnlyList<ListOfCapsDeserializer<T>> CastCapList<T>() where T: class
        {
            throw new NotSupportedException("This kind of list does not contain nested lists");
        }

        object CastND(int n, Func<ListDeserializer, object> func)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n));

            for (int i = 1; i < n; i++)
            {
                var copy = func; // This copy assignment is intentional. Try to optimize it away and be amazed!
                func = ld => ld.CastList().LazyListSelect(copy);
            }

            return func(this);
        }

        /// <summary>
        /// Represents this list as n-dimensional list of T, with T being a primitive type.
        /// </summary>
        /// <typeparam name="T">Element type, must be primitive</typeparam>
        /// <param name="n">Number of dimensions</param>
        /// <returns>The desired representation as <![CDATA[IReadOnlyList<...IReadOnlyList<T>>]]></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is less than or equal to 0</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public object CastND<T>(int n) => CastND(n, ld => ld.Cast<IReadOnlyList<T>>());

        /// <summary>
        /// Represents this list as n-dimensional list of T, with T being any type.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="n">Number of dimensions</param>
        /// <param name="cons">Selector function which constructs an instance of <typeparamref name="T"/> from a <see cref="DeserializerState"/></param>
        /// <returns>The desired representation as <![CDATA[IReadOnlyList<...IReadOnlyList<T>>]]></returns>
        /// <exception cref="ArgumentNullException"><paramref name="cons"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is less than or equals 0.</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public object CastND<T>(int n, Func<DeserializerState, T> cons) => CastND(n, ld => ld.Cast(cons));

        /// <summary>
        /// Represents this list as n-dimensional list of enums.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="n">Number of dimensions</param>
        /// <param name="cons">Cast function which converts ushort value to enum value</param>
        /// <returns>The desired representation as <![CDATA[IReadOnlyList<...IReadOnlyList<T>>]]></returns>
        /// <exception cref="ArgumentNullException"><paramref name="cons"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is less than or equals 0.</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public object CastEnumsND<T>(int n, Func<ushort, T> cons) => CastND(n, ld => ld.CastEnums(cons));

        /// <summary>
        /// Represents this list as n-dimensional List(...List(Void))
        /// </summary>
        /// <param name="n">Number of dimensions</param>
        /// <returns>The desired representation as <![CDATA[IReadOnlyList<...IReadOnlyList<T>>]]></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is less than or equals 0</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public object CastVoidND(int n) => CastND(n, (ListDeserializer ld) => ld.Count);

        /// <summary>
        /// Represents this list as "matrix" (jagged array) with primitive element type.
        /// </summary>
        /// <typeparam name="T">Element type, must be primitive</typeparam>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<T>> Cast2D<T>()
        {
            return CastList().LazyListSelect(ld => ld.Cast<IReadOnlyList<T>>());
        }

        /// <summary>
        /// Represents this list as List(Data).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<byte>> CastData() => Cast2D<byte>();

        /// <summary>
        /// Represents this list as "matrix" (jagged array) with complex element type.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="cons">Selector function which constructs an instance of <typeparamref name="T"/> from a <see cref="DeserializerState"/></param>
        /// <returns>The desired representation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cons"/> is null.</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<T>> Cast2D<T>(Func<DeserializerState, T> cons)
        {
            return CastList().LazyListSelect(ld => ld.Cast(cons));
        }

        /// <summary>
        /// Represents this list as "matrix" (jagged array) of enum-typed elements.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="cons">Cast function which converts ushort value to enum value</param>
        /// <returns>The desired representation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cons"/> is null.</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<T>> CastEnums2D<T>(Func<ushort, T> cons)
        {
            return CastList().LazyListSelect(ld => ld.CastEnums(cons));
        }

        /// <summary>
        /// Represents this list as 3-dimensional jagged array with primitive element type.
        /// </summary>
        /// <typeparam name="T">Element type, must be primitive</typeparam>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<T>>> Cast3D<T>()
        {
            return CastList().LazyListSelect(ld => ld.Cast2D<T>());
        }

        /// <summary>
        /// Represents this list as 3-dimensional jagged array with complex element type.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="cons">Selector function which constructs an instance of <typeparamref name="T"/> from a <see cref="DeserializerState"/></param>
        /// <returns>The desired representation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cons"/> is null.</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<T>>> Cast3D<T>(Func<DeserializerState, T> cons)
        {
            return CastList().LazyListSelect(ld => ld.Cast2D(cons));
        }

        /// <summary>
        /// Represents this list as 3-dimensional jagged array of enum-typed elements.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="cons">Cast function which converts ushort value to enum value</param>
        /// <returns>The desired representation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cons"/> is null.</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<T>>> CastEnums3D<T>(Func<ushort, T> cons)
        {
            return CastList().LazyListSelect(ld => ld.CastEnums2D(cons));
        }

        /// <summary>
        /// Represents this list as list of enum-typed elements.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="cons">Cast function which converts ushort value to enum value</param>
        /// <returns>The desired representation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cons"/> is null.</exception>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<T> CastEnums<T>(Func<ushort, T> cons)
        {
            return CastUShort().LazyListSelect(cons);
        }

        /// <summary>
        /// Represents this list as List(Void), which boils down to returning the number of elements.
        /// </summary>
        /// <returns>The List(Void) representation which is nothing but the list's element count.</returns>
        public int CastVoid() => Count;

        /// <summary>
        /// Represents this list as List(List(Void)), which boils down to returning a list of element counts.
        /// </summary>
        /// <returns>A list of integers whereby each number equals the sublist's element count.</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<int> CastVoid2D() => CastList().LazyListSelect(ld => ld.Count);

        /// <summary>
        /// Represents this list as List(List(List(Void))).
        /// </summary>
        /// <returns>The List(List(List(Void))) representation which is in turn a 2-dimensional jagged array of element counts.</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<IReadOnlyList<int>> CastVoid3D() => CastList().LazyListSelect(ld => ld.CastVoid2D());

        /// <summary>
        /// Represents this list as List(Text). For representing it as Text, use <seealso cref="CastText"/>.
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public IReadOnlyList<string> CastText2() => CastList().LazyListSelect(ld => ld.CastText());

        /// <summary>
        /// Represents this list as Text. For representing it as List(Text), use <seealso cref="CastText2"/>.
        /// </summary>
        /// <remarks>
        /// Did you notice that the naming pattern is broken here? For every other CastX method, X depicts the element type. 
        /// CastX actually means "represent this list as list of X". Logically, the semantics of CastText should be the semantics
        /// implemented by CastText2. And this method's name should be "CastChar". This wouldn't be accurate either, since a string
        /// is semantically more than the list of its characters. Trying to figure out a consistent naming pattern, we'd probably
        /// end up in less concise method names (do you have a good suggestion?). Considering this and the fact that you probably
        /// won't use these methods directly (because the code generator will produce nice wrappers for you) it seems acceptable to
        /// live with the asymmetric and somewhat ugly naming.
        /// </remarks>
        /// <returns>The decoded text</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual string CastText()
        {
            throw new NotSupportedException("This kind of list does not represent text");
        }

        /// <summary>
        /// Represents this list as List(Bool).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<bool> CastBool()
        {
            return Cast(sd => sd.ReadDataBool(0));
        }

        /// <summary>
        /// Represents this list as List(Int8).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<sbyte> CastSByte()
        {
            return Cast(sd => sd.ReadDataSByte(0));
        }

        /// <summary>
        /// Represents this list as List(UInt8).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<byte> CastByte()
        {
            return Cast(sd => sd.ReadDataByte(0));
        }

        /// <summary>
        /// Represents this list as List(Int16).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<short> CastShort()
        {
            return Cast(sd => sd.ReadDataShort(0));
        }

        /// <summary>
        /// Represents this list as List(UInt16).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<ushort> CastUShort()
        {
            return Cast(sd => sd.ReadDataUShort(0));
        }

        /// <summary>
        /// Represents this list as List(Int32).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<int> CastInt()
        {
            return Cast(sd => sd.ReadDataInt(0));
        }

        /// <summary>
        /// Represents this list as List(UInt32).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<uint> CastUInt()
        {
            return Cast(sd => sd.ReadDataUInt(0));
        }

        public virtual IReadOnlyList<long> CastLong()
        /// <summary>
        /// Represents this list as List(Int64).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        {
            return Cast(sd => sd.ReadDataLong(0));
        }

        /// <summary>
        /// Represents this list as List(UInt64).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<ulong> CastULong()
        {
            return Cast(sd => sd.ReadDataULong(0));
        }

        /// <summary>
        /// Represents this list as List(Float32).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<float> CastFloat()
        {
            return Cast(sd => sd.ReadDataFloat(0));
        }

        /// <summary>
        /// Represents this list as List(Float64).
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">If this list cannot be represented in the desired manner.</exception>
        public virtual IReadOnlyList<double> CastDouble()
        {
            return Cast(sd => sd.ReadDataDouble(0));
        }
    }
}
