namespace SharpMock.Core.Interception.Registration
{
    public interface ISpecificationRegistry
    {
        void AddInterceptor(IInterceptor interceptor);
    }
}