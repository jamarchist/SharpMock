using SharpMock.Core.PostCompiler.Construction.Methods;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public abstract class ReplacementMethodBuilderBase : MethodBodyBuilder, IReplacementMethodBuilder
    {
        protected ReplacementMethodBuilderBase(ReplacementMethodConstructionContext context) : 
            base(context.Host, context.FakeMethodParameters)
        {
            Context = context;
            SharpMockTypes = new SharpMockTypes(context.Host);
            AddStatement = new CommonStatementsAdder(this, s => context.Block.Statements.Add(s), context.Log);
        }

        protected SharpMockTypes SharpMockTypes { get; private set; }
        protected ReplacementMethodConstructionContext Context { get; private set; }
        protected ICommonStatementsAdder AddStatement { get; private set; }

        public abstract void BuildMethod();
    }
}