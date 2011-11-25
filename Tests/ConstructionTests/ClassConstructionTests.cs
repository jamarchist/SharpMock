using System;
using Microsoft.Cci;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Assemblies;

namespace ConstructionTests
{
    [TestFixture]
    public class ClassConstructionTests : BaseConstructionTests
    {
        private IModule Assembly { get; set; }

        private void InAssembly(VoidAction<ITypeOptions> create)
        {
            Assembly = AssemblyBuilder.CreateNewDll(with =>
            {
                with.Name(AssemblyName);
                create(with.Type);
            });
        }

        private void InAssembly(VoidAction<IAssemblyConstructionOptions> create)
        {
            Assembly = AssemblyBuilder.CreateNewDll(with =>
            {
                with.Name(AssemblyName);
                create(with);
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

        [Test]
        public void CanCreateClassWithField()
        {
            InAssembly(create => create
                .Class.Public.Concrete.Named("ClassWithField")
                    .WithField(createField => 
                        createField.Public.Instance.Named("helloField").OfType<string>()));

            var field = GetFieldFromClass("ClassWithField", "helloField");
            Assert.IsNotNull(field);
            Assert.AreEqual(typeof(string), field.FieldType);
            Assert.IsTrue(field.IsPublic);
            Assert.IsFalse(field.IsStatic);
        }

        [Test]
        public void CanCreateClassInNamespace()
        {
            InAssembly(create => create.Class.Public.Concrete.Named("ClassInNamespace")
                .InNamespace("A"));

            var @class = GetTypeFromAssembly("A.ClassInNamespace");
            Assert.IsNotNull(@class);
            Assert.AreEqual(String.Format("{0}.{1}", AssemblyName, "A"), @class.Namespace);
        }

        [Test]
        public void CanCreateClassInNestedNamespace()
        {
            InAssembly(create => create.Class.Public.Concrete.Named("ClassInNestedNamespace")
                .InNamespace("A.B"));

            var @class = GetTypeFromAssembly("A.B.ClassInNestedNamespace");
            Assert.IsNotNull(@class);
            Assert.AreEqual(String.Format("{0}.{1}", AssemblyName, "A.B"), @class.Namespace);
        }

        [Test]
        public void CanCreateMultipleClassesInNamespace()
        {
            InAssembly(create =>
            {
                create.Type.Class.Public.Concrete.Named("ClassA").InNamespace("Z");
                create.Type.Class.Public.Concrete.Named("ClassB").InNamespace("Z");
            });

            var classA = GetTypeFromAssembly("Z.ClassA");
            var classB = GetTypeFromAssembly("Z.ClassB");

            Assert.IsNotNull(classA);
            Assert.IsNotNull(classB);

            var namespaceName = String.Format("{0}.{1}", AssemblyName, "Z");
            Assert.AreEqual(namespaceName, classA.Namespace);
            Assert.AreEqual(namespaceName, classB.Namespace);
        }

        [Test]
        public void CanCreateMultipleClasses()
        {
            InAssembly(create =>
            {
                create.Type.Class.Public.Concrete.Named("ClassA");
                create.Type.Class.Public.Concrete.Named("ClassB");
            });

            var classA = GetTypeFromAssembly("ClassA");
            var classB = GetTypeFromAssembly("ClassB");

            Assert.IsNotNull(classA);
            Assert.IsNotNull(classB);
        }
    }
}
