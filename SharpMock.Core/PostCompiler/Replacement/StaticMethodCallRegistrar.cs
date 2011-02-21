using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : BaseCodeTraverser
    {
        public override void Visit(IMethodCall methodCall)
        {
            if (methodCall.IsStaticCall)
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
            }
            base.Visit(methodCall);
        }

        //public override IBlockStatement Visit(BlockStatement blockStatement)
        //{
        //    return base.Visit(blockStatement);
        //}
    }
}