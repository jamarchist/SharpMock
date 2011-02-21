namespace SharpMock.Core.Syntax
{
    public interface IFakerOptions
    {
        IFakerOptions Asserting(Function<bool> callingAssertion);
        IFakerOptions Asserting<TInterceptedArgument>(Function<TInterceptedArgument, bool> argumentAssertion);

        IFakerOptions ByReplacingWith(VoidAction replacement);
        IFakerOptions ByReplacingWith<TInterceptedArgument>(VoidAction<TInterceptedArgument> replacement);
        IFakerOptions ByReplacingWith<TArg1, TArg2>(VoidAction<TArg1, TArg2> replacement);

        IFakerOptions ByReplacingWith<TReturnValue>(Function<TReturnValue> replacement);
        IFakerOptions ByReplacingWith<TInterceptedArgument, TReturnValue>(Function<TInterceptedArgument, TReturnValue> replacement);
        IFakerOptions ByReplacingWith<TArg1, TArg2, TReturnValue>(Function<TArg1, TArg2, TReturnValue> replacement);
    }
}