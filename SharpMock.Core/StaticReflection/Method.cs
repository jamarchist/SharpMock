using System;
using System.Reflection;

namespace SharpMock.Core.StaticReflection
{
    public static class Method
    {
        public static MethodInfo Of<T>(Function<T> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2>(Function<T1, T2> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3>(Function<T1, T2, T3> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4>(Function<T1, T2, T3, T4> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5>(Function<T1, T2, T3, T4, T5> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6>(Function<T1, T2, T3, T4, T5, T6> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7>(Function<T1, T2, T3, T4, T5, T6, T7> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8>(Function<T1, T2, T3, T4, T5, T6, T7, T8> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> function)
        {
            return MethodInfo(function);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> function)
        {
            return MethodInfo(function);
        }

        private static MethodInfo MethodInfo(Delegate function)
        {
            return function.Method;
        }
    }
}
