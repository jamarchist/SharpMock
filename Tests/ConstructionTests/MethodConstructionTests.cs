using System.Reflection;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Assemblies;

namespace ConstructionTests
{
    [TestFixture]
    public class MethodConstructionTests : BaseConstructionTests
    {
        private void InStaticClass(VoidAction<IMethodAccessibilityOptions> createMethod)
        {
            AssemblyBuilder.CreateNewDll(with =>
            {
                with.Name(AssemblyName);
                with.Type.Class.Public.Static
                    .Named("TestClass")
                    .With(method => createMethod(method));
            });
        }

        private MethodInfo GetMethodFromTestClass(string methodName)
        {
            return GetMethodFromClass("TestClass", methodName);
        }

        [Test]
        public void CanCreateEmptyPublicStaticMethod()
        {
            InStaticClass(createMethod => createMethod.Public.Static.Named("TestMethod"));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
            Assert.IsTrue(testMethod.IsStatic);
        }
    }
}
