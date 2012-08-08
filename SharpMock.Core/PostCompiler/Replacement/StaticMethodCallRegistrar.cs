using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : CodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly SpecifiedCodeMatcher matcher;
        private readonly ILogger log;
        private readonly ReplacementRegistry registry;

        public StaticMethodCallRegistrar(IMetadataHost host, string assemblyLocation, ILogger log, ReplacementRegistry registry)
        {
            this.log = log;
            this.registry = registry;
            reflector = new UnitReflector(host);
            matcher = new SpecifiedCodeMatcher(assemblyLocation, reflector, registry);
        }

        public override void TraverseChildren(IFieldReference fieldReference)
        {
            if (matcher.ShouldReplace(fieldReference))
            {
                registry.RegisterReference(fieldReference.AsReplaceable(ReplaceableReferenceTypes.FieldAccessor));
            }

            if (matcher.ShouldReplaceAssignment(fieldReference))
            {
                registry.RegisterReference(fieldReference.AsReplaceable(ReplaceableReferenceTypes.FieldAssignment));    
            }
        }

        public override void TraverseChildren(ICreateObjectInstance createObjectInstance)
        {
            var container = createObjectInstance.Type as INamedEntity;
            var containerName = container == null ? "<unknown>" : container.Name.Value;

            log.WriteTrace("StaticMethodCallRegistrar visiting newobj '{0}.{1}'.",
                containerName, createObjectInstance.MethodToCall.Name.Value);

            VisitCallOrConstructor(createObjectInstance.MethodToCall);

            base.TraverseChildren(createObjectInstance);
        }

        public override void TraverseChildren(IMethodCall methodCall)
        {
            log.WriteTrace("StaticMethodCallRegistrar visiting call '{0}.{1}'.", 
                (methodCall.MethodToCall.ContainingType as INamedEntity).Name.Value, 
                methodCall.MethodToCall.Name.Value);
            
            VisitCallOrConstructor(methodCall.MethodToCall);

            base.TraverseChildren(methodCall);
        }

        private void VisitCallOrConstructor(IMethodReference methodToCall)
        {
            if (matcher.ShouldReplace(methodToCall))
            {
                registry.RegisterReference(methodToCall.AsReplaceable());
            }            
        }

        private interface IReplacementMatcher
        {
            bool ShouldReplace(IMethodReference methodToCall);
        }

        private class SpecifiedCodeMatcher : IReplacementMatcher
        {
            private readonly ReplacementRegistry registry;

            public SpecifiedCodeMatcher(string assemblyLocation, IUnitReflector reflector, ReplacementRegistry registry)
            {
                this.registry = registry;
                var serializer = new ReplaceableCodeInfoSerializer(assemblyLocation);
                var specifiedCode = serializer.DeserializeAllSpecifications();

                registry.Load(specifiedCode);

                var specifiedMethods = specifiedCode.Methods;
                var specifiedFieldAccessors = specifiedCode.FieldAccessors;
                var specifiedFieldAssignments = specifiedCode.FieldAssignments;

                foreach (var method in specifiedMethods)
                {
                    var assembly = Assembly.LoadFrom(method.DeclaringType.Assembly.AssemblyPath);
                    var declaringType = assembly.GetType(
                        String.Format("{0}.{1}", method.DeclaringType.Namespace, method.DeclaringType.Name));
                    foreach (var parameter in method.Parameters)
                    {
                        Assembly.LoadFrom(parameter.ParameterType.Assembly.AssemblyPath);
                    }

                    var overloads = reflector.From(declaringType).GetAllOverloadsOf(method.Name);
                    foreach (var overload in overloads)
                    {
                        registry.RegisterReference(overload.AsReplaceable());
                    }
                }

                foreach (var field in specifiedFieldAccessors)
                {
                    Assembly.LoadFrom(field.DeclaringType.Assembly.AssemblyPath);
                }

                foreach (var field in specifiedFieldAssignments)
                {
                    Assembly.LoadFrom(field.DeclaringType.Assembly.AssemblyPath);
                }
            }

            public bool ShouldReplace(IMethodReference methodToCall)
            {
                return registry.IsRegistered(methodToCall.AsReplaceable());
            }

            public bool ShouldReplace(IFieldReference fieldReference)
            {
                return registry.IsRegistered(fieldReference.AsReplaceable(ReplaceableReferenceTypes.FieldAccessor));
            }

            public bool ShouldReplaceAssignment(IFieldReference field)
            {
                return registry.IsRegistered(field.AsReplaceable(ReplaceableReferenceTypes.FieldAssignment));
            }
        }
    }
}