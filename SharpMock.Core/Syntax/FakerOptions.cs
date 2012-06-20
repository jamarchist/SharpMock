using System;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;

namespace SharpMock.Core.Syntax
{
    public class FakerOptions : IFakerOptions
    {
        private readonly Expectations currentExpectations;

        public FakerOptions(Expectations currentExpectations)
        {
            this.currentExpectations = currentExpectations;
        }

        public IFakerOptions Asserting(Function<bool> callingAssertion)
        {
            AddAssertion(callingAssertion);
            return this;
        }

        public IFakerOptions Asserting<TInterceptedArgument>(Function<TInterceptedArgument, bool> argumentAssertion)
        {
            AddAssertion(argumentAssertion);
            return this;
        }

        public IFakerOptions ByReplacingWith(VoidAction replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }

        public IFakerOptions ByReplacingWith<TInterceptedArgument>(VoidAction<TInterceptedArgument> replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }

        public IFakerOptions ByReplacingWith<TArg1, TArg2>(VoidAction<TArg1, TArg2> replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }

        public IFakerOptions ByReplacingWith<TReturnValue>(Function<TReturnValue> replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }

        public IFakerOptions ByReplacingWith<TInterceptedArgument, TReturnValue>(Function<TInterceptedArgument, TReturnValue> replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }

        public IFakerOptions ByReplacingWith<TArg1, TArg2, TReturnValue>(Function<TArg1, TArg2, TReturnValue> replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }

        public IFakerOptions AndAllMatchingCalls()
        {
            currentExpectations.MatchingStrategy = new AllOverloadsMatch(currentExpectations.Method);
            return this;
        }

        private void AddAssertion(Delegate newAssertion)
        {
            currentExpectations.Assertions.Add(newAssertion);
        }
    }
}