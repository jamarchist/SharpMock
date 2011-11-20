using Microsoft.Cci;
using NUnit.Framework;

namespace DecompilerHelper
{
    [TestFixture]
    public class Decompile
    {
        [Test]
        public void ConstructionSamples()
        {
            var host = new PeReader.DefaultHost();
            var assembly = host.LoadUnitFrom("ConstructionSamples.dll") as IAssembly;

            var mutable = Microsoft.Cci.ILToCodeModel.Decompiler.GetCodeModelFromMetadataModel(host, assembly, null);

            var codePrinter = new CodePrinter();
            codePrinter.Visit(mutable);

            Assert.Ignore("This is not a test.");
        }
    }
}
