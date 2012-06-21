using System;
using System.Reflection;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;

namespace SharpMock.Core.Syntax
{
    public class ReplacementOptions : IReplacementOptions
    {
        private readonly Expectations currentExpectations;

        public ReplacementOptions(Expectations currentExpectations)
        {
            this.currentExpectations = currentExpectations;
        }

        public IReplacementOptions Asserting(Function<bool> callingAssertion) { return AssertionOptions(callingAssertion); }
        public IReplacementOptions Asserting<T>(Function<T, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2>(Function<T1, T2, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2, T3>(Function<T1, T2, T3, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2, T3, T4>(Function<T1, T2, T3, T4, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2, T3, T4, T5>(Function<T1, T2, T3, T4, T5, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6>(Function<T1, T2, T3, T4, T5, T6, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6, T7>(Function<T1, T2, T3, T4, T5, T6, T7, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6, T7, T8>(Function<T1, T2, T3, T4, T5, T6, T7, T8, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }
        public IReplacementOptions Asserting<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> argumentAssertion) { return AssertionOptions(argumentAssertion); }

        public IReplacementOptions With(VoidAction replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T>(VoidAction<T> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2>(VoidAction<T1, T2> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3>(VoidAction<T1, T2, T3> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4>(VoidAction<T1, T2, T3, T4> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5>(VoidAction<T1, T2, T3, T4, T5> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6>(VoidAction<T1, T2, T3, T4, T5, T6> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7>(VoidAction<T1, T2, T3, T4, T5, T6, T7> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> replacement) { return WithOptions(replacement); }

        public IReplacementOptions With<TReturn>(Function<TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, TReturn>(Function<T1, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, TReturn>(Function<T1, T2, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, TReturn>(Function<T1, T2, T3, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, TReturn>(Function<T1, T2, T3, T4, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, TReturn>(Function<T1, T2, T3, T4, T5, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, TReturn>(Function<T1, T2, T3, T4, T5, T6, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> replacement) { return WithOptions(replacement); }
        public IReplacementOptions With<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> replacement) { return WithOptions(replacement); }

        public IReplacementOptions MatchingWith(IMatchingStrategy matcher)
        {
            currentExpectations.MatchingStrategy = matcher;
            return this;
        }

        public IReplacementOptions MatchingWith(Function<MethodInfo, IMatchingStrategy> matcherBinder)
        {
            currentExpectations.MatchingStrategy = matcherBinder(currentExpectations.Method);
            return this;
        }

        public IReplacementOptions AndAllOverloads()
        {
            currentExpectations.MatchingStrategy = new AllOverloadsMatch(currentExpectations.Method);
            return this;
        }

        public IReplacementOptions AsInterceptor()
        {
            currentExpectations.Invoker = new InvokeWithInvocation(() => currentExpectations.Replacement);
            return this;
        }

        public IReplacementOptions With(IInterceptionStrategy interceptor)
        {
            currentExpectations.Invoker = interceptor;
            return this;
        }

        private IReplacementOptions AssertionOptions(Delegate assertion)
        {
            currentExpectations.Assertions.Add(assertion);
            return this;
        }

        private IReplacementOptions WithOptions(Delegate replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }
    }
}