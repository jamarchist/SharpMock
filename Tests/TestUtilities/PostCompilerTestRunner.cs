using System;
using System.Diagnostics;
using SharpMock.Core.PostCompiler;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.PostCompiler.Core;

namespace TestUtilities
{
    public abstract class PostCompilerTestRunner
    {
        private readonly AssemblyLocations assemblyLocations;

        protected PostCompilerTestRunner(AssemblyLocations assemblyLocations)
        {
            this.assemblyLocations = assemblyLocations;
        }

        public void RunTests()
        {
            //  1) Build for this depends on the following:
            //      a) Build of SharpMock.Core
            //      b) Build of SharpMock.PostCompiler
            //      c) Build of ScenarioDependencies
            //      d) Build of Scenarios
            //      e) Build of MethodInterceptionTests

            //  2) Run SharpMock.PostCompiler.exe from build directory against compiled dll for tests
            var postCompilerArgs = new PostCompilerArgs(new[] { assemblyLocations.TestAssemblyPath, assemblyLocations.TargetAssemblyPath });
            var postCompiler = new PostCompiler(postCompilerArgs);
            postCompiler.InterceptSpecifications();
            MethodReferenceReplacementRegistry.Clear();
            Intercept(postCompiler);

            //  3) Run tests (against modified dll)
            var nunitConsole = new Process();
            var nunitArgs = new ProcessStartInfo(assemblyLocations.NUnitConsoleRunnerPath, WrapInQuotes(assemblyLocations.TestAssemblyPath) + " /wait");
            nunitConsole.StartInfo = nunitArgs;
            nunitConsole.Start();
            nunitConsole.WaitForExit();
        }

        protected abstract void Intercept(PostCompiler postCompiler);

        private static string WrapInQuotes(string path)
        {
            return String.Format("\"{0}\"", path);
        }
    }
}
