namespace SharpMock.Core.Interception.Registration
{
    public class SpecificationRegistry : ISpecificationRegistry
    {
        public void AddInterceptor(IInterceptor interceptor)
        {
            InterceptorRegistry.AddInterceptor(interceptor);
        }
    }
}