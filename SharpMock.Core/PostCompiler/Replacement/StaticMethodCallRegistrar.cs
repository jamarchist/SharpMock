using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : BaseCodeTraverser //CodeMutatingVisitor
    {
        public StaticMethodCallRegistrar(IMetadataHost host) : base()
        {
        }

        public override void Visit(IMethodCall methodCall)
        {
            if (methodCall.IsStaticCall)
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
            }
            base.Visit(methodCall);
            //return base.Visit(methodCall);
        }

        //public override IBlockStatement Visit(BlockStatement blockStatement)
        //{
        //    return base.Visit(blockStatement);
        //}
    }
}