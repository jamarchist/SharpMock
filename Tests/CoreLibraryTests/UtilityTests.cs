using System.Reflection;
using Microsoft.Cci;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.Interception.Helpers;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.Interception.Registration;

namespace CoreLibraryTests
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void CanBuildReverseString()
        {
            var builder = new ReverseStringBuilder();
            builder.Prepend("Three");
            builder.Prepend("Two");
            builder.Prepend("One");

            var reverseString = builder.ToString();
            Assert.AreEqual("OneTwoThree", reverseString);
        }

        [Test]
        public void CanGetNamespaceFromTypeReference()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get(typeof(ReverseStringBuilder));

            var resolvedNamespace = cciType.Namespace();
            Assert.AreEqual("SharpMock.Core.Interception.Helpers", resolvedNamespace);
        }

        [Test]
        public void CanGetAssemblyPathFromTypeReference()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get(typeof(ReverseStringBuilder));

            var assemblyPath = cciType.AssemblyPath();

            var executingDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var typeReferenceAssemblyPath = System.IO.Path.Combine(executingDirectory, "SharpMock.Core.dll");
            Assert.AreEqual(typeReferenceAssemblyPath, assemblyPath);            
        }
    }
}
