#nullable enable
namespace Capnp
{
    /// <summary>
    /// This interface is intended to be implemented by schema-generated domain classes which support deserialization from
    /// a <see cref="DeserializerState"/> and serialization to a <see cref="SerializerState"/>.
    /// </summary>
    public interface ICapnpSerializable
    {
        /// <summary>
        /// Serializes the implementation's current state to a serializer state.
        /// </summary>
        /// <param name="state">Target serializer state</param>
        void Serialize(SerializerState state);

        /// <summary>
        /// Deserializes the implementation's state from a deserializer state.
        /// </summary>
        /// <param name="state">Source deserializer state</param>
        void Deserialize(DeserializerState state);
    }
}
#nullable restore