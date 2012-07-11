using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public interface ICommonStatementsAdder
    {
        void DeclareRegistryInterceptor();
        void DeclareInvocation();
        void DeclareInterceptedType(ITypeDefinition type);
        void DeclareParameterTypesArray(int length);
        void DeclareArgumentsList();
        void AssignParameterTypeValue(int index, ITypeDefinition type);
        void CallShouldInterceptOnInterceptor();
        void SetOriginalCallOnInvocation();
        void SetArgumentsOnInvocation();
        void SetTargetOnInvocationToNull();
        void SetTargetOnInvocationToTargetParameter();
        void SetOriginalCallInfoOnInvocation();
        void CallInterceptOnInterceptor();
    }
}