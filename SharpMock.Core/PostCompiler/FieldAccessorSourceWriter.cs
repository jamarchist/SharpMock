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
    internal class FieldAccessorSourceWriter
    {
        private readonly FakeNamespace fakeNamespace;
        //private readonly IFieldReference field;
        private readonly IMetadataHost host;
        private readonly ILogger log;
        private readonly ReplaceableFieldInfo fieldInfo;

        public FieldAccessorSourceWriter(FakeNamespace fakeNamespace, IMetadataHost host, ILogger log, ReplaceableFieldInfo fieldInfo)
        {
            this.fakeNamespace = fakeNamespace;
            //this.field = field;
            this.host = host;
            this.log = log;
            this.fieldInfo = fieldInfo;
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
            var reflector = new UnitReflector(host);
            var fieldType = reflector.Get(fieldInfo.FieldType.FullName);

            var methodClass = fakeNamespace.Classes[fullyQualifiedTypeName];
            var methodName = String.Format("<accessor>{0}", fieldInfo.Name);
            return methodClass.AddPublicStaticMethod(methodName, fieldType, host);
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

        }

        private void BuildBody(MethodDefinition fakeMethod)
        {
            var body = new SourceMethodBody(host, null, null);
            body.MethodDefinition = fakeMethod;
            body.LocalsAreZeroed = true;

            var block = new BlockStatement();
            body.Block = block;

            var methodBuilderContext = new ReplacementMethodConstructionContext(host, null, fakeMethod, block, false, log, fieldInfo);
            var methodBuilder = new ReplacementFieldAccessorBuilder(methodBuilderContext, fieldInfo);
            methodBuilder.BuildMethod();

            fakeMethod.Body = body;
        }

        private IMethodReference GetFakeMethodReference(IMethodDefinition fakeMethod)
        {
            var parameterTypes = new List<ITypeReference>();
            var fakeCallReference = new Microsoft.Cci.MethodReference(host, fakeMethod.ContainingTypeDefinition,
                fakeMethod.CallingConvention, fakeMethod.Type, fakeMethod.Name, 0, parameterTypes.ToArray());

            return fakeCallReference;
        }        
    }
}