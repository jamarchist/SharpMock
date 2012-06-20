namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class LazyIntercept : IInterceptionStrategy
    {
        private readonly Function<IInterceptionStrategy> strategyBinder;

        public LazyIntercept(Function<IInterceptionStrategy> strategyBinder)
        {
            this.strategyBinder = strategyBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            strategyBinder().Intercept(invocation);
        }
    }
}