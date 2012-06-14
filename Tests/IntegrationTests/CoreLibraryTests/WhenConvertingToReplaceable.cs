using Microsoft.Cci;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.Interception.Helpers;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Interception.Registration;

namespace IntegrationTests.CoreLibraryTests
{
    [TestFixture]
    public class WhenConvertingToReplaceable
    {
        [Test]
        public void ReverseStringBuilderWorks()
        {
            var builder = new ReverseStringBuilder();
            builder.Prepend("Three");
            builder.Prepend("Two");
            builder.Prepend("One");

            var reverseString = builder.ToString();
            Assert.AreEqual("OneTwoThree", reverseString);
        }

        [Test]
        public void NamespaceIsDiscovered()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get(typeof(ReverseStringBuilder));

            var resolvedNamespace = cciType.GetNamespaceType().Namespace();
            Assert.AreEqual("SharpMock.Core.Interception.Helpers", resolvedNamespace);
        }

        [Test]
        public void AssemblyPathIsDiscovered()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get(typeof(ReverseStringBuilder));

            var assemblyPath = cciType.GetNamespaceType().AssemblyPath();

            Assert.AreEqual("SharpMock.Core.dll", assemblyPath);            
        }

        [Test]
        public void FullReplaceableMethodIsConstructed()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var prepend = reflector.From<ReverseStringBuilder>().GetMethod("Prepend", typeof (string));

            var replaceable = prepend.AsReplaceable();

            Assert.IsNotNull(replaceable);  
        }

        [Test]
        public void FullReplaceableGenericTypeIsConstructed()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get<Function<string>>();

            var replaceable = cciType.AsReplaceable();

            Assert.IsNotNull(replaceable);
        }
    }
}
