using SharpMock.Core.Interception;

namespace SharpMock.Core.Syntax
{
    public interface IReplacementOptions
    {
        IReplacementOptions Asserting(Function<bool> callingAssertion);
        IReplacementOptions Asserting<T>(Function<T, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2>(Function<T1, T2, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2, T3>(Function<T1, T2, T3, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2, T3, T4>(Function<T1, T2, T3, T4, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2, T3, T4, T5>(Function<T1, T2, T3, T4, T5, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6>(Function<T1, T2, T3, T4, T5, T6, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6, T7>(Function<T1, T2, T3, T4, T5, T6, T7, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6, T7, T8>(Function<T1, T2, T3, T4, T5, T6, T7, T8, bool> argumentAssertion);
        IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> argumentAssertion);

        IReplacementOptions With(VoidAction replacement);
        IReplacementOptions With<T>(VoidAction<T> replacement);
        IReplacementOptions With<T1, T2>(VoidAction<T1, T2> replacement);
        IReplacementOptions With<T1, T2, T3>(VoidAction<T1, T2, T3> replacement);
        IReplacementOptions With<T1, T2, T3, T4>(VoidAction<T1, T2, T3, T4> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5>(VoidAction<T1, T2, T3, T4, T5> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6>(VoidAction<T1, T2, T3, T4, T5, T6> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7>(VoidAction<T1, T2, T3, T4, T5, T6, T7> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> replacement);

        IReplacementOptions With<TReturn>(Function<TReturn> replacement);
        IReplacementOptions With<T1, TReturn>(Function<T1, TReturn> replacement);
        IReplacementOptions With<T1, T2, TReturn>(Function<T1, T2, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, TReturn>(Function<T1, T2, T3, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, T4, TReturn>(Function<T1, T2, T3, T4, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, TReturn>(Function<T1, T2, T3, T4, T5, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, TReturn>(Function<T1, T2, T3, T4, T5, T6, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> replacement);
        IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> replacement);

        IReplacementOptions AndAllOverloads();

        IReplacementOptions With(IInterceptionStrategy interceptor);
        IReplacementOptions AsInterceptor();
    }
}