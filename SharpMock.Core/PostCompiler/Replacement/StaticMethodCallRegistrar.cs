using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly SpecifiedCodeMatcher matcher;
        private readonly ILogger log;

        public StaticMethodCallRegistrar(IMetadataHost host, string assemblyLocation, ILogger log)
        {
            this.log = log;
            reflector = new UnitReflector(host);
            matcher = new SpecifiedCodeMatcher(assemblyLocation, reflector);
        }

        public override void Visit(IFieldReference fieldReference)
        {
            if (matcher.ShouldReplace(fieldReference))
            {
                FieldReferenceReplacementRegistry.AddFieldToIntercept(fieldReference);
            }
        }

        public override void Visit(ICreateObjectInstance createObjectInstance)
        {
            var container = createObjectInstance.Type as INamedEntity;
            var containerName = container == null ? "<unknown>" : container.Name.Value;

            log.WriteTrace("StaticMethodCallRegistrar visiting newobj '{0}.{1}'.",
                containerName, createObjectInstance.MethodToCall.Name.Value);

            VisitCallOrConstructor(createObjectInstance.MethodToCall);

            base.Visit(createObjectInstance);
        }

        public override void Visit(IMethodCall methodCall)
        {
            log.WriteTrace("StaticMethodCallRegistrar visiting call '{0}.{1}'.", 
                (methodCall.MethodToCall.ContainingType as INamedEntity).Name.Value, 
                methodCall.MethodToCall.Name.Value);
            
            VisitCallOrConstructor(methodCall.MethodToCall);

            base.Visit(methodCall);
        }

        private void VisitCallOrConstructor(IMethodReference methodToCall)
        {
            if (matcher.ShouldReplace(methodToCall))
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodToCall);
            }            
        }

        private interface IReplacementMatcher
        {
            bool ShouldReplace(IMethodReference methodToCall);
        }

        private class SpecifiedCodeMatcher : IReplacementMatcher
        {
            private readonly List<ReplaceableMethodInfo> specdReplacements;
            private readonly List<ReplaceableFieldAccessorInfo> specdFieldAccessors; 
            private readonly IUnitReflector reflector;

            public SpecifiedCodeMatcher(string assemblyLocation, IUnitReflector reflector)
            {
                this.reflector = reflector;
                var serializer = new ReplaceableCodeInfoSerializer(assemblyLocation);
                var specifiedCode = serializer.DeserializeAllSpecifications();
                var specifiedMethods = specifiedCode.Methods;
                var specifiedFieldAccessors = specifiedCode.FieldAccessors;
                //var assemblies = new List<string>();
                specdReplacements = new List<ReplaceableMethodInfo>();
                specdFieldAccessors = new List<ReplaceableFieldAccessorInfo>();

                //specifiedMethods.ForEach(m =>
                //{
                //    if (!assemblies.Contains(m.DeclaringType.Assembly.AssemblyPath))
                //        assemblies.Add(m.DeclaringType.Assembly.AssemblyPath);
                //});

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

                foreach (var field in specifiedFieldAccessors)
                {
                    var assembly = Assembly.LoadFrom(field.DeclaringType.Assembly.AssemblyPath);
                    var declaringType = assembly.GetType(String.Format("{0}.{1}", field.DeclaringType.Namespace, field.DeclaringType.Name));

                    var fieldDefinition = reflector.From(declaringType).GetField(field.Name);
                    specdFieldAccessors.Add(fieldDefinition.AsReplaceable());
                }
            }

            public bool ShouldReplace(IMethodReference methodToCall)
            {
                var matches =
                    specdReplacements.FindAll(
                        m => new ReplaceableMethodInfoComparer().Equals(m, methodToCall.AsReplaceable()));

                return matches.Count > 0;
            }

            public bool ShouldReplace(IFieldReference fieldReference)
            {
                var matches = specdFieldAccessors.FindAll(
                    f => new ReplaceableFieldAccessorInfoComparer().Equals(f, fieldReference.ResolvedField.AsReplaceable()));

                return matches.Count > 0;
            }
        }
    }
}