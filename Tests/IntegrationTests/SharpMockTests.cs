using NUnit.Framework;
using SharpMock.Core.Interception;

namespace IntegrationTests
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
