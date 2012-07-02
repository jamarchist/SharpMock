using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementRegistrar : IReplacementRegistrar
    {
        public void RegisterReplacement(object replacementTarget)
        {
            var firstMethodCall = replacementTarget as ConstructorOrMethodCall;

            if (firstMethodCall != null)
            {
                var replaceable = firstMethodCall.MethodToCall.AsReplaceable();
                MethodReferenceReplacementRegistry.AddReplaceable(replaceable);
                MethodReferenceReplacementRegistry.AddMethodToIntercept(firstMethodCall.MethodToCall);
            }
        }
    }

    internal class MethodCallReplacementBuilder : IReplacementBuilder
    {
        public object BuildReplacement(object replacementTarget)
        {
            var firstMethodCall = replacement as ConstructorOrMethodCall;

            if (firstMethodCall != null)
            {
                if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall.AsReplaceable()))
                //if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(firstMethodCall.MethodToCall);
                    firstMethodCall.MethodToCall = replacementCall;

                    if (firstMethodCall is CreateObjectInstance)
                    {
                        var newCall = new MethodCall();
                        newCall.Type = firstMethodCall.Type;
                        newCall.Arguments = firstMethodCall.Arguments;
                        newCall.Locations = firstMethodCall.Locations;
                        newCall.MethodToCall = replacementCall;
                        newCall.IsStaticCall = true;

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
        }
    }

    internal class MethodCallReplacer : IReplacer
    {
        public void ReplaceWith(object replacement)
        {
            var firstMethodCall = replacement as ConstructorOrMethodCall;

            if (firstMethodCall != null)
            {
                if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall.AsReplaceable()))
                //if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(firstMethodCall.MethodToCall);
                    firstMethodCall.MethodToCall = replacementCall;

                    if (firstMethodCall is CreateObjectInstance)
                    {
                        var newCall = new MethodCall();
                        newCall.Type = firstMethodCall.Type;
                        newCall.Arguments = firstMethodCall.Arguments;
                        newCall.Locations = firstMethodCall.Locations;
                        newCall.MethodToCall = replacementCall;
                        newCall.IsStaticCall = true;

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
        }
    }

    internal class MethodCallReplacementFactory : IReplacementFactory
    {
        public IReplacementRegistrar GetRegistrar()
        {
            return new MethodCallReplacementRegistrar();
        }

        public IReplacementBuilder GetBuilder()
        {
            throw new NotImplementedException();
        }

        public IReplacer GetReplacer()
        {
            throw new NotImplementedException();
        }
    }

    internal interface IReplacementRegistrar
    {
        void RegisterReplacement(object replacementTarget);
    }

    internal interface IReplacementBuilder
    {
        object BuildReplacement(object replacementTarget);
    }

    internal interface IReplacer
    {
        void ReplaceWith(object replacement);
    }

    internal interface IReplacementFactory
    {
        IReplacementRegistrar GetRegistrar();
        IReplacementBuilder GetBuilder();
        IReplacer GetReplacer();
    }

    internal class LambdaParser
    {
        internal IReplacementFactory GetReplacementFactory()
        {
            return new MethodCallReplacementFactory();
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

            if (IsFieldReference())
            {
                //throw new NotImplementedException("FieldReference replacement is not yet implemented.");
                return null;
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

            return FirstStatementAs<ExpressionStatement>().Expression as MethodCall != null;
        }

        private TExpression FirstStatementAs<TExpression>() where TExpression : class
        {
            return (lambda.Body as BlockStatement).Statements[0] as TExpression;
        }

        private bool IsFieldReference()
        {
            if (!ReturnsValue()) return false;

            var returnStatement = FirstStatementAs<ReturnStatement>();
            var fieldBinding = returnStatement.Expression as BoundExpression;
            if (fieldBinding == null) return false;

            var field = fieldBinding.Definition as FieldReference;
            if (field == null) return false;

            return true;
        }
    }
}