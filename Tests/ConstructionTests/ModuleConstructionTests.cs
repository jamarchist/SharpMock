using NUnit.Framework;

namespace ConstructionTests
{
    [TestFixture]
    public class ModuleConstructionTests : BaseConstructionTests
    {
        [Test]
        public void CanCreateAssembly()
        {
            AssemblyBuilder.CreateNewDll(with => with.Name(AssemblyName));
        }        
    }
}