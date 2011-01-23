using System.Diagnostics;
using Microsoft.Cci;
using Microsoft.Cci.MetadataReader;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler
{
    public class StaticMethodCallRegistrar : CodeMutatingVisitor
    {
        public StaticMethodCallRegistrar(IMetadataHost host) : base(host)
        {
        }

        public override IExpression Visit(MethodCall methodCall)
        {
            if (methodCall.IsStaticCall)
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
            }

            return base.Visit(methodCall);
        }
    }
}