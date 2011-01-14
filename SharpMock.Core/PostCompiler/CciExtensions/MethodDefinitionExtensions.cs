using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CciExtensions
{
    public static class MethodDefinitionExtensions
    {
        public static IParameterTypeInformation AddParameter(this MethodDefinition method, 
            ushort parameterIndex, string parameterName, ITypeReference parameterType, IMetadataHost host)
        {
            var fakeMethodParameter = new ParameterDefinition();
            fakeMethodParameter.ContainingSignature = method;
            fakeMethodParameter.Index = parameterIndex;
            fakeMethodParameter.IsByReference = false;
            fakeMethodParameter.Type = parameterType;
            fakeMethodParameter.Name = host.NameTable.GetNameFor(parameterName);

            method.Parameters.Add(fakeMethodParameter);

            return fakeMethodParameter;
        }
    }
}
