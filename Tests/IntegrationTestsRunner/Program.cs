using System;
using System.Diagnostics;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.PostCompiler.Core;

namespace MethodInterceptionTestsRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            //  1) Build for this depends on the following:
            //      a) Build of SharpMock.Core
            //      b) Build of SharpMock.PostCompiler
            //      c) Build of ScenarioDependencies
            //      d) Build of Scenarios
            //      e) Build of MethodInterceptionTests

            //  2) Run SharpMock.PostCompiler.exe from build directory against compiled dll for tests
            const string testAssemblyLocation = @"C:\Projects\SharpMock\Tests\IntegrationTests\bin\Debug\IntegrationTests.dll";
            const string assemblyToModifyLocation = @"C:\Projects\SharpMock\Tests\IntegrationTests\bin\Debug\Scenarios.dll";

            var postCompilerArgs = new PostCompilerArgs(new[] { testAssemblyLocation, assemblyToModifyLocation });
            var postCompiler = new PostCompiler(postCompilerArgs);
            postCompiler.InterceptSpecifications();
            MethodReferenceReplacementRegistry.Clear();
            postCompiler.InterceptAllStaticMethodCalls();

            //  3) Run tests (against modified dll)
            const string nunitConsoleLocation = @"C:\Projects\SharpMock\packages\NUnit.2.5.7.10213\Tools\nunit-console.exe";
            var nunitConsole = new Process();
            var nunitArgs = new ProcessStartInfo(nunitConsoleLocation, WrapInQuotes(testAssemblyLocation) + " /wait");
            nunitConsole.StartInfo = nunitArgs;
            nunitConsole.Start();
            nunitConsole.WaitForExit();
        }

        private static string WrapInQuotes(string path)
        {
            return String.Format("\"{0}\"", path);
        }
    }
}
