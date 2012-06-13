using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
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

        public SpecifiedMethodCallRegistrar(IMetadataHost host)
        {
            this.host = host;
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var mutableMethodCall = methodCall as MethodCall;

            var callsToOverloads = reflector.From<Faker>().GetAllOverloadsOf("CallsTo");
            if (mutableMethodCall.MethodCallMatchesAnOverload(callsToOverloads))
            {
                var lambda = mutableMethodCall.Arguments[0] as AnonymousDelegate;

                var parser = new LambdaParser(lambda, host);
                var firstMethodCall = parser.GetFirstMethodCall();

                var replaceable = firstMethodCall.MethodToCall.AsReplaceable();
                MethodReferenceReplacementRegistry.AddReplaceable(replaceable);
                MethodReferenceReplacementRegistry.AddMethodToIntercept(firstMethodCall.MethodToCall);
            }
            base.Visit(methodCall);
        }

    }
}