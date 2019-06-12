namespace Capnp
{
    /// <summary>
    /// Generic <see cref="ICapnpSerializable"/> implementation, based on a wrapper around <see cref="DeserializerState"/>.
    /// </summary>
    public class AnyPointer : ICapnpSerializable
    {
        /// <summary>
        /// The <see cref="DeserializerState"/> will be set by the Deserialize method.
        /// </summary>
        public DeserializerState State { get; private set; }

        /// <summary>
        /// Sets the State property.
        /// </summary>
        /// <param name="state">deserializer state</param>
        public void Deserialize(DeserializerState state)
        {
            State = state;
        }

        /// <summary>
        /// Performs a deep copy from State to given state.
        /// </summary>
        /// <param name="state">serializer state</param>
        public void Serialize(SerializerState state)
        {
            Reserializing.DeepCopy(State, state);
        }
    }
}
