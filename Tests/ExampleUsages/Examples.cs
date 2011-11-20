using System;
using System.Diagnostics;
using ExampleApplication;
using NUnit.Framework;
using SharpMock.Core.Syntax;

namespace ExampleUsages
{
    [TestFixture]
    public class Examples
    {
        [Test]
        public void CanInstrumentAssembly()
        {
            var intercept = new Faker();
            intercept.CallsTo(() => Dao.Insert(null)).ByReplacingWith(
                (Model m) =>
                    {
                        Console.WriteLine("Stopping insert.");
                        return 99;
                    });

            Program.Main(new string[] { "Hello" });

            var startInfo = new ProcessStartInfo("ExampleApplication.exe", "Boo!");
            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            Assert.Fail("Instrumentation ability has not been implemented yet.");
        }
    }
}
