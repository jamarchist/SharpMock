using System;

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
    }
}
