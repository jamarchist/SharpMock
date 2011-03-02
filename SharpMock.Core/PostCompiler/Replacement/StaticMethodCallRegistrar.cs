using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Helpers;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Syntax;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;

        public StaticMethodCallRegistrar(IMetadataHost host)
        {
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            if (methodCall.IsStaticCall || methodCall.ThisArgument.Type.ResolvedType.IsSealed)
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
            }

            base.Visit(methodCall);
        }

        //public override IBlockStatement Visit(BlockStatement blockStatement)
        //{
        //    return base.Visit(blockStatement);
        //}
    }
}