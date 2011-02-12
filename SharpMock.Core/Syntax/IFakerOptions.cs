namespace SharpMock.Core.Syntax
{
    public interface IFakerOptions
    {
        IFakerOptions Asserting(Function<bool> callingAssertion);
        IFakerOptions Asserting<TInterceptedArgument>(Function<TInterceptedArgument, bool> argumentAssertion);
        IFakerOptions ByReplacingWith(VoidAction replacement);
        IFakerOptions ByReplacingWith<TInterceptedArgument>(VoidAction<TInterceptedArgument> replacement);
        IFakerOptions ByReplacingWith<TReturnValue>(Function<TReturnValue> replacement);
    }
}