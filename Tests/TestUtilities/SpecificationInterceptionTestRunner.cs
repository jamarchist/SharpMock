using SharpMock.PostCompiler.Core;

namespace TestUtilities
{
    public class SpecificationInterceptionTestRunner : PostCompilerTestRunner
    {
        public SpecificationInterceptionTestRunner(AssemblyLocations assemblyLocations) : base(assemblyLocations)
        {
        }

        protected override void Intercept(PostCompiler postCompiler)
        {
            postCompiler.InterceptSpecifications();
        }
    }
}