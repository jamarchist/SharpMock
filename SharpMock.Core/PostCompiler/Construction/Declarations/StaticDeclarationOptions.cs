using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Definitions;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Declarations
{
    public class StaticDeclarationOptions<TReflectionType> : IStaticDeclarationOptions<TReflectionType>
    {
        private readonly IDefinitionBuilder define;
        private readonly string variableName;

        public StaticDeclarationOptions(IDefinitionBuilder define, string variableName)
        {
            this.define = define;
            this.variableName = variableName;
        }

        public LocalDeclarationStatement As(IExpression initialValue)
        {
            var localDeclaration = new LocalDeclarationStatement();
            localDeclaration.LocalVariable = define.VariableOf<TReflectionType>(variableName);
            localDeclaration.InitialValue = initialValue;

            return localDeclaration;
        }
    }
}