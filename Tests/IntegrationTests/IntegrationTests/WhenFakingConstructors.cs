using System;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingConstructors : SharpMockTests
    {
        //[Test]
        //public void ReplacementIsUsed()
        //{
        //    var preConstructedClass = new ClassWithConstructor(String.Empty);
        //    Replace.CallsTo(() => new ClassWithConstructor())
        //        .With(() => preConstructedClass);

        //    var code = new CodeUnderTest();
        //    var result = code.CallsClassWithConstructor();

        //    Assert.AreSame(preConstructedClass, result);
        //}
    }
}
