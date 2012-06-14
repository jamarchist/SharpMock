using NUnit.Framework;
using SharpMock.Core.Interception;

namespace TestUtilities
{
    public abstract class SharpMockTests
    {
        [SetUp]
        public void ClearInterceptorRegistryBefore()
        {
            InterceptorRegistry.Clear();
        }

        [TearDown]
        public void ClearInterceptorRegistryAfter()
        {
            InterceptorRegistry.Clear();
        }
    }
}
