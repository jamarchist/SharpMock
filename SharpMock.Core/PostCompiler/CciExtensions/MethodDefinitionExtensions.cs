using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.CciExtensions
{
    public static class MethodDefinitionExtensions
    {
        public static IParameterDefinition AddParameter(this MethodDefinition method, 
            ushort parameterIndex, string parameterName, ITypeReference parameterType, IMetadataHost host, bool isOut, bool isRef)
        {
            var fakeMethodParameter = new ParameterDefinition();
            fakeMethodParameter.ContainingSignature = method;
            fakeMethodParameter.Index = parameterIndex;
            fakeMethodParameter.IsByReference = false;
            fakeMethodParameter.Type = parameterType;
            fakeMethodParameter.Name = host.NameTable.GetNameFor(parameterName);
            fakeMethodParameter.IsOut = isOut;
            fakeMethodParameter.IsByReference = isRef;

            method.Parameters.Add(fakeMethodParameter);

            return fakeMethodParameter;
        }
    }
}
