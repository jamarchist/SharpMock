using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace TypeTranslationTests
{
    [TestFixture]
    public class GenericsTests
    {
        [Test]
        public void CanGetOpenGenericType()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get(typeof (VoidAction<>));

            Assert.IsInstanceOf<INamespaceTypeReference>(cciType);
        }
    }
}
