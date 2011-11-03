namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class MethodModifierOptions : IMethodModifierOptions
    {
        private readonly MethodConfiguration config;

        public MethodModifierOptions(MethodConfiguration config)
        {
            this.config = config;
        }

        public IMethodBuilder Static
        {
            get
            {
                config.IsStatic = true;
                return new MethodBuilder(config);
            }
        }

        public IMethodBuilder Virtual
        {
            get { throw new System.NotImplementedException(); }
        }

        public IMethodBuilder Sealed
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}