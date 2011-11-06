using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public class CodeBuilder : ICodeBuilder
    {
        private readonly IList<IStatement> statements = new List<IStatement>();
        private readonly IMethodBodyBuilder body;

        public CodeBuilder(IMetadataHost host, IEnumerable<IParameterDefinition> parameters)
        {
            body = new MethodBodyBuilder(host, parameters);
        }

        public void AddLine(Function<IMethodBodyBuilder, IStatement> x)
        {
            statements.Add(x(body));
        }

        internal IList<IStatement> Statements
        {
            get { return statements; }
        } 
    }
}