using SharpMock.Core.PostCompiler.Construction.Methods;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public abstract class ReplacementMethodBuilderBase : MethodBodyBuilder, IReplacementMethodBuilder
    {
        protected ReplacementMethodBuilderBase(ReplacementMethodConstructionContext context) : 
            base(context.Host, context.FakeMethod.Parameters)
        {
            Context = context;
            SharpMockTypes = new SharpMockTypes(context.Host);
        }

        protected SharpMockTypes SharpMockTypes { get; private set; }
        protected ReplacementMethodConstructionContext Context { get; private set; }
        
        protected abstract void BuildMethodTemplate();

        public void BuildMethod()
        {
            BuildMethodTemplate();
        }
    }
}