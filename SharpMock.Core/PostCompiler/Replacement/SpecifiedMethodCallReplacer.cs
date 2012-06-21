using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.Core.Interception.Helpers;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallReplacer : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly IMetadataHost host;

        public SpecifiedMethodCallReplacer(IMetadataHost host)
        {
            this.host = host;
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            var callsToOverloads = reflector.From(typeof(Replace)).GetAllOverloadsOf("CallsTo");
            if (mutableMethodCall.MethodCallMatchesAnOverload(callsToOverloads))
            {
                var lambda = mutableMethodCall.Arguments[0] as AnonymousDelegate;

                var parser = new LambdaParser(lambda, host);
                var firstMethodCall = parser.GetFirstMethodCall();

                if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall.AsReplaceable()))
                //if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(firstMethodCall.MethodToCall);
                    firstMethodCall.MethodToCall = replacementCall;

                    if (firstMethodCall is CreateObjectInstance)
                    {
                        
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

            base.Visit(methodCall);
        }
    }
}