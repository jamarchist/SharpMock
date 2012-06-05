using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Microsoft.Cci;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly string assemblyLocation;

        public StaticMethodCallRegistrar(IMetadataHost host, string assemblyLocation)
        {
            this.assemblyLocation = assemblyLocation;
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var matchers = new CompositeReplacementMatcher(
                //new RegisteredMethodMatcher(),
                new SpecifiedMethodMatcher(assemblyLocation, reflector),
                new StaticMethodMatcher(),
                new MethodInSealedClassMatcher()
                //new ConstructorMatcher(),
                //new DelegateConstructorMatcher()
            );

            //new RegisteredMethodMatcher().ShouldReplace(methodCall);

            if (matchers.ShouldReplace(methodCall))
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
            }

            base.Visit(methodCall);
        }

        private interface IReplacementMatcher
        {
            bool ShouldReplace(IMethodCall methodCall);
        }

        private class CompositeReplacementMatcher : IReplacementMatcher
        {
            private readonly IReplacementMatcher[] matchers;

            public CompositeReplacementMatcher(params IReplacementMatcher[] matchers)
            {
                this.matchers = matchers;
            }

            public bool ShouldReplace(IMethodCall methodCall)
            {
                foreach (var matcher in matchers)
                {
                    if (matcher.ShouldReplace(methodCall))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private class StaticMethodMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return methodCall.IsStaticCall;
            }
        }

        private class MethodInSealedClassMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return methodCall.ThisArgument.Type.ResolvedType.IsSealed;
            }
        }

        private class SpecifiedMethodMatcher : IReplacementMatcher
        {
            private readonly List<IMethodDefinition> specifiedDefinitions;
            private readonly IUnitReflector reflector;

            public SpecifiedMethodMatcher(string assemblyLocation, IUnitReflector reflector)
            {
                this.reflector = reflector;
                var specifiedMethods = DeserializeSpecifiedMethods(assemblyLocation);
                var assemblies = new List<string>();
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

                    specifiedDefinitions.Add(
                        reflector.From(declaringType).GetMethod(method.Name, parameters.ToArray()));
                }
            }

            public bool ShouldReplace(IMethodCall methodCall)
            {
                var matches = specifiedDefinitions
                    .FindAll(m => m.Equals(methodCall.MethodToCall.ResolvedMethod));

                return matches.Count > 0;
            }

            private List<ReplaceableMethodInfo> DeserializeSpecifiedMethods(string assemblyLocation)
            {
                var assemblyPath = Path.GetDirectoryName(assemblyLocation);
                var files = Directory.GetFiles(assemblyPath, "*.SharpMock.SerializedSpecifications.xml");

                var aggregateList = new List<ReplaceableMethodInfo>();
                foreach (var specList in files)
                {
                    var serializer = new XmlSerializer(typeof(List<ReplaceableMethodInfo>));
                    using (var fileStream = File.Open(specList, FileMode.Open))
                    {
                        var deserializedList = serializer.Deserialize(fileStream) as List<ReplaceableMethodInfo>;
                        aggregateList.AddRange(deserializedList);
                        fileStream.Close();
                    }
                }

                return aggregateList;
            }
        }

        private class ConstructorMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return !methodCall.MethodToCall.ResolvedMethod.IsConstructor;
            }
        }

        private class DelegateConstructorMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return methodCall.MethodToCall.ResolvedMethod.ContainingTypeDefinition.IsDelegate &&
                       new ConstructorMatcher().ShouldReplace(methodCall);
            }
        }

        private class RegisteredMethodMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return MethodReferenceReplacementRegistry.HasReplacementFor(methodCall.MethodToCall);
            }
        }
    }
}