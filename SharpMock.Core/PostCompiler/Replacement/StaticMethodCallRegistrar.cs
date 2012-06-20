using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly string assemblyLocation;
        private readonly SpecifiedMethodMatcher matcher;

        public StaticMethodCallRegistrar(IMetadataHost host, string assemblyLocation)
        {
            this.assemblyLocation = assemblyLocation;
            reflector = new UnitReflector(host);
            matcher = new SpecifiedMethodMatcher(assemblyLocation, reflector);
        }

        public override void Visit(IMethodCall methodCall)
        {
            if (matcher.ShouldReplace(methodCall))
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
                MethodReferenceReplacementRegistry.AddReplaceable(methodCall.MethodToCall.AsReplaceable());
            }

            base.Visit(methodCall);
        }

        private interface IReplacementMatcher
        {
            bool ShouldReplace(IMethodCall methodCall);
        }

        private class SpecifiedMethodMatcher : IReplacementMatcher
        {
            private readonly List<IMethodDefinition> specifiedDefinitions;
            private readonly List<ReplaceableMethodInfo> specdReplacements; 
            private readonly IUnitReflector reflector;

            public SpecifiedMethodMatcher(string assemblyLocation, IUnitReflector reflector)
            {
                this.reflector = reflector;
                var serializer = new ReplaceableMethodInfoListSerializer(assemblyLocation);
                var specifiedMethods = serializer.DeserializeAllSpecifiedMethods();
                var assemblies = new List<string>();
                specdReplacements = new List<ReplaceableMethodInfo>();
                specifiedMethods.ForEach(m =>
                {
                    if (!assemblies.Contains(m.DeclaringType.Assembly.AssemblyPath))
                        assemblies.Add(m.DeclaringType.Assembly.AssemblyPath);
                });

                specifiedDefinitions = new List<IMethodDefinition>();
                foreach (var method in specifiedMethods)
                {
                    var assembly = Assembly.LoadFrom(method.DeclaringType.Assembly.AssemblyPath);
                    var declaringType = assembly.GetType(
                        String.Format("{0}.{1}", method.DeclaringType.Namespace, method.DeclaringType.Name));
                    var parameters = new List<Type>();
                    foreach (var parameter in method.Parameters)
                    {
                        var parameterAssembly = Assembly.LoadFrom(parameter.ParameterType.Assembly.AssemblyPath);
                        var parameterTypeName = String.Format("{0}.{1}", 
                            parameter.ParameterType.Namespace, parameter.ParameterType.Name);
                        parameters.Add(parameterAssembly.GetType(parameterTypeName));
                    }

                    var overloads = reflector.From(declaringType).GetAllOverloadsOf(method.Name);
                    foreach (var overload in overloads)
                    {
                        specdReplacements.Add(overload.AsReplaceable());    
                    }
                }
            }

            public bool ShouldReplace(IMethodCall methodCall)
            {
                //var matches = specifiedDefinitions
                //    .FindAll(m => m.Equals(methodCall.MethodToCall.ResolvedMethod));

                //return matches.Count > 0;

                var matches =
                    specdReplacements.FindAll(
                        m => new ReplaceableMethodInfoComparer().Equals(m, methodCall.MethodToCall.AsReplaceable()));

                return matches.Count > 0;
            }
        }
    }
}