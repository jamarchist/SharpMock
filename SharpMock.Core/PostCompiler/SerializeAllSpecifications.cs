using System;
using System.Collections.Generic;
using System.IO;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    public class SerializeAllSpecifications : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var specAssembly = Path.GetFileNameWithoutExtension(context.Args.TestAssemblyPath);
            var autoSpecs = String.Format("{0}.Fluent.SharpMock.SerializedSpecifications.xml", specAssembly);

            var serializer = new ReplaceableMethodInfoListSerializer(
                Path.GetDirectoryName(context.Args.TestAssemblyPath));
            serializer.SerializeSpecifications(autoSpecs, MethodReferenceReplacementRegistry.GetReplaceables());
            SerializeExplicitSpecifications(context.Args.TestAssemblyPath);
        }

        private void SerializeExplicitSpecifications(string specAssembly)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(specAssembly);
            var specs = new List<Type>(assembly.GetTypes())
                .FindAll(t => typeof(IReplacementSpecification).IsAssignableFrom(t));

            var specifiedMethods = new List<ReplaceableMethodInfo>();

            foreach (var specType in specs)
            {
                var spec = Activator.CreateInstance(specType) as IReplacementSpecification;
                specifiedMethods.AddRange(spec.GetMethodsToReplace());
            }

            var specPath = Path.GetDirectoryName(specAssembly);
            var specAssemblyName = Path.GetFileNameWithoutExtension(specAssembly);
            var serializedSpecName = String.Format("{0}.SharpMock.SerializedSpecifications.xml", specAssemblyName);

            var serializer = new ReplaceableMethodInfoListSerializer(specPath);
            serializer.SerializeSpecifications(serializedSpecName, specifiedMethods);
        }
    }
}