using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
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

            var mutable = Decompiler.GetCodeModelFromMetadataModel(host, assembly, null);

            Assert.Ignore("This is not a test.");
        }
    }
}
