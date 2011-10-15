using TestUtilities;

namespace IntegrationTestsRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblies = new AssemblyLocations(
                @"C:\Projects\github\SharpMock\Tests\IntegrationTests\bin\Debug\IntegrationTests.dll", 
                @"C:\Projects\github\SharpMock\Tests\IntegrationTests\bin\Debug\Scenarios.dll");

            var runner = new StaticMethodCallInterceptionTestRunner(assemblies);
            runner.RunTests();
        }
    }
}
