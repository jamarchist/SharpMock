using System;
using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.CciExtensions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler
{
    internal class ReplacementParameters
    {
        private readonly List<ReplacementParameterInfo> originals = new List<ReplacementParameterInfo>();

        public ReplacementParameterInfo Target { get; set; }
        public IEnumerable<ReplacementParameterInfo> OriginalParameters { get { return originals; } }
        public IEnumerable<ReplacementParameterInfo> AllParameters
        {
            get
            {
                var all = new List<ReplacementParameterInfo>();
                if (Target != null) all.Add(Target);
                all.AddRange(OriginalParameters);

                return all;
            }
        }

        public void AddOriginalParameter(IParameterDefinition parameter)
        {
            var parameterInfo = new ReplacementParameterInfo();
            parameterInfo.Name = parameter.Name.Value;
            parameterInfo.IsOut = parameter.IsOut;
            parameterInfo.IsRef = parameter.IsByReference;
            parameterInfo.Type = parameter.Type;
            parameterInfo.Definition = parameter;
        }
    }

    internal class ReplacementParameterInfo
    {
        public bool IsOut { get; set; }
        public bool IsRef { get; set; }
        public string Name { get; set; }
        public ITypeReference Type { get; set; }
        public IParameterDefinition Definition { get; set; }
    }

    public class AddInterceptionTargetsToAssembly : IPostCompilerPipelineStep
    {
        private FakeNamespace fakeNamespace;
        private IMetadataHost host;
        private ILogger log;
        private ReplacementRegistry registry;

        public void Execute(PostCompilerContext context)
        {
            host = context.Host;
            log = context.Log;
            registry = context.Registry;

            fakeNamespace = new FakeNamespace(context.AssemblyToAlter, host, log);
            AddInterceptionTargets();    
        }

        private void AddInterceptionTargets()
        {
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);

            foreach (var method in MethodReferenceReplacementRegistry.GetMethodsToIntercept())
            {
                var nsType = method.ContainingType.GetNamespaceType();
                var fullNs = nsType.NamespaceBuilder();
                var fullNsWithType = String.Format("{0}.{1}", fullNs, nsType.Name.Value);

                log.WriteTrace("Adding interception target for '{0}'.", fullNsWithType);

                fakeNamespace.AddNamespaces(fullNs);
                fakeNamespace.AddClass(fullNs.ToString(), nsType.Name.Value);

                var methodClass = fakeNamespace.Classes[fullNsWithType];
                var methodName = method.Name.Value == ".ctor" ? "<constructor>" : method.Name.Value;
                var fakeMethod = methodClass.AddPublicStaticMethod(methodName, method.Type, host);

                // if it's an instance method, we add a parameter at the end for the target
                ushort extraParameters = 0;
                if (!method.ResolvedMethod.IsStatic && !method.ResolvedMethod.IsConstructor)
                {
                    fakeMethod.AddParameter(0, "target", method.ContainingType, host, false, false);
                    extraParameters = 1;
                }

                foreach (var parameter in method.Parameters)
                {
                    fakeMethod.AddParameter((ushort)(parameter.Index + extraParameters), 
                        "p" + parameter.Index, parameter.Type, host, parameter.IsByReference/*IsOut*/, parameter.IsByReference);
                }

                if (method.ResolvedMethod.IsConstructor)
                {
                    fakeMethod.Type = method.ResolvedMethod.ContainingTypeDefinition;
                }

                var customAttribute = new CustomAttribute();
                customAttribute.Constructor = new UnitReflector(host)
                    .From<SharpMockGeneratedAttribute>().GetConstructor(Type.EmptyTypes);
                fakeMethod.Attributes = new List<ICustomAttribute>();
                fakeMethod.Attributes.Add(customAttribute);
                fakeMethod.Body = GetBody(fakeMethod, method);

                var parameterTypes = new List<ITypeDefinition>();
                foreach (var param in fakeMethod.Parameters)
                {
                    parameterTypes.Add(param.Type.ResolvedType);
                }

                var fakeCallReference = new Microsoft.Cci.MethodReference(host, fakeMethod.ContainingTypeDefinition,
                    fakeMethod.CallingConvention, fakeMethod.Type, fakeMethod.Name, 0, parameterTypes.ToArray());

                MethodReferenceReplacementRegistry.ReplaceWith(method, fakeCallReference);
            }

            foreach (var field in registry.GetRegisteredReferences(ReplaceableReferenceTypes.FieldAccessor))
            {
                var fieldReplacementBuilder = new FieldAccessorSourceWriter(fakeNamespace, host, log, field as ReplaceableFieldInfo);
                var fakeCallReference = fieldReplacementBuilder.GetReference();

                registry.RegisterReplacement(field, fakeCallReference);

                //FieldReferenceReplacementRegistry.ReplaceWith(field, fakeCallReference);
            }

            foreach (var field in registry.GetRegisteredReferences(ReplaceableReferenceTypes.FieldAssignment))
            {
                var fieldReplacementBuilder = new FieldAssignmentSourceWriter(fakeNamespace, host, log, field);
                var fakeCallReference = fieldReplacementBuilder.GetReference();

                registry.RegisterReplacement(field, fakeCallReference);
                //FieldAssignmentReplacementRegistry.ReplaceWith(field, fakeCallReference);
            }
        }

        private void AddAlternativeInvocation(BlockStatement block,
            IMethodDefinition fakeMethod, IMethodReference originalCall)
        {
            var context = new ReplacementMethodConstructionContext(host, originalCall, fakeMethod, block, log, null);
            var methodBuilder = context.GetMethodBuilder();

            methodBuilder.BuildMethod();
        }

        private SourceMethodBody GetBody(IMethodDefinition method, IMethodReference originalCall)
        {
            var body = new SourceMethodBody(host, null, null);
            body.MethodDefinition = method;
            body.LocalsAreZeroed = true;

            var block = new BlockStatement();
            body.Block = block;

            AddAlternativeInvocation(block, method, originalCall);

            return body;
        }
    }
}