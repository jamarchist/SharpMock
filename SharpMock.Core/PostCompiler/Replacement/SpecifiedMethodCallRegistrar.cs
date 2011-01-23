using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;
using SharpMock.PostCompiler;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class SpecifiedMethodCallRegistrar : CodeMutatingVisitor
    {
        private readonly IUnitReflector reflector;

        public SpecifiedMethodCallRegistrar(IMetadataHost host) : base(host)
        {
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