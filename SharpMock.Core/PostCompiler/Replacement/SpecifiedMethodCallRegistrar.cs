using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.PostCompiler;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallRegistrar : CodeMutatingVisitor
    {
        private readonly IUnitReflector reflector;

        public SpecifiedMethodCallRegistrar(IMetadataHost host) : base(host)
        {
            reflector = new UnitReflector(host);
        }

        public override IExpression Visit(MethodCall methodCall)
        {
            var fakeCall = reflector.From<Faker>().GetMethod("CallsTo", typeof (VoidAction));
            if (methodCall.MethodToCall.ResolvedMethod.Equals(fakeCall.ResolvedMethod))
            {
                var lambda = methodCall.Arguments[0] as AnonymousDelegate;
                var lambdaBody = lambda.Body as BlockStatement;
                var firstMethodCallExpression = lambdaBody.Statements[0] as ExpressionStatement;
                var firstMethodCall = firstMethodCallExpression.Expression as MethodCall;

                if (firstMethodCall.IsStaticCall)
                {
                    MethodReferenceReplacementRegistry.AddMethodToIntercept(firstMethodCall.MethodToCall);
                }

                //var methodRegistrar = new StaticMethodCallRegistrar(host);
                //return methodRegistrar.Visit(methodCall.Arguments[0]);

                //if (methodCall.IsStaticCall)
                //{
                //    MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
                //}
            }

            return base.Visit(methodCall);
        }

        public override IBlockStatement Visit(BlockStatement blockStatement)
        {
            return base.Visit(blockStatement);
        }
    }
}