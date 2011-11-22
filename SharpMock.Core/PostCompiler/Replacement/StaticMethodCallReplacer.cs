using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallReplacer : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;

        public StaticMethodCallReplacer(IMetadataHost host)
        {
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            if (MethodReferenceReplacementRegistry.HasReplacementFor(mutableMethodCall.MethodToCall))
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