using System;
using System.Collections.Generic;
using System.IO;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler
{
    public class SerializeAllSpecifications : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var specAssembly = Path.GetFileNameWithoutExtension(context.Args.TestAssemblyPath);
            var autoSpecs = String.Format("{0}.Fluent.SharpMock.SerializedSpecifications.xml", specAssembly);

            var serializer = new ReplaceableCodeInfoSerializer(
                Path.GetDirectoryName(context.Args.TestAssemblyPath));

            var replaceableAssignments = context.Registry.GetRegisteredReferences(ReplaceableReferenceTypes.FieldAssignment);
            var replaceableAccessors = context.Registry.GetRegisteredReferences(ReplaceableReferenceTypes.FieldAccessor);
            var replaceableMethods = context.Registry.GetRegisteredReferences(ReplaceableReferenceTypes.Method);

            var replaceableCode = new ReplaceableCodeInfo();
            replaceableCode.Methods = replaceableMethods.As<ReplaceableMethodInfo>();
            replaceableCode.FieldAccessors = replaceableAccessors.As<ReplaceableFieldInfo>();
            replaceableCode.FieldAssignments = replaceableAssignments.As<ReplaceableFieldInfo>();

            serializer.SerializeSpecifications(autoSpecs, replaceableCode);
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

            var replaceableCode = new ReplaceableCodeInfo();
            replaceableCode.Methods = specifiedMethods;

            var serializer = new ReplaceableCodeInfoSerializer(specPath);
            serializer.SerializeSpecifications(serializedSpecName, replaceableCode);
        }
    }
}