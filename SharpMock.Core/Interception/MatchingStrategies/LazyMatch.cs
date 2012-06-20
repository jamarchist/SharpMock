using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class LazyMatch : IMatchingStrategy
    {
        private readonly Function<IMatchingStrategy> strategyBinder;

        public LazyMatch(Function<IMatchingStrategy> strategyBinder)
        {
            this.strategyBinder = strategyBinder;
        }

        public bool Matches(MethodInfo calledMethod, IList<object> arguments)
        {
            var strategy = strategyBinder();
            return strategy.Matches(calledMethod, arguments);
        }
    }
}