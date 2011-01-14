using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
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