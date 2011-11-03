namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class MethodBuilder : IMethodBuilder
    {
        private readonly MethodConfiguration config;

        public MethodBuilder(MethodConfiguration config)
        {
            this.config = config;
        }

        public IMethodBuilder Named(string methodName)
        {
            config.Name = methodName;
            return this;
        }

        public IMethodBuilder WithParameters()
        {
            throw new System.NotImplementedException();
        }

        public IMethodBuilder WithBody()
        {
            throw new System.NotImplementedException();
        }

        public IMethodBuilder Returning<TReturnType>()
        {
            config.ReturnType = typeof (TReturnType);
            return this;
        }
    }
}