using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.CciExtensions
{
    public static class NamespaceTypeDefinitionExtensions
    {
        public static MethodDefinition AddPublicStaticMethod(this NamespaceTypeDefinition type, string methodName, ITypeReference returnType, IMetadataHost host)
        {
            if (type.Methods == null) type.Methods = new List<IMethodDefinition>();

            var fakeMethod = new MethodDefinition();
            fakeMethod.ContainingTypeDefinition = type;
            fakeMethod.InternFactory = host.InternFactory;
            fakeMethod.IsCil = true;
            fakeMethod.IsStatic = true;
            fakeMethod.Name = host.NameTable.GetNameFor(methodName);
            fakeMethod.Type = returnType;
            fakeMethod.Visibility = TypeMemberVisibility.Public;
            fakeMethod.Parameters = new List<IParameterDefinition>();

            type.Methods.Add(fakeMethod);

            return fakeMethod;
        }
    }
}
