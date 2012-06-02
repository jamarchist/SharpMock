namespace SharpMock.Core.Interception.Registration
{
    public interface IInterceptionSpecification
    {
        void SpecifyInterceptors(ISpecificationRegistry registry);
    }
}
