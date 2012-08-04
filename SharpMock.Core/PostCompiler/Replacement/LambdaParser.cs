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

                log.WriteTrace("lambda identified as constructor.");
                return new ConstructorReplacementFactory(constructor, firstStatement, registry);                
            }

            if (IsFieldAccessor())
            {
                var returnStatement = FirstStatementAs<ReturnStatement>();
                var fieldBinding = returnStatement.Expression as BoundExpression;
                var field = fieldBinding.Definition as FieldReference;

                log.WriteTrace("Lambda identified as static field accessor.");
                return new FieldAccessorReplacementFactory(field, returnStatement, registry);
            }

            if (IsFieldAssignment())
            {
                var assignmentStatement = FirstStatementAs<ExpressionStatement>();
                var assignment = assignmentStatement.Expression as Assignment;
                var field = assignment.Target.Definition as FieldReference;

                log.WriteTrace("Lambda identified as static field assignment.");
                return new FieldAssignmentReplacementFactory(field, assignmentStatement, host, registry);
            }

            if (IsStaticMethodCallWithReturnValue() || IsVoidMethodCall() || IsMethodCall())
            {
                log.WriteTrace("Lamda identified as method call.");
                return new MethodCallReplacementFactory(FirstStatementAs<IStatement>(), registry);                
            }

            log.WriteTrace("Could not find a replacement factory for specified lambda.");
            return new NullReplacementFactory();
        }

        private readonly AnonymousDelegate lambda;
        private readonly IMetadataHost host;
        private readonly ILogger log;
        private readonly ReplacementRegistry registry;

        internal LambdaParser(AnonymousDelegate lambda, IMetadataHost host, ILogger log, ReplacementRegistry registry)
        {
            this.lambda = lambda;
            this.host = host;
            this.log = log;
            this.registry = registry;
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
    }
}