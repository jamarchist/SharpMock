using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class LambdaParser
    {
        internal IReplacementFactory GetReplacementFactory()
        {
            if (IsConstructor())
            {
                var firstStatement = FirstStatementAs<ReturnStatement>();
                var constructor = firstStatement.Expression as CreateObjectInstance;

                return new ConstructorReplacementFactory(constructor, firstStatement);                
            }

            if (IsFieldAccessor())
            {
                var returnStatement = FirstStatementAs<ReturnStatement>();
                var fieldBinding = returnStatement.Expression as BoundExpression;
                var field = fieldBinding.Definition as FieldReference;

                return new FieldAccessorReplacementFactory(field, returnStatement);
            }

            if (IsFieldAssignment())
            {
                var assignmentStatement = FirstStatementAs<ExpressionStatement>();
                var assignment = assignmentStatement.Expression as Assignment;
                var field = assignment.Target.Definition as FieldReference;

                return new FieldAssignmentReplacementFactory(field, assignmentStatement, host);
            }

            if (IsStaticMethodCallWithReturnValue() || IsVoidMethodCall() || IsMethodCall())
            {
                return new MethodCallReplacementFactory(FirstStatementAs<IStatement>());                
            }

            return new NullReplacementFactory();
        }

        private readonly AnonymousDelegate lambda;
        private readonly IMetadataHost host;
        private readonly ILogger log;

        internal LambdaParser(AnonymousDelegate lambda, IMetadataHost host, ILogger log)
        {
            this.lambda = lambda;
            this.host = host;
            this.log = log;
        }

        private bool IsStaticMethodCallWithReturnValue()
        {
            return ReturnsValue() && IsMethodCall() && !IsVoidMethodCall();
        }

        private bool IsVoidMethodCall()
        {
            return lambda.ReturnType.ResolvedType.Equals(host.PlatformType.SystemVoid.ResolvedType);
        }

        private bool IsConstructor()
        {
            if (ReturnsValue())
            {
                return FirstStatementAs<ReturnStatement>().Expression as CreateObjectInstance != null;                
            }

            return false;
        }

        private bool ReturnsValue()
        {
            return FirstStatementAs<ReturnStatement>() != null;
        }

        private bool IsMethodCall()
        {
            if (ReturnsValue())
            {
                return FirstStatementAs<ReturnStatement>().Expression as MethodCall != null;                
            }

            if (FirstStatementAs<ExpressionStatement>() != null)
            {
                return FirstStatementAs<ExpressionStatement>().Expression as MethodCall != null;
            }

            return false;
        }

        private TExpression FirstStatementAs<TExpression>() where TExpression : class
        {
            return (lambda.Body as BlockStatement).Statements[0] as TExpression;
        }

        private bool IsFieldAccessor()
        {
            if (!ReturnsValue()) return false;

            var returnStatement = FirstStatementAs<ReturnStatement>();
            var fieldBinding = returnStatement.Expression as BoundExpression;
            if (fieldBinding == null) return false;

            var field = fieldBinding.Definition as FieldReference;
            if (field == null) return false;

            return true;
        }

        private bool IsFieldAssignment()
        {
            if (ReturnsValue()) return false;

            var assignmentStatement = FirstStatementAs<ExpressionStatement>();
            if (assignmentStatement == null) return false;

            var assignment = assignmentStatement.Expression as Assignment;
            if (assignment == null) return false;

            var field = assignment.Target.Definition as FieldReference;
            if (field == null) return false;

            return true;
        }

        private bool IsImproperlyConstructedPropertySetter()
        {
            var block = (lambda.Body as BlockStatement);
            return block.Statements.Count > 2;
        }
    }
}