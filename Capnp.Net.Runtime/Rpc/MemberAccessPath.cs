using System;
using System.Collections.Generic;
using System.Linq;

namespace Capnp.Rpc
{
    /// <summary>
    /// A path from an outer Cap'n Proto struct to an inner (probably deeply nested) struct member.
    /// </summary>
    public class MemberAccessPath
    {
        public static readonly MemberAccessPath BootstrapAccess = new MemberAccessPath(new List<MemberAccess>());

        public static MemberAccessPath Deserialize(PromisedAnswer.READER promisedAnswer)
        {
            var ops = new MemberAccess[promisedAnswer.Transform.Count];

            int i = 0;
            foreach (var op in promisedAnswer.Transform)
            {
                ops[i++] = MemberAccess.Deserialize(op);
            }

            return new MemberAccessPath(ops);
        }

        /// <summary>
        /// Constructs a path from <see cref="MemberAccess"/> qualifiers.
        /// </summary>
        /// <param name="path">List of member access elements</param>
        public MemberAccessPath(IReadOnlyList<MemberAccess> path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        /// <summary>
        /// Constructs a path from Cap'n Proto struct member offsets.
        /// </summary>
        /// <param name="offsets">Member offsets</param>
        public MemberAccessPath(params uint[] offsets)
        {
            if (offsets == null)
                throw new ArgumentNullException(nameof(offsets));

            Path = offsets.Select(i => new StructMemberAccess(checked((ushort)i))).ToArray();
        }

        /// <summary>
        /// Base class of an individual member access.
        /// </summary>
        /// <remarks>
        /// This might appear a bit of overengineering, since the only specialization is the <see cref="StructMemberAccess"/>.
        /// But there might be further specializations in the future, the most obvious one being an "ArrayElementAccess".
        /// Now we already have a suitable design pattern, mainly to show the abstract concept behind a member access path.
        /// </remarks>
        public abstract class MemberAccess
        {
            public static MemberAccess Deserialize(PromisedAnswer.Op.READER op)
            {
                switch (op.which)
                {
                    case PromisedAnswer.Op.WHICH.GetPointerField:
                        return new StructMemberAccess(op.GetPointerField);

                    default:
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Serializes this instance to a <see cref="PromisedAnswer.Op"/>.
            /// </summary>
            /// <param name="op">Serialization target</param>
            public abstract void Serialize(PromisedAnswer.Op.WRITER op);

            /// <summary>
            /// Evaluates the member access on a given struct instance.
            /// </summary>
            /// <param name="state">Input struct instance</param>
            /// <returns>Member value or object</returns>
            public abstract DeserializerState Eval(DeserializerState state);
        }

        /// <summary>
        /// The one and only member access which is currently supported: Member of a struct.
        /// </summary>
        public class StructMemberAccess: MemberAccess
        {
            /// <summary>
            /// Constructs an instance for given struct member offset.
            /// </summary>
            /// <param name="offset">The Cap'n Proto struct member offset</param>
            public StructMemberAccess(ushort offset)
            {
                Offset = offset;
            }

            /// <summary>
            /// The Cap'n Proto struct member offset
            /// </summary>
            public ushort Offset { get; }

            /// <summary>
            /// Serializes this instance to a <see cref="PromisedAnswer.Op"/>.
            /// </summary>
            /// <param name="op">Serialization target</param>
            public override void Serialize(PromisedAnswer.Op.WRITER op)
            {
                op.which = PromisedAnswer.Op.WHICH.GetPointerField;
                op.GetPointerField = Offset;
            }

            /// <summary>
            /// Evaluates the member access on a given struct instance.
            /// </summary>
            /// <param name="state">Input struct instance</param>
            /// <returns>Member value or object</returns>
            public override DeserializerState Eval(DeserializerState state)
            {
                if (state.Kind == ObjectKind.Nil)
                {
                    return default(DeserializerState);
                }

                if (state.Kind != ObjectKind.Struct)
                {
                    throw new ArgumentException("Expected a struct");
                }

                return state.StructReadPointer(Offset);
            }
        }

        /// <summary>
        /// The access path is a composition of individual member accesses.
        /// </summary>
        public IReadOnlyList<MemberAccess> Path { get; }

        /// <summary>
        /// Serializes this path th a <see cref="PromisedAnswer"/>.
        /// </summary>
        /// <param name="promisedAnswer">The serialization target</param>
        public void Serialize(PromisedAnswer.WRITER promisedAnswer)
        {
            promisedAnswer.Transform.Init(Path.Count);

            for (int i = 0; i < Path.Count; i++)
            {
                Path[i].Serialize(promisedAnswer.Transform[i]);
            }
        }

        /// <summary>
        /// Evaluates the path on a given object.
        /// </summary>
        /// <param name="rpcState">The object (usually "params struct") on which to evaluate this path.</param>
        /// <returns>Resulting low-level capability</returns>
        /// <exception cref="DeserializationException">Evaluation of this path did not give a capability</exception>
        public ConsumedCapability Eval(DeserializerState rpcState)
        {
            var cur = rpcState;

            foreach (var op in Path)
            {
                cur = op.Eval(cur);
            }

            switch (cur.Kind)
            {
                case ObjectKind.Nil:
                    return null;

                case ObjectKind.Capability:
                    return rpcState.Caps[(int)cur.CapabilityIndex];

                default:
                    throw new DeserializationException("Access path did not result in a capability");
            }
        }
    }
}
