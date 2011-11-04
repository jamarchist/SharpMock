using SharpMock.Core.PostCompiler.Construction.ControlFlow;
using SharpMock.Core.PostCompiler.Construction.Conversions;
using SharpMock.Core.PostCompiler.Construction.Declarations;
using SharpMock.Core.PostCompiler.Construction.Definitions;
using SharpMock.Core.PostCompiler.Construction.Expressions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IMethodBodyBuilder
    {
        IUnitReflector Reflector { get; }
        ILocalVariableBindings Locals { get; }
        IDefinitionBuilder Define { get; }
        IInstanceCreator Create { get; }
        IDeclarationBuilder Declare { get; }
        IMethodCallBuilder Call { get; }
        IConverter ChangeType { get; }
        ITypeOperatorBuilder Operators { get; }
        ICompileTimeConstantBuilder Constant { get; }
        IIfStatementBuilder If { get; }
        ICodeReturnStatementBuilder Return { get; }
    }
}
