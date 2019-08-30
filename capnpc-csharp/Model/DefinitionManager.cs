using System;
using System.Collections.Generic;
using System.Linq;

namespace CapnpC.Model
{
    class DefinitionManager
    {
        readonly Dictionary<ulong, IDefinition> _id2def = new Dictionary<ulong, IDefinition>();

        public GenFile CreateFile(ulong id, bool isGenerated)
            => CreateId<GenFile>(id, () => new GenFile(id, isGenerated));
        public GenFile GetExistingFile(ulong id)
            => GetId<GenFile>(id, TypeTag.File);

        public TypeDefinition CreateTypeDef(ulong id, TypeTag tag, IHasNestedDefinitions decl)
            => CreateId<TypeDefinition>(id, () => new TypeDefinition(tag, id, decl));
        public TypeDefinition GetExistingTypeDef(ulong id, TypeTag tag)
        {
            var def = GetId<TypeDefinition>(id, tag);
            if (def.Tag == TypeTag.Unknown) def.Tag = tag;
            return def;
        }

        public Annotation CreateAnnotation(ulong id, IHasNestedDefinitions decl)
            => CreateId<Annotation>(id, () => new Annotation(id, decl));
        public Annotation GetExistingAnnotation(ulong id)
            => GetId<Annotation>(id, TypeTag.Annotation);

        public Constant CreateConstant(ulong id, IHasNestedDefinitions decl)
            => CreateId<Constant>(id, () => new Constant(id, decl));
        public Constant GetExistingConstant(ulong id)
            => GetId<Constant>(id, TypeTag.Const);

        public IDefinition GetExistingDef(ulong id, TypeTag tag)
            => GetId<IDefinition>(id, tag);

        public IEnumerable<GenFile> Files
        {
            get => _id2def.Values.Where(d => d.Tag == TypeTag.File).Select(f => f as GenFile);
        }

        T CreateId<T>(ulong id, Func<IDefinition> creator) where T : class, IDefinition
        {
            if (_id2def.TryGetValue(id, out var d))
            {
                throw new ArgumentException(nameof(id), $"Attempting to redefine {d.Tag.ToString()} {id.StrId()} (as {nameof(T)}).");
            }
            var def = creator();
            _id2def.Add(id, def);
            return def as T;
        }

        T GetId<T>(ulong id, TypeTag tag) where T : IDefinition
        {
            if (!_id2def.TryGetValue(id, out var anyDef))
            {
                throw new ArgumentOutOfRangeException($"Attempting to retrieve nonexistent node {id.StrId()}.");
            }
            if (!(anyDef is T def) || (tag != TypeTag.Unknown && def.Tag != tag))
            {
                throw new ArgumentOutOfRangeException($"Attempting to retrieve {tag.ToString()} {id.StrId()}, but found {anyDef.Tag.ToString()} instead.");
            }
            return def;
        }
    }
}
