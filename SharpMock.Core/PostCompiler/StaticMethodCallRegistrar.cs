using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler
{
    public class StaticMethodCallRegistrar : CodeMutatingVisitor
    {
        public StaticMethodCallRegistrar(IMetadataHost host) : base(host)
        {
        }

        public override IExpression Visit(MethodCall methodCall)
        {
            if (methodCall.IsStaticCall)
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
            }

            return base.Visit(methodCall);
        }

        public override IExpression Visit(BlockExpression blockExpression)
        {
            return base.Visit(blockExpression);
        }

        public override IBlockStatement Visit(BlockStatement blockStatement)
        {
            return base.Visit(blockStatement);
        }

        public override IFieldDefinition Visit(IFieldDefinition fieldDefinition)
        {
            return base.Visit(fieldDefinition);
        }

        public override INestedTypeDefinition Visit(INestedTypeDefinition nestedTypeDefinition)
        {
            return base.Visit(nestedTypeDefinition);
        }

        public override IStatement Visit(ReturnStatement returnStatement)
        {
            return base.Visit(returnStatement);
        }

        public override IExpression Visit(ReturnValue returnValue)
        {
            return base.Visit(returnValue);
        }

        public override IExpression Visit(AnonymousDelegate anonymousDelegate)
        {
            return base.Visit(anonymousDelegate);
        }

        public override IGenericMethodInstanceReference Visit(IGenericMethodInstanceReference genericMethodInstanceReference)
        {
            return base.Visit(genericMethodInstanceReference);
        }
    }
}