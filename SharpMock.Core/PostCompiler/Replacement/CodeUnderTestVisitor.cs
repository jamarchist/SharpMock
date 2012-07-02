using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class CodeUnderTestVisitor : BaseCodeTraverser
    {
        private readonly IMetadataHost host;
        private readonly IUnitReflector reflector;
        private readonly ILogger log;

        public CodeUnderTestVisitor(IMetadataHost host, ILogger log)
        {
            this.host = host;
            this.log = log;

            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodDefinition method)
        {
            var sharpMockGenerated = reflector.Get<SharpMockGeneratedAttribute>().ResolvedType;
            var methodAttributes = new List<ICustomAttribute>(method.Attributes);

            log.WriteTrace("Method '{0}' has {1} custom attributes.", method.Name.Value, methodAttributes.Count);

            var sharpMockAttributes = methodAttributes.FindAll(a => a.Constructor.ContainingType.ResolvedType.Equals(sharpMockGenerated));

            if (sharpMockAttributes.Count == 0)
            {
                var replacer = new StaticMethodCallReplacer(host, log);
                replacer.Visit(method);
            }
            else
            {
                var container = method.ContainingType as INamedEntity;
                var containerName = container == null ? "<unknown>" : container.Name.Value;
                log.WriteTrace("Skipping visit to '{0}.{1}' because SharpMockGeneratedAttribute was found.", containerName, method.Name.Value);
            }

            base.Visit(method);
        }
    }
}