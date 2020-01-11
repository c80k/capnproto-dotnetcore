using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// Provides functionality to construct domain objects from <see cref="DeserializerState"/>.
    /// </summary>
    public static class CapnpSerializable
    {
        interface IConstructibleFromDeserializerState
        {
            object? Create(DeserializerState state);
        }

        class FromStruct<T>: IConstructibleFromDeserializerState
            where T : ICapnpSerializable, new()
        {
            public object Create(DeserializerState state)
            {
                var result = new T();
                if (state.Kind != ObjectKind.Nil)
                {
                    result.Deserialize(state);
                }
                return result;
            }
        }

        class FromList<T>: IConstructibleFromDeserializerState
            where T: class
        {
            readonly Func<DeserializerState, T> _elementSerializer;

            public FromList()
            {
                _elementSerializer = (Func<DeserializerState, T>)GetSerializer(typeof(T));

            }
            public object Create(DeserializerState state)
            {
                return state.RequireList().Cast(_elementSerializer);
            }
        }

        class FromCapability<T>: IConstructibleFromDeserializerState
            where T: class
        {
            public object? Create(DeserializerState state)
            {
                return state.RequireCap<T>();
            }
        }

        static readonly ConditionalWeakTable<Type, Func<DeserializerState, object?>> _typeMap =
            new ConditionalWeakTable<Type, Func<DeserializerState, object?>>();

        static CapnpSerializable()
        {
            _typeMap.Add(typeof(string), d => d.RequireList().CastText());
            _typeMap.Add(typeof(IReadOnlyList<bool>), d => d.RequireList().CastBool());
            _typeMap.Add(typeof(IReadOnlyList<sbyte>), d => d.RequireList().CastSByte());
            _typeMap.Add(typeof(IReadOnlyList<byte>), d => d.RequireList().CastByte());
            _typeMap.Add(typeof(IReadOnlyList<short>), d => d.RequireList().CastShort());
            _typeMap.Add(typeof(IReadOnlyList<ushort>), d => d.RequireList().CastUShort());
            _typeMap.Add(typeof(IReadOnlyList<int>), d => d.RequireList().CastInt());
            _typeMap.Add(typeof(IReadOnlyList<uint>), d => d.RequireList().CastUInt());
            _typeMap.Add(typeof(IReadOnlyList<long>), d => d.RequireList().CastLong());
            _typeMap.Add(typeof(IReadOnlyList<ulong>), d => d.RequireList().CastULong());
            _typeMap.Add(typeof(IReadOnlyList<float>), d => d.RequireList().CastFloat());
            _typeMap.Add(typeof(IReadOnlyList<double>), d => d.RequireList().CastDouble());
        }

        static Func<DeserializerState, object?> CreateSerializer(Type type)
        {
            if (typeof(ICapnpSerializable).IsAssignableFrom(type))
            {
                try
                {
                    return ((IConstructibleFromDeserializerState)
                        Activator.CreateInstance(typeof(FromStruct<>).MakeGenericType(type))!).Create;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"Cannot create serializer, probably because serializer {type.Name} does not expose a public parameterless constructor",
                        ex);
                }
            }
            else if (type.IsGenericType && typeof(IReadOnlyList<>) == type.GetGenericTypeDefinition())
            {
                try
                {
                    var elementType = type.GetGenericArguments()[0];
                    return ((IConstructibleFromDeserializerState)
                        Activator.CreateInstance(typeof(FromList<>).MakeGenericType(elementType))!).Create;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"Cannot create list serializer, probably because the element type is not a reference type",
                        ex);
                }
            }
            else
            {
                try
                {
                    Rpc.CapabilityReflection.ValidateCapabilityInterface(type);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"Don't know how to construct a serializer from {type.Name}. Tried to interpret it as capability interface, but it didn't work.",
                        ex);
                }

                try
                {
                    return ((IConstructibleFromDeserializerState)
                        Activator.CreateInstance(typeof(FromCapability<>).MakeGenericType(type))!).Create;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"Cannot create serializer, probably because serializer {type.Name} a not a viable capability interface",
                        ex);
                }
            }
        }

        static Func<DeserializerState, object?> GetSerializer(Type type)
        {
            return _typeMap.GetValue(type, CreateSerializer);
        }

        /// <summary>
        /// Constructs a domain object from a given deserializer state.
        /// </summary>
        /// <typeparam name="T">Type of domain object to construct. Must be one of the following:
        /// <list type="bullet">
        /// <item><description>Type implementing <see cref="ICapnpSerializable"/>. The type must must have a public parameterless constructor.</description></item>
        /// <item><description>A capability interface (<seealso cref="Rpc.InvalidCapabilityInterfaceException"/> for further explanation)</description></item>
        /// <item><description><see cref="String"/></description></item>
        /// <item><description><see cref="IReadOnlyList{Boolean}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{SByte}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{Byte}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{Int16}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{UInt16}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{Int32}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{UInt32}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{Int64}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{UInt64}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{Single}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{Double}"/></description></item>
        /// <item><description><see cref="IReadOnlyList{T}"/> whereby T is one of the things listed here.</description></item>
        /// </list>
        /// </typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public static T? Create<T>(DeserializerState state)
            where T: class
        {
            return (T?)GetSerializer(typeof(T))(state);
        }
    }
}
#nullable restore