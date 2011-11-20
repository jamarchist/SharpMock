using System.Diagnostics;
using TestUtilities;

namespace ExampleUsagesRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblies = new AssemblyLocations(
                @"C:\Projects\github\SharpMock\Tests\ExampleUsagesRunner\bin\Debug\ExampleUsages.dll",
                @"C:\Projects\github\SharpMock\Tests\ExampleUsagesRunner\bin\Debug\ExampleApplication.exe");

            var runner = new StaticMethodCallInterceptionTestRunner(assemblies);
            runner.RunTests();

        }
    }
}
