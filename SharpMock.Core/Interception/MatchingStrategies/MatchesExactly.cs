namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class MatchesExactly : IArgumentMatcher
    {
        private readonly object matchingObject;

        public MatchesExactly(object matchingObject)
        {
            this.matchingObject = matchingObject;
        }

        public bool Matches(object argument)
        {
            return argument.Equals(matchingObject);
        }
    }
}