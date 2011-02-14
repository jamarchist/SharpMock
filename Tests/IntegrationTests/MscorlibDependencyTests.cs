using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;

namespace IntegrationTests
{
    [TestFixture]
    public class MscorlibDependencyTests
    {
        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void InterceptsPrimitiveParsing()
        {
            var faker = new Faker();

            faker.CallsTo(() => bool.Parse("")).ByReplacingWith((string x) => false);
            //faker.CallsTo(() => int.Parse("")).ByReplacingWith((string x) => 4);
            faker.CallsTo(() => decimal.Parse("")).ByReplacingWith((string x) => 3.14M);

            var code = new CodeWithMscorlibDependencies();
            var result = code.ParsesHardCodedStrings();

            Assert.AreEqual(false, result.FirstValue);
            //Assert.AreEqual(4, result.SecondValue);
            Assert.AreEqual(3.14M, result.ThirdValue);
        }
    }
}
