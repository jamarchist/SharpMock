namespace SharpMock.Core.PostCompiler.Construction.Classes
{
    internal class ClassAccessibilityOptions : IClassAccessiblityOptions
    {
        private readonly ClassConfiguration config;

        public ClassAccessibilityOptions(ClassConfiguration config)
        {
            this.config = config;
        }

        public IClassModifierOptions Public
        {
            get
            {
                config.Modifier = "Public";
                return Options();
            }
        }

        public IClassModifierOptions Private
        {
            get
            {
                config.Modifier = "Private";
                return Options();
            }
        }

        public IClassModifierOptions Internal
        {
            get
            {
                config.Modifier = "Internal";
                return Options();
            }
        }

        private IClassModifierOptions Options()
        {
            return new ClassModifierOptions(config);
        }
    }
}