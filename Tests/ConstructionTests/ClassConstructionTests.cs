using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Assemblies;

namespace ConstructionTests
{
    [TestFixture]
    public class ClassConstructionTests : BaseConstructionTests
    {
        private void InAssembly(VoidAction<ITypeOptions> create)
        {
            AssemblyBuilder.CreateNewDll(with =>
            {
                with.Name(AssemblyName);
                create(with.Type);
            });
        }

        [Test]
        public void CanCreateConcreteClass()
        {
            InAssembly(create => create.Class.Public.Concrete.Named("TestClass"));

            var concreteClass = GetTypeFromAssembly("TestClass");
            Assert.IsNotNull(concreteClass);
        }

        [Test]
        public void CanCreateAbstractClass()
        {
            InAssembly(create => create.Class.Public.Abstract.Named("BaseTestClass"));

            var abstractClass = GetTypeFromAssembly("BaseTestClass");
            Assert.IsNotNull(abstractClass);
            Assert.IsTrue(abstractClass.IsAbstract);
        }

        [Test]
        public void CanCreateStaticClass()
        {
            InAssembly(create => create.Class.Public.Static.Named("StaticClass"));

            var staticClass = GetTypeFromAssembly("StaticClass");
            Assert.IsNotNull(staticClass);
            Assert.IsTrue(staticClass.IsSealed);
            Assert.IsTrue(staticClass.IsAbstract);
        }
    }
}
