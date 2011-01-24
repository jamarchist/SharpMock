using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
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

        public override IBlockStatement Visit(BlockStatement blockStatement)
        {
            return base.Visit(blockStatement);
        }
    }
}