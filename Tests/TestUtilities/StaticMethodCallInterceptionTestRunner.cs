using SharpMock.Core.PostCompiler;

namespace TestUtilities
{
    public class StaticMethodCallInterceptionTestRunner : PostCompilerTestRunner
    {
        public StaticMethodCallInterceptionTestRunner(AssemblyLocations assemblyLocations) : base(assemblyLocations)
        {
        }

        protected override void Intercept(PostCompiler postCompiler)
        {
            postCompiler.InterceptAllStaticMethodCalls();
        }
    }
}
