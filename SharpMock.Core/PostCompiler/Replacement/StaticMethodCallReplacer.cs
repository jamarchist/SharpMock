using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallReplacer : CodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly ILogger log;

        public StaticMethodCallReplacer(IMetadataHost host, ILogger log)
        {
            this.log = log;
            reflector = new UnitReflector(host);
        }

        public override void TraverseChildren(IStatement statement)
        {
            log.WriteTrace("Traversing {0} statement.", statement.GetType().Name);

            var statementVisitor = new NewObjStatementVisitor(statement, log);
            statementVisitor.Traverse(statement);

            var fieldReferenceVisitor = new FieldReferenceVisitor(statement, log);
            fieldReferenceVisitor.Traverse(statement);

            var fieldAssignmentVisitor = new FieldAssignmentVisitor(statement, log);
            fieldAssignmentVisitor.Traverse(statement);

            base.TraverseChildren(statement);
        }

        public override void TraverseChildren(IMethodCall methodCall)
        {
            if (!IsSharpMockGenerated(methodCall))
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

                base.TraverseChildren(methodCall);                
            }
        }

        private bool IsSharpMockGenerated(IMethodCall methodCall)
        {
            foreach (var customAttribute in methodCall.MethodToCall.ResolvedMethod.Attributes)
            {
                if (customAttribute.Constructor.ResolvedMethod.Equals(reflector.From<SharpMockGeneratedAttribute>().GetConstructor(System.Type.EmptyTypes)))
                {
                    return true;
                }
            }

            return false;
        }
    }  
}