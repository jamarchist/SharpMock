using System;
using System.Collections.Generic;
using System.Reflection;

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
    }
}
