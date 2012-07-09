using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallRegistrar : CodeTraverser
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

        public override void TraverseChildren(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            var callsToOverloads = reflector.From(typeof(Replace)).GetAllOverloadsOf("CallsTo");
            if (mutableMethodCall.MethodCallMatchesAnOverload(callsToOverloads))
            {
                var lambda = mutableMethodCall.Arguments[0] as AnonymousDelegate;

                var parser = new LambdaParser(lambda, host, log);

                var replacement = parser.GetReplacementFactory();
                var registrar = replacement.GetRegistrar();
                
                registrar.RegisterReplacement();
            }

            base.TraverseChildren(methodCall);
        }
    }
}