using Microsoft.Cci;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace IntegrationTests.TypeTranslationTests
{
    [TestFixture]
    public class WhenTranslatingTypes
    {
        [Test]
        public void GenericDelegatesAreHandled()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get<VoidAction<string>>();

            Assert.IsInstanceOf<IGenericTypeInstanceReference>(cciType);
        }

        [Test]
        public void OpenGenericTypesAreHandled()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get(typeof(VoidAction<>));

            Assert.IsInstanceOf<INamespaceTypeReference>(cciType);
        }
    }
}
