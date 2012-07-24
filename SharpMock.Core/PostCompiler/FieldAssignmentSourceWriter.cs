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
        private readonly IFieldReference field;
        private readonly IMetadataHost host;
        private readonly ILogger log;

        public FieldAssignmentSourceWriter(FakeNamespace fakeNamespace, IMetadataHost host, ILogger log, IFieldReference field)
        {
            this.fakeNamespace = fakeNamespace;
            this.field = field;
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
            var nsType = field.ContainingType.GetNamespaceType();
            var fullNs = nsType.NamespaceBuilder();
            var fullNsWithType = String.Format("{0}.{1}", fullNs, nsType.Name.Value);

            log.WriteTrace("Adding interception target for '{0}'.", fullNsWithType);

            fakeNamespace.AddNamespaces(fullNs);
            fakeNamespace.AddClass(fullNs.ToString(), nsType.Name.Value);

            return fullNsWithType;
        }

        private MethodDefinition AddFakeMethod(string fullyQualifiedTypeName)
        {
            var methodClass = fakeNamespace.Classes[fullyQualifiedTypeName];
            var methodName = String.Format("<assignment>{0}", field.Name.Value);
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
            if (!field.IsStatic)
            {
                fakeMethod.AddParameter(0, "target", field.ContainingType, host, false, false);
            }

            fakeMethod.AddParameter((ushort)(fakeMethod.Parameters.Count + 1), "assignedValue", field.Type, host, false, false);
        }

        private void BuildBody(MethodDefinition fakeMethod)
        {
            var body = new SourceMethodBody(host, null, null);
            body.MethodDefinition = fakeMethod;
            body.LocalsAreZeroed = true;

            var block = new BlockStatement();
            body.Block = block;

            var methodBuilderContext = new ReplacementMethodConstructionContext(host, field, fakeMethod, block, true, log);

            IReplacementMethodBuilder methodBuilder = null;
            if (field.IsStatic)
            {
                methodBuilder = new ReplacementStaticFieldAssignmentBuilder(methodBuilderContext, field);     
            }
            else
            {
                methodBuilder = new ReplacementInstanceFieldAssignmentBuilder(methodBuilderContext, field);
            }

            methodBuilder.BuildMethod();  

            fakeMethod.Body = body;
        }

        private IMethodReference GetFakeMethodReference(IMethodDefinition fakeMethod)
        {
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