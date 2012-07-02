using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class LambdaParser
    {
        private readonly AnonymousDelegate lambda;
        private readonly IMetadataHost host;
        private readonly ILogger log;

        internal LambdaParser(AnonymousDelegate lambda, IMetadataHost host, ILogger log)
        {
            this.lambda = lambda;
            this.host = host;
            this.log = log;
        }

        public ConstructorOrMethodCall GetFirstMethodCall()
        {
            if (IsStaticMethodCallWithReturnValue() && !IsConstructor())
            {
                log.WriteTrace("MethodCall identified as static with return value.");
                return FirstStatementAs<ReturnStatement>().Expression as MethodCall;
            }

            if (IsVoidMethodCall())
            {
                log.WriteTrace("MethodCall identified as void.");
                return FirstStatementAs<ExpressionStatement>().Expression as MethodCall;
            }

            if (IsConstructor())
            {
                log.WriteTrace("MethodCall identified as .ctor.");
                return FirstStatementAs<ReturnStatement>().Expression as CreateObjectInstance;
            }

            log.WriteTrace("MethodCall defaulted to instance with return value.");
            return FirstStatementAs<ReturnStatement>().Expression as MethodCall;
        }

        /// <summary>
        /// This is required for intercepting constructors
        /// </summary>
        /// <param name="replacementCall"></param>
        public void ReplaceFirstCall(ConstructorOrMethodCall replacementCall)
        {
            var firstStatement = FirstStatementAs<ReturnStatement>();
            firstStatement.Expression = replacementCall;
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