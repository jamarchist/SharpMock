using System;
using SharpMock.Core.Interception.Interceptors;

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
            currentExpectations.Assertion = callingAssertion;
            return this;
        }

        public IFakerOptions Asserting<TInterceptedArgument>(Function<TInterceptedArgument, bool> argumentAssertion)
        {
            currentExpectations.Assertion = argumentAssertion;
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

        public IFakerOptions ByReplacingWith<TReturnValue>(Function<TReturnValue> replacement)
        {
            currentExpectations.Replacement = replacement;
            return this;
        }
    }
}