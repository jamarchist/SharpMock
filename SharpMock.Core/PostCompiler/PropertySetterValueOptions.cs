using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core
{
    public class PropertySetterValueOptions : IPropertySetterValueOptions
    {
        private readonly IMethodDefinition setter;
        private readonly IBoundExpression target;
        private readonly IMetadataHost host;
        private readonly ILocalVariableBindings locals;

        public PropertySetterValueOptions(IMethodDefinition setter, IBoundExpression target, IMetadataHost host, ILocalVariableBindings locals)
        {
            this.setter = setter;
            this.locals = locals;
            this.host = host;
            this.target = target;
        }

        public IExpressionStatement To(IExpression value)
        {
            var set = new MethodCall();
            set.ThisArgument = target;
            set.MethodToCall = setter;
            set.Type = host.PlatformType.SystemVoid;
            set.Arguments.Add(value);

            var setStatement = new ExpressionStatement();
            setStatement.Expression = set;

            return setStatement;
        }

        public IExpressionStatement To(string variableName)
        {
            return To(locals[variableName]);
        }
    }
}