using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Syntax;

namespace IntegrationTests
{
    [TestFixture]
    public class InterfaceTests
    {
        [Test]
        public void CanInterceptInterfaceCalls()
        {
            var wasCalled = false;

            var fake = new Faker();
            fake.CallsTo<ISomeInterface>(i => i.DoSomething()).ByReplacingWith(() => wasCalled = true);

            var code = new CodeUnderTest();
            code.CallsSomeInterface(null);

            Assert.IsTrue(wasCalled);
        }

        //[Test]
        //public void CanInterceptConstructorCalls()
        //{
        //    var wasCalled = false;
        //    var instance = new SomeConcreteClass();

        //    var fake = new Faker();
        //    fake.CallsTo(() => new SomeConcreteClass()).ByReplacingWith(() => instance);

        //    var code = new CodeUnderTest();
        //    var result = code.CallsConstructor();

        //    Assert.AreSame(instance, result);
        //}
    }
}
