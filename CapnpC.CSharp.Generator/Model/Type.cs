using Capnp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CapnpC.CSharp.Generator.Model
{
    class Type: AbstractType
    {
        // Representation of a type expression in the schema language

        public TypeDefinition Definition { get; set; }
        // The model for all nodes that are not file nodes - they define types

        public GenericParameter Parameter { get; set; }
        // A reference to type parameter in this scope

        public Type ElementType { get; set; }
        // The type of a list element, if this is a list.

        readonly Dictionary<GenericParameter, Type> _parameterBindings =
            new Dictionary<GenericParameter, Type>();
        public Type(TypeTag tag)
        {
            Tag = tag;
        }

        public bool IsValueType
        {
            get
            {
                switch (Tag)
                {
                    case TypeTag.AnyPointer:
                    case TypeTag.CapabilityPointer:
                    case TypeTag.Data:
                    case TypeTag.Group:
                    case TypeTag.Interface:
                    case TypeTag.List when ElementType.Tag != TypeTag.Void:
                    case TypeTag.ListPointer:
                    case TypeTag.Struct:
                    case TypeTag.StructPointer:
                    case TypeTag.Text:
                    case TypeTag.Void:
                        return false;

                    default:
                        return true;
                }
            }
        }

        public void InheritFreeParameters(IHasGenericParameters declaringType)
        {
            while (declaringType != null)
            {
                foreach (var p in declaringType.GetLocalTypeParameters())
                {
                    if (!_parameterBindings.ContainsKey(p))
                    {
                        _parameterBindings[p] = Types.FromParameter(p);
                    }
                }

                declaringType = (declaringType as TypeDefinition)?.DeclaringElement as IHasGenericParameters;
            }
        }

        Type SubstituteGenerics(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (type.Parameter != null)
            {
                if (_parameterBindings.TryGetValue(type.Parameter, out var boundType))
                {
                    return boundType;
                }
                else
                {
                    return type;
                }
            }

            var stype = new Type(type.Tag)
            {
                Definition = type.Definition,
                ElementType = SubstituteGenerics(type.ElementType)
            };

            foreach (var kvp in type._parameterBindings)
            {
                var p = kvp.Value.Parameter;

                if (p != null && _parameterBindings.TryGetValue(p, out var boundType))
                {
                    stype._parameterBindings[kvp.Key] = boundType;
                }
                else
                {
                    stype._parameterBindings[kvp.Key] = kvp.Value;
                }
            }

            return stype;
        }

        Field SubstituteGenerics(Field field)
        {
            var result = field.Clone();
            result.Type = SubstituteGenerics(result.Type);
            return result;
        }

        Method SubstituteGenerics(Method method)
        {
            var result = method.Clone();
            result.ParamsStruct = SubstituteGenerics(result.ParamsStruct);
            result.ResultStruct = SubstituteGenerics(result.ResultStruct);
            foreach (var field in result.Params)
            {
                field.Type = SubstituteGenerics(field.Type);
            }
            foreach (var field in result.Results)
            {
                field.Type = SubstituteGenerics(field.Type);
            }
            return result;
        }

        public IReadOnlyList<Field> Fields => Definition.Fields.LazyListSelect(SubstituteGenerics);

        public IReadOnlyList<Method> Methods => Definition.Methods.LazyListSelect(SubstituteGenerics);

        public Type DeclaringType
        {
            get
            {
                var parentDef = Definition?.DeclaringElement as TypeDefinition;
                // FIXME: Will become more sophisticated as soon as generics are implemented
                return parentDef != null ? Types.FromDefinition(parentDef) : null;
            }
        }

        public (int, Type) GetRank()
        {
            var cur = this;
            int rank = 0;

            while (cur.Tag == TypeTag.List)
            {
                cur = cur.ElementType;
                ++rank;
            }

            return (rank, cur);
        }

        public IEnumerable<Type> AllImplementedClasses
        {
            get
            {
                var stk = new Stack<Type>();
                stk.Push(this);
                var set = new HashSet<Type>();
                while (stk.Count > 0)
                {
                    var def = stk.Pop();

                    if (def == null)
                    {
                        break;
                    }

                    if (set.Add(def))
                    {
                        foreach (var super in def.Definition.Superclasses)
                        {
                            stk.Push(super);
                        }
                    }
                }
                return set;
            }
        }

        public Type ResolveGenericParameter(GenericParameter genericParameter)
        {
            if (_parameterBindings.TryGetValue(genericParameter, out var type))
            {
                return type;
            }
            else
            {
                return Types.AnyPointer;
            }
        }

        public void BindGenericParameter(GenericParameter genericParameter, Type boundType)
        {
            _parameterBindings.Add(genericParameter, boundType);
        }

        public override bool Equals(object obj)
        {
            return obj is Type other && Definition == other.Definition;
        }

        public override int GetHashCode()
        {
            return Definition?.GetHashCode() ?? 0;
        }
    }
}
