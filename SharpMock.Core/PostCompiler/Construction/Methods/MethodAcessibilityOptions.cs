namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class MethodAcessibilityOptions : IMethodAccessibilityOptions
    {
        private readonly MethodConfiguration config;

        public MethodAcessibilityOptions(MethodConfiguration config)
        {
            this.config = config;
        }

        public IMethodModifierOptions Public
        {
            get
            {
                config.Modifier = "Public";
                return new MethodModifierOptions(config);
            }
        }

        public IMethodModifierOptions Private
        {
            get { throw new System.NotImplementedException(); }
        }

        public IMethodModifierOptions Internal
        {
            get { throw new System.NotImplementedException(); }
        }

        public IMethodModifierOptions Protected
        {
            get { throw new System.NotImplementedException(); }
        }

        public IMethodModifierOptions ProtectedInternal
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}