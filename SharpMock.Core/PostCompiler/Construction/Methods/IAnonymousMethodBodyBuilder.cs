using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IAnonymousMethodBodyBuilder
    {
        IExpression WithBody(VoidAction<ICodeBuilder> code);
    }
}