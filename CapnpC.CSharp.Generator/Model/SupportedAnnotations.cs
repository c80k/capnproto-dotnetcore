using System;
using System.Collections.Generic;
using System.Text;

namespace CapnpC.CSharp.Generator.Model
{
    static class SupportedAnnotations
    {
        static class AnnotationIds
        {
            public static class Cxx
            {
                public const ulong Namespace = 0xb9c6f99ebf805f2c;
            }

            public static class Cs
            {
                public const ulong Namespace = 0xeb0d831668c6eda0;
                public const ulong NullableEnable = 0xeb0d831668c6eda1;
                public const ulong Name = 0xeb0d831668c6eda2;
            }
        }

        public static string[] GetNamespaceAnnotation(Schema.Node.Reader fileNode)
        {
            foreach (var annotation in fileNode.Annotations)
            {
                if (annotation.Id == AnnotationIds.Cs.Namespace)
                {
                    return annotation.Value.Text.Split(new string[1] { "." }, default);
                }

                if (annotation.Id == AnnotationIds.Cxx.Namespace)
                {
                    return annotation.Value.Text.Split(new string[1] { "::" }, default);
                }
            }
            return null;
        }

        public static string GetCsName(Schema.Field.Reader node)
        {
            foreach (var annotation in node.Annotations)
            {
                if (annotation.Id == AnnotationIds.Cs.Name)
                {
                    return annotation.Value.Text;
                }
            }
            return null;
        }

        public static string GetCsName(Schema.Node.Reader node)
        {
            foreach (var annotation in node.Annotations)
            {
                if (annotation.Id == AnnotationIds.Cs.Name)
                {
                    return annotation.Value.Text;
                }
            }
            return null;
        }

        public static string GetCsName(Schema.Method.Reader node)
        {
            foreach (var annotation in node.Annotations)
            {
                if (annotation.Id == AnnotationIds.Cs.Name)
                {
                    return annotation.Value.Text;
                }
            }
            return null;
        }

        public static bool? GetNullableEnable(Schema.Node.Reader node)
        {
            foreach (var annotation in node.Annotations)
            {
                if (annotation.Id == AnnotationIds.Cs.NullableEnable && annotation.Value.IsBool)
                {
                    return annotation.Value.Bool;
                }
            }
            return null;
        }
    }
}
