using System;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// This SerializerState specialization provides functionality to build arbitrary Cap'n Proto objects without requiring the schema code generator.
    /// </summary>
    public class DynamicSerializerState : SerializerState
    {
        /// <summary>
        /// Constructs an unbound instance.
        /// </summary>
        public DynamicSerializerState()
        {
        }

        /// <summary>
        /// Constructs an instance and binds it to the given <see cref="MessageBuilder"/>.
        /// </summary>
        /// <param name="messageBuilder">message builder</param>
        public DynamicSerializerState(MessageBuilder messageBuilder):
            base(messageBuilder)
        {
        }

        /// <summary>
        /// Constructs an instance, binds it to a dedicated message builder, and initializes the capability table for usage in RPC context.
        /// </summary>
        public static DynamicSerializerState CreateForRpc()
        {
            var mb = MessageBuilder.Create();
            mb.InitCapTable();
            return new DynamicSerializerState(mb);
        }

        /// <summary>
        /// Converts any <see cref="DeserializerState"/> to a DynamicSerializerState instance, which involves deep copying the object graph.
        /// </summary>
        /// <param name="state">The deserializer state to convert</param>
        public static explicit operator DynamicSerializerState(DeserializerState state)
        {
            var mb = MessageBuilder.Create();
            if (state.Caps != null)
                mb.InitCapTable();
            var sstate = mb.CreateObject<DynamicSerializerState>();
            Reserializing.DeepCopy(state, sstate);

            return sstate;
        }

        /// <summary>
        /// Links a sub-item (struct field or list element) of this state to another state. Usually, this operation is not necessary, since objects are constructed top-down.
        /// However, there might be some advanced scenarios where you want to reference the same object twice (also interesting for designing amplification attacks).
        /// The Cap'n Proto serialization intrinsically supports this, since messages are object graphs, not trees.
        /// </summary>
        /// <param name="slot">If this state describes a struct: Index into this struct's pointer table. 
        /// If this state describes a list of pointers: List element index.</param>
        /// <param name="target">state to be linked</param>
        /// <param name="allowCopy">Whether to deep copy the target state if it belongs to a different message builder than this state.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> out of range</exception>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>This state does neither describe a struct, nor a list of pointers</description></item>
        /// <item><description>Another state is already linked to the specified position (sorry, no overwrite allowed)</description></item>
        /// <item><description>This state and <paramref name="target"/> belong to different message builder, and<paramref name="allowCopy"/> is false</description></item></list>
        /// </exception>
        public new void Link(int slot, SerializerState target, bool allowCopy = true) => base.Link(slot, target, allowCopy);

        /// <summary>
        /// Links a sub-item (struct field or list element) of this state to a capability.
        /// </summary>
        /// <param name="slot">If this state describes a struct: Index into this struct's pointer table. 
        /// If this state describes a list of pointers: List element index.</param>
        /// <param name="capabilityIndex">capability index inside the capability table</param>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>This state does neither describe a struct, nor a list of pointers</description></item>
        /// <item><description>Another state is already linked to the specified position (sorry, no overwrite allowed)</description></item></list>
        /// </exception>
        public new void LinkToCapability(int slot, uint capabilityIndex) => base.LinkToCapability(slot, capabilityIndex);

        /// <summary>
        /// Determines the underlying object to be a struct.
        /// </summary>
        /// <param name="dataCount">Desired size of the struct's data section, in words</param>
        /// <param name="ptrCount">Desired size of the struct's pointer section, in words</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        public new void SetStruct(ushort dataCount, ushort ptrCount) => base.SetStruct(dataCount, ptrCount);

        /// <summary>
        /// Determines the underlying object to be a list of (primitive) values.
        /// </summary>
        /// <param name="bitsPerElement">Element size in bits, must be 0 (void), 1 (bool), 8, 16, 32, or 64</param>
        /// <param name="totalCount">Desired element count</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitsPerElement"/> outside allowed range, 
        /// <paramref name="totalCount"/> negative or exceeding 2^29-1</exception>
        public new void SetListOfValues(byte bitsPerElement, int totalCount) => base.SetListOfValues(bitsPerElement, totalCount);

        /// <summary>
        /// Determines the underlying object to be a list of pointers.
        /// </summary>
        /// <param name="totalCount">Desired element count</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="totalCount"/> negative or exceeding 2^29-1</exception>
        public new void SetListOfPointers(int totalCount) => base.SetListOfPointers(totalCount);

        /// <summary>
        /// Determines the underlying object to be a list of structs (fixed-width compound list).
        /// </summary>
        /// <param name="totalCount">Desired element count</param>
        /// <param name="dataCount">Desired size of each struct's data section, in words</param>
        /// <param name="ptrCount">Desired size of each struct's pointer section, in words</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="totalCount"/> negative, or total word count would exceed 2^29-1</exception>
        public new void SetListOfStructs(int totalCount, ushort dataCount, ushort ptrCount) => base.SetListOfStructs(totalCount, dataCount, ptrCount);

        /// <summary>
        /// Constructs the underlying object from the given representation.
        /// </summary>
        /// <param name="obj">Object representation. Must be one of the following:
        /// <list type="bullet">
        /// <item><description>An instance implementing <see cref="ICapnpSerializable"/></description></item>
        /// <item><description>null</description></item>
        /// <item><description>A <see cref="String"/></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<byte>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<sbyte>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<ushort>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<short>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<int>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<uint>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<long>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<ulong>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<float>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<double>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<bool>]]></code></description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<string>]]></code></description></item>
        /// <item><description>Another <see cref="DeserializerState"/></description></item>
        /// <item><description>Another <see cref="SerializerState"/></description></item>
        /// <item><description>Low-level capability object (<see cref="Rpc.ConsumedCapability"/>)</description></item>
        /// <item><description>Proxy object (<see cref="Rpc.Proxy"/>)</description></item>
        /// <item><description>Skeleton object (<see cref="Rpc.Skeleton"/>)</description></item>
        /// <item><description>Capability interface implementation</description></item>
        /// <item><description>A <code><![CDATA[IReadOnlyList<object>]]></code> whereby each list item is one of the things listed here.</description></item>
        /// </list>
        /// </param>
        public void SetObject(object obj)
        {
            switch (obj)
            {
                case ICapnpSerializable serializable:
                    serializable.Serialize(this);
                    break;

                case string s:
                    WriteText(s);
                    break;

                case IReadOnlyList<byte> bytes:
                    Rewrap<ListOfPrimitivesSerializer<byte>>().Init(bytes);
                    break;

                case IReadOnlyList<sbyte> sbytes:
                    Rewrap<ListOfPrimitivesSerializer<sbyte>>().Init(sbytes);
                    break;

                case IReadOnlyList<ushort> ushorts:
                    Rewrap<ListOfPrimitivesSerializer<ushort>>().Init(ushorts);
                    break;

                case IReadOnlyList<short> shorts:
                    Rewrap<ListOfPrimitivesSerializer<short>>().Init(shorts);
                    break;

                case IReadOnlyList<uint> uints:
                    Rewrap<ListOfPrimitivesSerializer<uint>>().Init(uints);
                    break;

                case IReadOnlyList<int> ints:
                    Rewrap<ListOfPrimitivesSerializer<int>>().Init(ints);
                    break;

                case IReadOnlyList<ulong> ulongs:
                    Rewrap<ListOfPrimitivesSerializer<ulong>>().Init(ulongs);
                    break;

                case IReadOnlyList<long> longs:
                    Rewrap<ListOfPrimitivesSerializer<long>>().Init(longs);
                    break;

                case IReadOnlyList<float> floats:
                    Rewrap<ListOfPrimitivesSerializer<float>>().Init(floats);
                    break;

                case IReadOnlyList<double> doubles:
                    Rewrap<ListOfPrimitivesSerializer<double>>().Init(doubles);
                    break;

                case IReadOnlyList<bool> bools:
                    Rewrap<ListOfBitsSerializer>().Init(bools);
                    break;

                case IReadOnlyList<string> strings:
                    Rewrap<ListOfTextSerializer>().Init(strings);
                    break;

                case IReadOnlyList<object> objects:
                    Rewrap<ListOfPointersSerializer<DynamicSerializerState>>().Init(objects, (s, o) => s.SetObject(o));
                    break;

                case DeserializerState ds:
                    Reserializing.DeepCopy(ds, this);
                    break;

                case SerializerState s:
                    Reserializing.DeepCopy(s, this);
                    break;

                case null:
                    break;

                default:
                    SetCapability(ProvideCapability(obj));
                    break;
            }
        }
    }
}