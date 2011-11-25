using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallRegistrar : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;

        public SpecifiedMethodCallRegistrar(IMetadataHost host)
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

                if (mutableMethodCall.MethodToCall.IsGeneric && lambda.Parameters.Count == 0)
                {
                    var firstMethodCallDeclaration = lambdaBody.Statements[0] as LocalDeclarationStatement;
                    firstMethodCall = firstMethodCallDeclaration.InitialValue as MethodCall;
                }
                else
                {
                    var firstMethodCallExpression = lambdaBody.Statements[0] as ExpressionStatement;
                    if (firstMethodCallExpression != null)
                    {
                        firstMethodCall = firstMethodCallExpression.Expression as MethodCall;    
                    }
                    else
                    {
                        var firstMethodCallReturn = lambdaBody.Statements[0] as ReturnStatement;
                        firstMethodCall = firstMethodCallReturn.Expression as MethodCall;
                    }
                }

                MethodReferenceReplacementRegistry.AddMethodToIntercept(firstMethodCall.MethodToCall);
                //MethodReferenceReplacementRegistry.AddSpecification(firstMethodCall.MethodToCall);
            }
            base.Visit(methodCall);
        }
    }
}