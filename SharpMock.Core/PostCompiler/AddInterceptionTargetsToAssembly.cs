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
    public class AddInterceptionTargetsToAssembly : IPostCompilerPipelineStep
    {
        private FakeNamespace fakeNamespace;
        private IMetadataHost host;
        private ILogger log;

        public void Execute(PostCompilerContext context)
        {
            host = context.Host;
            log = context.Log;

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

            foreach (var field in FieldReferenceReplacementRegistry.GetFieldsToIntercept())
            {
                ////var nsType = field.ContainingType.GetNamespaceType();
                ////var fullNs = nsType.NamespaceBuilder();
                ////var fullNsWithType = String.Format("{0}.{1}", fullNs, nsType.Name.Value);

                ////log.WriteTrace("Adding interception target for '{0}'.", fullNsWithType);

                ////fakeNamespace.AddNamespaces(fullNs);
                ////fakeNamespace.AddClass(fullNs.ToString(), nsType.Name.Value);

                ////var methodClass = fakeNamespace.Classes[fullNsWithType];
                ////var methodName = String.Format("<accessor>{0}", field.Name.Value);
                ////var fakeMethod = methodClass.AddPublicStaticMethod(methodName, field.Type, host);

                ////var customAttribute = new CustomAttribute();
                ////customAttribute.Constructor = new UnitReflector(host)
                ////    .From<SharpMockGeneratedAttribute>().GetConstructor(Type.EmptyTypes);
                ////fakeMethod.Attributes = new List<ICustomAttribute>();
                ////fakeMethod.Attributes.Add(customAttribute);
                ////fakeMethod.Body = GetBody(fakeMethod, field, false);

                ////var parameterTypes = new List<ITypeDefinition>();
                //////foreach (var param in fakeMethod.Parameters)
                //////{
                //////    parameterTypes.Add(param.Type.ResolvedType);
                //////}

                ////var fakeCallReference = new Microsoft.Cci.MethodReference(host, fakeMethod.ContainingTypeDefinition,
                ////    fakeMethod.CallingConvention, fakeMethod.Type, fakeMethod.Name, 0, parameterTypes.ToArray());

                var fieldReplacementBuilder = new FieldAccessorSourceWriter(fakeNamespace, host, log, field);
                var fakeCallReference = fieldReplacementBuilder.GetReference();

                FieldReferenceReplacementRegistry.ReplaceWith(field, fakeCallReference);
            }

            foreach (var field in FieldAssignmentReplacementRegistry.GetFieldsToIntercept())
            {
                var fieldReplacementBuilder = new FieldAssignmentSourceWriter(fakeNamespace, host, log, field);
                var fakeCallReference = fieldReplacementBuilder.GetReference();

                FieldAssignmentReplacementRegistry.ReplaceWith(field, fakeCallReference);
            }
        }

        private void AddAlternativeInvocation(BlockStatement block,
            IMethodDefinition fakeMethod, IMethodReference originalCall)
        {
            var context = new ReplacementMethodConstructionContext(host, originalCall, fakeMethod, block, log);
            var methodBuilder = context.GetMethodBuilder();

            methodBuilder.BuildMethod();
        }

        private void AddAlternativeInvocation(BlockStatement block,
            IMethodDefinition fakeMethod, IFieldReference originalField, bool isAssignment)
        {
            var context = new ReplacementMethodConstructionContext(host, originalField, fakeMethod, block, isAssignment, log);
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

        private SourceMethodBody GetBody(IMethodDefinition method, IFieldReference originalField, bool isAssignment)
        {
            var body = new SourceMethodBody(host, null, null);
            body.MethodDefinition = method;
            body.LocalsAreZeroed = true;

            var block = new BlockStatement();
            body.Block = block;

            AddAlternativeInvocation(block, method, originalField, isAssignment);

            return body;
        }
    }
}