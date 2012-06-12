using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Cci;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Interception.Registration
{
    public static class ReplaceableTypeExtensions
    {
        public static ReplaceableMethodInfo AsReplaceable(this MethodInfo methodInfo)
        {
            var replaceable = new ReplaceableMethodInfo();
            replaceable.DeclaringType = methodInfo.DeclaringType.AsReplaceable();
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
            assembly.AssemblyFullName = type.Assembly.FullName;
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
            replaceable.ReturnType = methodReference.Type.AsReplaceable();

            replaceable.Parameters = new List<ReplaceableParameterInfo>();
            foreach (var parameter in methodReference.Parameters)
            {
                var replaceableParameter = new ReplaceableParameterInfo();
                replaceableParameter.Name = String.Format("p{0}", parameter.Index);
                replaceableParameter.Index = parameter.Index;
                replaceableParameter.ParameterType = parameter.Type.AsReplaceable();
            }

            replaceable.DeclaringType = declaringType;

            return replaceable;
        }

        internal static ReplaceableTypeInfo AsReplaceable(this ITypeReference typeReference)
        {
            var replaceable = new ReplaceableTypeInfo();
            var namespaces = new ReverseStringBuilder();
            var currentNamespace = (typeReference as INamespaceTypeReference).ContainingUnitNamespace;
            namespaces.Prepend(currentNamespace.ResolvedUnitNamespace.Name.Value);
            while (currentNamespace != null)
            {
                namespaces.Prepend(currentNamespace.ResolvedUnitNamespace.Name.Value);
                currentNamespace = (currentNamespace as INestedUnitNamespaceReference);
            }

            replaceable.Namespace = namespaces.ToString();
            replaceable.Name = (typeReference as INamedTypeReference).Name.Value;

            var assembly = new ReplaceableAssemblyInfo();
            assembly.AssemblyFullName = typeReference.Namespace();
            assembly.AssemblyPath = typeReference.AssemblyPath();

            replaceable.Assembly = assembly;

            return replaceable;
        }

        internal static string AssemblyPath(this ITypeReference typeReference)
        {
            var assembly = (typeReference as INamespaceTypeReference).ContainingUnitNamespace.Unit.ResolvedUnit as IAssembly;
            return assembly.Location;
        }

        internal static string Namespace(this ITypeReference typeReference)
        {
            var namespaceType = typeReference as INamespaceTypeReference;

            var namespaceBuilder = new ReverseStringBuilder();
            namespaceType.ContainingUnitNamespace.AddParentNamespaces(namespaceBuilder);

            return namespaceBuilder.ToString();
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
    } 
}
