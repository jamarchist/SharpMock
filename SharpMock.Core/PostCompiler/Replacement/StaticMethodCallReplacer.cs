using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallReplacer : BaseCodeTraverser //CodeMutatingVisitor
    {
        public StaticMethodCallReplacer(IMetadataHost host) : base()
        {
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            if (mutableMethodCall.IsStaticCall && MethodReferenceReplacementRegistry.HasReplacementFor(mutableMethodCall.MethodToCall))
            {
                var replacementCall = MethodReferenceReplacementRegistry.GetReplacementFor(mutableMethodCall.MethodToCall);
                mutableMethodCall.MethodToCall = replacementCall;
            }
            base.Visit(methodCall);
            //return base.Visit(methodCall);
        }
    }
}