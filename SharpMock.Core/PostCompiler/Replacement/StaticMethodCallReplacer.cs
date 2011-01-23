using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallReplacer : CodeMutatingVisitor
    {
        public StaticMethodCallReplacer(IMetadataHost host) : base(host)
        {
        }

        public override IExpression Visit(MethodCall methodCall)
        {
            if (methodCall.IsStaticCall && MethodReferenceReplacementRegistry.HasReplacementFor(methodCall.MethodToCall))
            {
                var replacementCall = MethodReferenceReplacementRegistry.GetReplacementFor(methodCall.MethodToCall);
                methodCall.MethodToCall = replacementCall;
            }

            return base.Visit(methodCall);
        }
    }
}