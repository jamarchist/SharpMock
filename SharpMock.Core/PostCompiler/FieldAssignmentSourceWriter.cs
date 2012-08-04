using System;
using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.CciExtensions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    internal class FieldAssignmentSourceWriter
    {
        private readonly FakeNamespace fakeNamespace;
        private readonly IMetadataHost host;
        private readonly ILogger log;
        private readonly ReplaceableFieldInfo fieldInfo;

        public FieldAssignmentSourceWriter(FakeNamespace fakeNamespace, IMetadataHost host, ILogger log, IReplaceableReference fieldInfo)
        {
            this.fieldInfo = fieldInfo as ReplaceableFieldInfo;
            this.fakeNamespace = fakeNamespace;
            this.host = host;
            this.log = log;
        }

        public IMethodReference GetReference()
        {
            var fullyQualifiedTypeName = AddFakeNamespacesAndClass();
            var fakeMethod = AddFakeMethod(fullyQualifiedTypeName);
            AddCustomAttribute(fakeMethod);
            AddParameters(fakeMethod);
            BuildBody(fakeMethod);
            return GetFakeMethodReference(fakeMethod);
        }

        private string AddFakeNamespacesAndClass()
        {
            var fullNamespace = fieldInfo.DeclaringType.Namespace;
            var fullNamespaceWithType = String.Format("{0}.{1}", fullNamespace, fieldInfo.DeclaringType.Name);

            log.WriteTrace("Adding interception target for '{0}'.", fullNamespaceWithType);

            fakeNamespace.AddNamespaces(fullNamespace);
            fakeNamespace.AddClass(fullNamespace, fieldInfo.DeclaringType.Name);

            return fullNamespaceWithType;
        }

        private MethodDefinition AddFakeMethod(string fullyQualifiedTypeName)
        {
            var methodClass = fakeNamespace.Classes[fullyQualifiedTypeName];
            var methodName = String.Format("<assignment>{0}", fieldInfo.Name);
            return methodClass.AddPublicStaticMethod(methodName, host.PlatformType.SystemVoid, host);
        }

        private void AddCustomAttribute(MethodDefinition fakeMethod)
        {
            var customAttribute = new CustomAttribute();
            customAttribute.Constructor = new UnitReflector(host)
                .From<SharpMockGeneratedAttribute>().GetConstructor(Type.EmptyTypes);
            fakeMethod.Attributes = new List<ICustomAttribute>();
            fakeMethod.Attributes.Add(customAttribute);
        }

        private void AddParameters(MethodDefinition fakeMethod)
        {
            var reflector = new UnitReflector(host);
            var containingType = reflector.Get(fieldInfo.DeclaringType.FullName);
            var field = reflector.From(containingType).GetField(fieldInfo.Name);
            
            if (!field.IsStatic)
            {
                fakeMethod.AddParameter(0, "target", containingType, host, false, false);
            }

            fakeMethod.AddParameter((ushort)(fakeMethod.Parameters.Count), "assignedValue", field.Type, host, false, false);
        }

        private void BuildBody(MethodDefinition fakeMethod)
        {
            var body = new SourceMethodBody(host, null, null);
            body.MethodDefinition = fakeMethod;
            body.LocalsAreZeroed = true;

            var block = new BlockStatement();
            body.Block = block;

            var methodBuilderContext = new ReplacementMethodConstructionContext(host, null, fakeMethod, block, true, log, fieldInfo);

            IReplacementMethodBuilder methodBuilder = null;

            var reflector = new UnitReflector(host);
            var containingType = reflector.Get(fieldInfo.DeclaringType.FullName);
            var field = reflector.From(containingType).GetField(fieldInfo.Name);

            if (field.IsStatic)
            {
                methodBuilder = new ReplacementStaticFieldAssignmentBuilder(methodBuilderContext, fieldInfo);     
            }
            else
            {
                methodBuilder = new ReplacementInstanceFieldAssignmentBuilder(methodBuilderContext, fieldInfo);
            }

            methodBuilder.BuildMethod();  

            fakeMethod.Body = body;
        }

        private IMethodReference GetFakeMethodReference(IMethodDefinition fakeMethod)
        {
            var reflector = new UnitReflector(host);
            var containingType = reflector.Get(fieldInfo.DeclaringType.FullName);
            var field = reflector.From(containingType).GetField(fieldInfo.Name);

            var parameterTypes = new List<ITypeReference>();
            if (!field.IsStatic)
            {
                parameterTypes.Add(field.ContainingType);
            }

            parameterTypes.Add(field.Type.ResolvedType);

            var fakeCallReference = new Microsoft.Cci.MethodReference(host, fakeMethod.ContainingTypeDefinition,
                fakeMethod.CallingConvention, fakeMethod.Type, fakeMethod.Name, 0, parameterTypes.ToArray());

            return fakeCallReference;
        }
    }
}