using SharpMock.Core.PostCompiler.Construction;
using SharpMock.Core.PostCompiler.Construction.ControlFlow;
using SharpMock.Core.PostCompiler.Construction.Conversions;
using SharpMock.Core.PostCompiler.Construction.Declarations;
using SharpMock.Core.PostCompiler.Construction.Definitions;
using SharpMock.Core.PostCompiler.Construction.Expressions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public abstract class ReplacementMethodBuilderBase : IReplacementMethodBuilder
    {
        protected ReplacementMethodBuilderBase(ReplacementMethodConstructionContext context)
        {
            Context = context;
            SharpMockTypes = new SharpMockTypes(context.Host);
        }

        protected SharpMockTypes SharpMockTypes { get; private set; }
        protected ReplacementMethodConstructionContext Context { get; private set; }
        protected IUnitReflector Reflector { get; private set; }
        protected IDefinitionBuilder Define { get; private set; }
        protected IDeclarationBuilder Declare { get; private set; }
        protected ILocalVariableBindings Locals { get; private set; }
        protected IInstanceCreator Create { get; private set; }
        protected IMethodCallBuilder Call { get; private set; }
        protected IConverter ChangeType { get; private set; }
        protected IStatementBuilder Statements { get; private set; }
        protected ITypeOperatorBuilder Operators { get; private set; }
        protected ICompileTimeConstantBuilder Constant { get; private set; }
        protected IIfStatementBuilder If { get; private set; }

        protected void CreateDslContext()
        {
            Reflector = new UnitReflector(Context.Host);
            Locals = new LocalVariableBindings(Reflector);
            Define = new DefinitionBuilder(Reflector, Locals, Context.Host.NameTable);
            Create = new InstanceCreator(Reflector);
            Declare = new DeclarationBuilder(Define);
            Call = new MethodCallBuilder(Context.Host, Reflector, Locals);
            ChangeType = new Converter(Reflector);
            Statements = new StatementBuilder();
            Operators = new TypeOperatorBuilder(Reflector);
            Constant = new CompileTimeConstantBuilder(Reflector);
            If = new IfStatementBuilder();
        }

        protected abstract void BuildMethodTemplate();

        public void BuildMethod()
        {
            CreateDslContext();
            BuildMethodTemplate();
        }
    }
}