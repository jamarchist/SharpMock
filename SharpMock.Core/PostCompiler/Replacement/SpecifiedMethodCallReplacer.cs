using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallReplacer : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;
        private readonly IMetadataHost host;
        private readonly ILogger log;

        public SpecifiedMethodCallReplacer(IMetadataHost host, ILogger log)
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
                var target = parser.GetFirstMethodCall();

                var factory = parser.GetReplacementFactory();

                var builder = factory.GetBuilder();
                var replacement = builder.BuildReplacement();

                var replacer = factory.GetReplacer();
                replacer.ReplaceWith(replacement);
            }

            base.Visit(methodCall);
        }
    }
}