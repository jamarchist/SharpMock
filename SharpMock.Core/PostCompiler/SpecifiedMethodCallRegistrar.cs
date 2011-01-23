using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.DelegateTypes;
using SharpMock.Core.Syntax;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.PostCompiler
{
    public class SpecifiedMethodCallRegistrar : CodeMutatingVisitor
    {
        private readonly IMetadataHost host;
        private readonly IUnitReflector reflector;

        public SpecifiedMethodCallRegistrar(IMetadataHost host) : base(host)
        {
            this.host = host;
            reflector = new UnitReflector(host);
        }

        public override IExpression Visit(MethodCall methodCall)
        {
            var fakeCall = reflector.From<Faker>().GetMethod("CallsTo", typeof (VoidAction));
            if (methodCall.MethodToCall.ResolvedMethod.Equals(fakeCall.ResolvedMethod))
            {
                var methodRegistrar = new StaticMethodCallRegistrar(host);
                return methodRegistrar.Visit(methodCall.Arguments[0]);
            }

            return base.Visit(methodCall);
        }
    }
}