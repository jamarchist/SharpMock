using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallReplacer : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;

        public SpecifiedMethodCallReplacer(IMetadataHost host)
        {
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            var callsToOverloads = reflector.From<Faker>().GetAllOverloadsOf("CallsTo");
            if (mutableMethodCall.MethodCallMatchesAnOverload(callsToOverloads))
            {
                var lambda = mutableMethodCall.Arguments[0] as AnonymousDelegate;
                var lambdaBody = lambda.Body as BlockStatement;

                MethodCall firstMethodCall = null;

                if (mutableMethodCall.MethodToCall.IsGeneric)
                {
                    var firstMethodCallDeclaration = lambdaBody.Statements[0] as LocalDeclarationStatement;
                    firstMethodCall = firstMethodCallDeclaration.InitialValue as MethodCall;
                }
                else
                {
                    var firstMethodCallExpression = lambdaBody.Statements[0] as ExpressionStatement;
                    firstMethodCall = firstMethodCallExpression.Expression as MethodCall;
                }

                if (firstMethodCall.IsStaticCall &&
                    MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(firstMethodCall.MethodToCall);
                    firstMethodCall.MethodToCall = replacementCall;
                }
            }
            base.Visit(methodCall);
        }
    }
}