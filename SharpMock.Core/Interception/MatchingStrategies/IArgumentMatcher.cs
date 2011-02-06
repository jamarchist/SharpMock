namespace SharpMock.Core.Interception.MatchingStrategies
{
    public interface IArgumentMatcher
    {
        bool Matches(object argument);
    }
}