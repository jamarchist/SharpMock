using System;
using ScenarioDependencies;
using SharpMock.Core;

namespace ConstructionSamples
{
    public static class TestClass
    {
        public static DateTime TestMethod()
        {
            return new DateTime();
        }

        public static void ArrayIndexerTest()
        {
            var paramterTypes = new Type[1];
            var stringType = typeof (string);
            paramterTypes[0] = stringType;
            var isNullOrEmptyMethod = stringType.GetMethod("IsNullOrEmpty", paramterTypes);
        }

        public static void VoidActionTest()
        {
            VoidAction x = () =>
                           {
                               StaticClass.VoidReturnNoParameters();
                           };
        }

        public static void FunctionTest()
        {
            Function<bool> x = () =>
                                   {
                                       var s = StaticClass.StringReturnNoParameters();
                                       return String.IsNullOrEmpty(s);
                                   };
        }
    }

    public enum AnEnumeration
    {
        Value1,
        Value2,
        Value3
    }

    public enum AnotherEnumeration : long
    {
        Value1,
        Value2,
        Value3
    }

    public enum YetAnotherEnumeration
    {
        Value1 = 5,
        Value2 = 10,
        Value3 = 15
    }

    [Flags]
    public enum FlagsEnumeration
    {
        Value1 = 1,
        Value2 = 2,
        Value3 = 4,
        Value4 = 8
    }
}
