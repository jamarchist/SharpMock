using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallReplacer : BaseCodeTraverser //CodeMutatingVisitor
    {
        private readonly IUnitReflector reflector;

        public SpecifiedMethodCallReplacer(IMetadataHost host) : base()
        {
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            var fakeCall = reflector.From<Faker>().GetMethod("CallsTo", typeof (VoidAction));
            if (mutableMethodCall.MethodToCall.ResolvedMethod.Equals(fakeCall.ResolvedMethod))
            {
                var lambda = mutableMethodCall.Arguments[0] as AnonymousDelegate;
                var lambdaBody = lambda.Body as BlockStatement;
                var firstMethodCallExpression = lambdaBody.Statements[0] as ExpressionStatement;
                var firstMethodCall = firstMethodCallExpression.Expression as MethodCall;

                if (firstMethodCall.IsStaticCall &&
                    MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(firstMethodCall.MethodToCall);
                    firstMethodCall.MethodToCall = replacementCall;
                }
            }

            //return base.Visit(methodCall);
        }
    }
}