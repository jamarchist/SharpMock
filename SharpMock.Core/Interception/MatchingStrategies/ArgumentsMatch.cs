using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class ArgumentsMatch : IMatchingStrategy
    {
        private readonly IMatchingStrategy signatureMatcher;
        private readonly IArgumentMatcher[] argumentMatchers; 

        public ArgumentsMatch(IMatchingStrategy signatureMatcher, params IArgumentMatcher[] argumentMatchers)
        {
            this.signatureMatcher = signatureMatcher;
            this.argumentMatchers = argumentMatchers;
        }

        public bool Matches(MemberInfo calledMethod, IList<object> arguments)
        {
            var signaturesMatch = signatureMatcher.Matches(calledMethod, arguments);
            if (signaturesMatch)
            {
                for (int argumentIndex = 0; argumentIndex < arguments.Count; argumentIndex++)
                {
                    if (!argumentMatchers[argumentIndex].Matches(arguments[argumentIndex]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}