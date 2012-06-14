using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace TypeTranslationTests
{
    [TestFixture]
    public class DelegatesTests
    {
        [Test]
        public void CanGetGenericDelegate()
        {
            var host = new PeReader.DefaultHost();
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            host.LoadUnitFrom("SharpMock.Core.dll");

            var reflector = new UnitReflector(host);
            var cciType = reflector.Get<VoidAction<string>>();

            Assert.IsInstanceOf<IGenericTypeInstanceReference>(cciType);
        }

        //[Test]
        //public void CanTranslateSharpMockTypes()
        //{
        //    var host = new PeReader.DefaultHost();
        //    host.LoadUnit(host.CoreAssemblySymbolicIdentity);
        //    host.LoadUnitFrom("SharpMock.Core.dll");

        //    var sharpMockTypes = new SharpMockTypes(host);
        //    var reflector = new UnitReflector(host);

        //    var function = (GenericTypeInstanceReference)sharpMockTypes.Functions[1];
        //    var stringType = reflector.Get<string>();
        //    var boolType = reflector.Get<bool>();

        //    var genericArguments = new List<ITypeReference>();
        //    genericArguments.Add(stringType);
        //    genericArguments.Add(boolType);
        //    function.GenericArguments = genericArguments;
        //}
    }
}
