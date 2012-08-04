using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Interception.Registration
{
    public static class ReplaceableTypeExtensions
    {
        internal static ReplaceableFieldInfo AsReplaceable(this IFieldReference fieldReference, string referenceType)
        {
            var replaceable = new ReplaceableFieldInfo(referenceType);
            replaceable.Name = fieldReference.Name.Value;
            replaceable.FieldType = fieldReference.Type.AsReplaceable();
            replaceable.DeclaringType = fieldReference.ContainingType.AsReplaceable();

            return replaceable;
        }

        public static ReplaceableMethodInfo AsReplaceable(this MethodInfo methodInfo)
        {
            var replaceable = new ReplaceableMethodInfo();
            replaceable.DeclaringType = methodInfo.DeclaringType.AsReplaceable();
            if (methodInfo.IsConstructor)
                replaceable.ReturnType = replaceable.DeclaringType;
            else
                replaceable.ReturnType = methodInfo.ReturnType.AsReplaceable();
            replaceable.Name = methodInfo.Name;
            replaceable.Parameters = new List<ReplaceableParameterInfo>();

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                var parameter = new ReplaceableParameterInfo();
                parameter.Name = parameterInfo.Name;
                parameter.Index = parameterInfo.Position;
                parameter.ParameterType = parameterInfo.ParameterType.AsReplaceable();
            
                replaceable.Parameters.Add(parameter);
            }

            return replaceable;
        }

        private static ReplaceableTypeInfo AsReplaceable(this Type type)
        {
            var assembly = new ReplaceableAssemblyInfo();
            assembly.Name = type.Assembly.GetName().Name;
            assembly.AssemblyPath = type.Assembly.Location;

            var replaceable = new ReplaceableTypeInfo();
            replaceable.Assembly = assembly;
            replaceable.Namespace = type.Namespace;
            replaceable.Name = type.Name;

            return replaceable;
        }

        internal static ReplaceableMethodInfo AsReplaceable(this IMethodReference methodReference)
        {
            var declaringType = methodReference.ContainingType.AsReplaceable();

            var replaceable = new ReplaceableMethodInfo();
            replaceable.Name = methodReference.Name.Value;
            if (methodReference.ResolvedMethod.IsConstructor)
                replaceable.ReturnType = declaringType;
            else
                replaceable.ReturnType = methodReference.Type.AsReplaceable();

            replaceable.Parameters = new List<ReplaceableParameterInfo>();
            foreach (var parameter in methodReference.Parameters)
            {
                var namedParameter = parameter as INamedEntity;
                var name = namedParameter == null ? String.Format("p{0}", parameter.Index) : namedParameter.Name.Value;

                var replaceableParameter = new ReplaceableParameterInfo();
                replaceableParameter.Name = name;
                replaceableParameter.Index = parameter.Index;
                replaceableParameter.ParameterType = parameter.Type.AsReplaceable();

                replaceable.Parameters.Add(replaceableParameter);
            }

            replaceable.DeclaringType = declaringType;

            return replaceable;
        }

        internal static ReplaceableTypeInfo AsReplaceable(this ITypeReference typeReference)
        {
            var replaceable = new ReplaceableTypeInfo();

            var namespaceType = typeReference.GetNamespaceType();

            if (namespaceType == null)
            {
                replaceable.Namespace = String.Empty;   
                replaceable.Assembly = new ReplaceableAssemblyInfo();
                replaceable.Assembly.Name = String.Empty;
                replaceable.Assembly.AssemblyPath = String.Empty;

                replaceable.Name = (typeReference as INamedEntity).Name.Value;
            }
            else
            {
                replaceable.Namespace = namespaceType.Namespace();
                replaceable.Name = namespaceType.Name.Value;

                var assembly = new ReplaceableAssemblyInfo();
                assembly.Name = namespaceType.ContainingUnitNamespace.Unit.Name.Value;
                assembly.AssemblyPath = namespaceType.AssemblyPath();

                replaceable.Assembly = assembly;                
            }

            return replaceable;
        }

        internal static string AssemblyPath(this INamespaceTypeReference namespaceType)
        {
            var assembly = namespaceType.ContainingUnitNamespace.Unit.ResolvedUnit as IAssembly;
            if (assembly == null) return "(unknown)";

            return assembly.Location;
        }

        internal static string Namespace(this INamespaceTypeReference namespaceType)
        {
            return namespaceType.NamespaceBuilder().ToString();
        }

        internal static ReverseStringBuilder NamespaceBuilder(this INamespaceTypeReference namespaceType)
        {
            var namespaceBuilder = new ReverseStringBuilder();
            namespaceType.ContainingUnitNamespace.AddParentNamespaces(namespaceBuilder);

            return namespaceBuilder;
        }

        private static void AddParentNamespaces(this IUnitNamespaceReference ns, ReverseStringBuilder namespaceBuilder)
        {
            var nested = ns as INestedUnitNamespaceReference;
            if (nested != null)
            {
                namespaceBuilder.Prepend(nested.Name.Value);
                namespaceBuilder.Prepend(".");
                nested.ContainingUnitNamespace.AddParentNamespaces(namespaceBuilder);
            }
            else
            {
                namespaceBuilder.Pop();
                // Root
                // namespaceBuilder.Prepend(ns.ResolvedUnitNamespace.Name.Value);
            }
        }

        internal static INamespaceTypeReference GetNamespaceType(this ITypeReference typeReference)
        {
            var generic = typeReference as IGenericTypeInstanceReference;
            var genericParam = typeReference as IGenericTypeParameter;
            var namespaceType = typeReference as INamespaceTypeReference;
            var vector = typeReference as IArrayTypeReference;

            if (vector != null)
            {
                namespaceType = vector.ElementType.GetNamespaceType();
            }

            if (generic != null)
            {
                namespaceType = generic.GenericType as INamespaceTypeReference;
            }

            return namespaceType;
        }
    } 
}
