using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public class DeclarationBuilder : IDeclarationBuilder
    {
        private readonly IDefinitionBuilder define;

        public DeclarationBuilder(IDefinitionBuilder define)
        {
            this.define = define;
        }

        public IStaticDeclarationOptions<TReflectionType> Variable<TReflectionType>(string variableName)
        {
            return new StaticDeclarationOptions<TReflectionType>(define, variableName);
        }

        public IDynamicDeclarationOptions Variable(string variableName, ITypeReference type)
        {
            return new DynamicDeclarationOptions(define, variableName, type);
        }
    }
}