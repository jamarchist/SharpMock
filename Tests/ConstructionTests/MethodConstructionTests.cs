using System.Reflection;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Methods;

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
        public void CanCreateStaticMethod()
        {
            InStaticClass(createMethod => createMethod.Public.Static.Named("TestMethod"));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
            Assert.IsTrue(testMethod.IsStatic);
        }

        [Test]
        public void CanCreateMethodThatReturnsVoid()
        {
            InStaticClass(createMethod => createMethod.Public.Static.Named("TestMethod"));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
            Assert.AreEqual(typeof(void), testMethod.ReturnType);
        }

        [Test]
        public void CanCreateEmptyStaticMethodReturningString()
        {
            InStaticClass(createMethod => createMethod.Public.Static
                .Named("TestMethod")
                .Returning<string>()
                .WithBody(code =>
                              {
                                  code.AddLine( x => x.Declare.Variable<string>("returnValue").As(x.Constant.Of("something")) );
                                  code.AddLine( x => x.Return.Variable(x.Locals["returnValue"]) );
                              }));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
            Assert.AreEqual(typeof(string), testMethod.ReturnType);
        }
    }
}
