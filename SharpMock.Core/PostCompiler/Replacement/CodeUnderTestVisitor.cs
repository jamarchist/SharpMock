using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class CodeUnderTestVisitor : CodeTraverser
    {
        private readonly IMetadataHost host;
        private readonly IUnitReflector reflector;
        private readonly ILogger log;
        private readonly ReplacementRegistry registry;

        public CodeUnderTestVisitor(IMetadataHost host, ILogger log, ReplacementRegistry registry)
        {
            this.host = host;
            this.log = log;
            this.registry = registry;

            reflector = new UnitReflector(host);
        }

        public override void TraverseChildren(ITypeDefinition typeDefinition)
        {
            var named = typeDefinition as INamedEntity;
            var name = typeDefinition == null ? "<unknown type>" : named.Name.Value;
            log.WriteTrace("Traversing {0}.", name);
            base.TraverseChildren(typeDefinition);
        }

        public override void TraverseChildren(IMethodDefinition method)
        {
            if (!IsSharpMockGenerated(method))
            {
                var replacer = new StaticMethodCallReplacer(host, log, registry);
                replacer.TraverseChildren(method);

                base.TraverseChildren(method);    
            }
        }

        private bool IsSharpMockGenerated(IMethodDefinition method)
        {
            foreach (var customAttribute in method.Attributes)
            {
                if (customAttribute.Constructor.ResolvedMethod.Equals(reflector.From<SharpMockGeneratedAttribute>().GetConstructor(System.Type.EmptyTypes)))
                {
                    return true;
                }
            }

            return false;
        }

        public override void TraverseChildren(ISourceMethodBody sourceMethodBody)
        {
            log.WriteTrace("Traversing SourceMethodBody of {0}.", sourceMethodBody.MethodDefinition.Name.Value);
            base.TraverseChildren(sourceMethodBody);
        }

        public override void TraverseChildren(IMethodCall methodCall)
        {
            log.WriteTrace("Traversing '{0}' method call.", methodCall.MethodToCall.ResolvedMethod.Name.Value);
            base.TraverseChildren(methodCall);                
        }
    }
}