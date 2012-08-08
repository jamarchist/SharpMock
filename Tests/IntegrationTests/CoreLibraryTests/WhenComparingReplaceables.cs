using System.Collections.Generic;
using NUnit.Framework;
using SharpMock.Core.Interception.Registration;

namespace IntegrationTests.CoreLibraryTests
{
    [TestFixture]
    public class WhenComparingReplaceables
    {
        [Test]
        public void MethodsWithSamePropertiesShouldBeEqual()
        {
            var method1 = MakeMethod();
            var method2 = MakeMethod();

            Assert.IsTrue(method1.Equals(method2));
            Assert.IsTrue(method2.Equals(method1));
        }

        [Test]
        public void MethodsWithSamePropertiesShouldHaveSameHashCode()
        {
            var method1 = MakeMethod();
            var method2 = MakeMethod();

            Assert.AreEqual(method1.GetHashCode(), method2.GetHashCode());
        }

        private ReplaceableMethodInfo MakeMethod()
        {
            var method = new ReplaceableMethodInfo
            {
                DeclaringType = new ReplaceableTypeInfo
                {
                    Assembly = new ReplaceableAssemblyInfo
                    {
                        AssemblyPath = @"C:\temp\assembly.dll",
                        Name = "Assembly"
                    },
                    Name = "Type",
                    Namespace = "Assembly"
                },
                Name = "Method",
                Parameters = new List<ReplaceableParameterInfo>(),
                ReturnType = new ReplaceableTypeInfo
                {
                    Assembly = new ReplaceableAssemblyInfo
                    {
                        AssemblyPath = @"C:\temp\assembly.dll",
                        Name = "Assembly"
                    },
                    Name = "Type",
                    Namespace = "Assembly"
                }
            };

            return method;
        }
    }
}