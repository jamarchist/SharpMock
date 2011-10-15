using TestUtilities;

namespace SyntaxTestsRunner
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
            var assemblyLocations = new AssemblyLocations(
                @"C:\Projects\github\SharpMock\Tests\SyntaxTests\bin\Debug\SyntaxTests.dll",
                @"C:\Projects\github\SharpMock\Tests\SyntaxTests\bin\Debug\SyntaxTests.dll");

            var runner = new SpecificationInterceptionTestRunner(assemblyLocations);
            runner.RunTests();
        }
    }
}
