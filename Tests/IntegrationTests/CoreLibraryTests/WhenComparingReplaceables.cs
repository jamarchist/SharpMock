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

        [Test]
        public void DictionaryShouldOnlyAddMethodsWithSamePropertiesOnce()
        {
            var method1 = MakeMethod();
            var method2 = MakeMethod();

            var dictionary = new Dictionary<IReplaceableReference, object>();
            if (!dictionary.ContainsKey(method1))
            {
                dictionary.Add(method1, new object());
            }

            if (!dictionary.ContainsKey(method2))
            {
                dictionary.Add(method2, new object());
            }

            Assert.AreEqual(1, dictionary.Count);
        }

        [Test]
        public void ParameterNameShouldNotAffectEqualityCheck()
        {
            var method1 = MakeMethod();
            var method2 = MakeMethod();

            method1.Parameters[0].Name = "pDifferent";

            Assert.IsTrue(method1.Equals(method2));
        }

        [Test]
        public void ParameterNameShouldNotAffectHashCodeValue()
        {
            var method1 = MakeMethod();
            var method2 = MakeMethod();

            method1.Parameters[0].Name = "pDifferent";

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
                Parameters = new List<ReplaceableParameterInfo>
                                {
                                    new ReplaceableParameterInfo
                                    {
                                        Index = 0,
                                        Name = "parameter",
                                        ParameterType = new ReplaceableTypeInfo
                                        {
                                            Name = "typeX",
                                            Namespace = "assemblyX.namespaceX",
                                            Assembly = new ReplaceableAssemblyInfo
                                            {
                                                AssemblyPath = @"C:\temp\assemblyX.dll",
                                                Name = "assemblyX"
                                            }
                                        }
                                    }
                                },
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