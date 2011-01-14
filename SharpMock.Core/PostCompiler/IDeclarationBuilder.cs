using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core
{
    public interface IDeclarationBuilder
    {
        IStaticDeclarationOptions<TReflectionType> Variable<TReflectionType>(string variableName);
        IDynamicDeclarationOptions Variable(string variableName, ITypeReference type);
    }

    public interface IStaticDeclarationOptions<TReflectionType>
    {
        LocalDeclarationStatement As(IExpression initialValue);
    }

    public interface IDynamicDeclarationOptions
    {
        LocalDeclarationStatement As(IExpression initialValue);
    }

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
