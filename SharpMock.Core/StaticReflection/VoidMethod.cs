using System;
using System.Reflection;

namespace SharpMock.Core.StaticReflection
{
    public static class VoidMethod
    {
        public static MethodInfo Of(VoidAction action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T>(VoidAction<T> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2>(VoidAction<T1, T2> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3>(VoidAction<T1, T2, T3> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3, T4>(VoidAction<T1, T2, T3, T4> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5>(VoidAction<T1, T2, T3, T4, T5> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6>(VoidAction<T1, T2, T3, T4, T5, T6> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7>(VoidAction<T1, T2, T3, T4, T5, T6, T7> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            return MethodInfo(action);
        }

        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
        {
            return MethodInfo(action);
        }

        private static MethodInfo MethodInfo(Delegate action)
        {
            return action.Method;
        }
    }
}