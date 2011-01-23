using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Definitions;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Declarations
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