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

                ConstructorOrMethodCall firstMethodCall = null;

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
                        var firstMethodReturn = lambdaBody.Statements[0] as ReturnStatement;
                        firstMethodCall = firstMethodReturn.Expression as MethodCall;

                        if (firstMethodCall == null)
                        {
                            firstMethodCall = firstMethodReturn.Expression as CreateObjectInstance;
                        }
                    }
                }

                if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(firstMethodCall.MethodToCall);
                    firstMethodCall.MethodToCall = replacementCall;

                    if (firstMethodCall is CreateObjectInstance)
                    {
                        
                    }
                    else
                    {
                        var call = firstMethodCall as MethodCall;

                        if (!call.IsStaticCall)
                        {
                            call.Arguments.Insert(0, call.ThisArgument);
                            call.IsStaticCall = true;
                            call.IsVirtualCall = false;
                            call.ThisArgument = CodeDummy.Expression;                            
                        }
                    }
                }
            }

            base.Visit(methodCall);
        }
    }
}