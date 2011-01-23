using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Definitions;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Declarations
{
    public class DynamicDeclarationOptions : IDynamicDeclarationOptions
    {
        private readonly IDefinitionBuilder define;
        private readonly string variableName;
        private readonly ITypeReference type;

        public DynamicDeclarationOptions(IDefinitionBuilder define, string variableName, ITypeReference type)
        {
            this.define = define;
            this.type = type;
            this.variableName = variableName;
        }

        public LocalDeclarationStatement As(IExpression initialValue)
        {
            var localDeclaration = new LocalDeclarationStatement();
            localDeclaration.LocalVariable = define.VariableOf(variableName, type);
            localDeclaration.InitialValue = initialValue;

            return localDeclaration;
        }
    }
}