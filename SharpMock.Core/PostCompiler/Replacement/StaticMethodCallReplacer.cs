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

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;
            var method = mutableMethodCall.MethodToCall.AsReplaceable();

            log.WriteTrace("Finding replacement for {0}.{1} in '{2}' in '{3}'",
                method.DeclaringType.Name, method.Name, method.DeclaringType.Assembly.AssemblyPath, method.DeclaringType.Assembly.AssemblyFullName);

            if (MethodReferenceReplacementRegistry.HasReplacementFor(method))
            //if (MethodReferenceReplacementRegistry.HasReplacementFor(mutableMethodCall.MethodToCall))
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
            }
            
            base.Visit(methodCall);
        }
    }  
}