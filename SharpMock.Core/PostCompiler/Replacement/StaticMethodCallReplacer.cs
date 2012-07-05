using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallReplacer : BaseCodeTraverser
    {
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

            var fieldReferenceVisitor = new FieldReferenceVisitor(statement, log);
            fieldReferenceVisitor.Visit(statement);

            var fieldAssignmentVisitor = new FieldAssignmentVisitor(statement, log);
            fieldAssignmentVisitor.Visit(statement);

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