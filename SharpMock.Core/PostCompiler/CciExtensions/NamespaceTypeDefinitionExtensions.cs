using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.CciExtensions
{
    public static class NamespaceTypeDefinitionExtensions
    {
        public static MethodDefinition AddPublicStaticMethod(this NamespaceTypeDefinition type, string methodName, ITypeReference returnType, IMetadataHost host)
        {
            var fakeMethod = new MethodDefinition();
            fakeMethod.ContainingTypeDefinition = type;
            fakeMethod.InternFactory = host.InternFactory;
            fakeMethod.IsCil = true;
            fakeMethod.IsStatic = true;
            fakeMethod.Name = host.NameTable.GetNameFor(methodName);
            fakeMethod.Type = returnType;
            fakeMethod.Visibility = TypeMemberVisibility.Public;

            type.Methods.Add(fakeMethod);

            return fakeMethod;
        }
    }
}
