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

            Assert.AreEqual("SharpMock.Core.dll", assemblyPath);            
        }

        [Test]
        public void CanGetReplaceableMethod()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var prepend = reflector.From<ReverseStringBuilder>().GetMethod("Prepend", typeof (string));

            var replaceable = prepend.AsReplaceable();

            Assert.IsNotNull(replaceable);  
        }
    }
}
