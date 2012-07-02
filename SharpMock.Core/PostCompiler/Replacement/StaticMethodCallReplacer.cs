using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallReplacer : BaseCodeTraverser
    {
        private class NewObjStatementVisitor : BaseCodeTraverser
        {
            private readonly IStatement parent;
            private readonly ILogger log;

            public NewObjStatementVisitor(IStatement parent, ILogger log)
            {
                this.parent = parent;
                this.log = log;
            }

            public override void Visit(ICreateObjectInstance createObjectInstance)
            {
                if (MethodReferenceReplacementRegistry.HasReplacementFor(createObjectInstance.MethodToCall.AsReplaceable()))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(createObjectInstance.MethodToCall);

                    var newCall = new MethodCall();
                    newCall.Type = createObjectInstance.Type;
                    newCall.Arguments = new List<IExpression>(createObjectInstance.Arguments);
                    newCall.Locations = new List<ILocation>(createObjectInstance.Locations);
                    newCall.MethodToCall = replacementCall;
                    newCall.IsStaticCall = true;

                    if (parent is LocalDeclarationStatement)
                    {
                        var declaration = parent as LocalDeclarationStatement;
                        declaration.InitialValue = newCall;
                    }
                }

                //base.Visit(createObjectInstance);
            }
        }

        private readonly IUnitReflector reflector;
        private readonly ILogger log;

        public StaticMethodCallReplacer(IMetadataHost host, ILogger log)
        {
            this.log = log;
            reflector = new UnitReflector(host);
        }

        public override void Visit(IStatement statement)
        {
            var statementVisitor = new NewObjStatementVisitor(statement, log);
            statementVisitor.Visit(statement);

            base.Visit(statement);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;
            var method = mutableMethodCall.MethodToCall.AsReplaceable();

            log.WriteTrace("Finding replacement for {0}.{1}", method.DeclaringType.Name, method.Name);
            log.WriteTrace("  in '{0}' at '{1}'", method.DeclaringType.Assembly.Name, method.DeclaringType.Assembly.AssemblyPath);
            
            if (MethodReferenceReplacementRegistry.HasReplacementFor(method))
            {
                var replacementCall =
                    MethodReferenceReplacementRegistry.GetReplacementFor(mutableMethodCall.MethodToCall);
                mutableMethodCall.MethodToCall = replacementCall;
                
                if (!methodCall.IsStaticCall)
                {
                    mutableMethodCall.Arguments.Insert(0, mutableMethodCall.ThisArgument);
                    mutableMethodCall.IsStaticCall = true;
                    mutableMethodCall.IsVirtualCall = false;
                    mutableMethodCall.ThisArgument = CodeDummy.Expression;
                }

                log.WriteTrace("  --REPLACEMENT FOUND--");
            }
            else
            {
                log.WriteTrace("  --NOT FOUND--");                
            }
            
            base.Visit(methodCall);
        }
    }  
}