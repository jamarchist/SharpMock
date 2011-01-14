using System.Collections.Generic;

namespace SharpMock.Core.DelegateTypes
{
    public static class InterceptorRegistry
    {
        private static readonly List<IInterceptor> interceptors = new List<IInterceptor>();

        public static void AddInterceptor(IInterceptor interceptor)
        {
            interceptors.Add(interceptor);
        }

        internal static IList<IInterceptor> GetInterceptors()
        {
            return interceptors;
        }

        public static void Clear()
        {
            interceptors.Clear();
        }
    }
}
