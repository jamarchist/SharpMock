using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class LambdaParser
    {
        private readonly AnonymousDelegate lambda;
        private readonly IMetadataHost host;

        public LambdaParser(AnonymousDelegate lambda, IMetadataHost host)
        {
            this.lambda = lambda;
            this.host = host;
        }

        public ConstructorOrMethodCall GetFirstMethodCall()
        {
            if (IsStaticMethodCallWithReturnValue())
            {
                return FirstStatementAs<ReturnStatement>().Expression as MethodCall;
            }

            if (IsVoidMethodCall())
            {
                return FirstStatementAs<ExpressionStatement>().Expression as MethodCall;
            }

            if (IsConstructor())
            {
                return FirstStatementAs<ReturnStatement>().Expression as CreateObjectInstance;
            }

            return FirstStatementAs<ReturnStatement>().Expression as MethodCall;
        }

        private bool IsStaticMethodCallWithReturnValue()
        {
            return IsStaticMethodCall() && !IsVoidMethodCall();
        }

        private bool IsStaticMethodCall()
        {
            return lambda.Parameters.Count == 0;
        }

        private bool IsVoidMethodCall()
        {
            return lambda.ReturnType.ResolvedType.Equals(host.PlatformType.SystemVoid.ResolvedType);
        }

        private bool IsConstructor()
        {
            return FirstStatementAs<ReturnStatement>().Expression as CreateObjectInstance != null;
        }

        private TExpression FirstStatementAs<TExpression>() where TExpression : class
        {
            return (lambda.Body as BlockStatement).Statements[0] as TExpression;
        }
    }
}