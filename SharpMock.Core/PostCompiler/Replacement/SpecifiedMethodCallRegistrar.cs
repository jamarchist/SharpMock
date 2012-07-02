using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallRegistrar : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly IMetadataHost host;
        private readonly ILogger log;

        public SpecifiedMethodCallRegistrar(IMetadataHost host, ILogger log)
        {
            this.host = host;
            this.log = log;
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            var callsToOverloads = reflector.From(typeof(Replace)).GetAllOverloadsOf("CallsTo");
            if (mutableMethodCall.MethodCallMatchesAnOverload(callsToOverloads))
            {
                var lambda = mutableMethodCall.Arguments[0] as AnonymousDelegate;

                var parser = new LambdaParser(lambda, host, log);
                var firstMethodCall = parser.GetFirstMethodCall();

                var replaceable = firstMethodCall.MethodToCall.AsReplaceable();
                MethodReferenceReplacementRegistry.AddReplaceable(replaceable);
                MethodReferenceReplacementRegistry.AddMethodToIntercept(firstMethodCall.MethodToCall);
            }

            base.Visit(methodCall);
        }
    }
}