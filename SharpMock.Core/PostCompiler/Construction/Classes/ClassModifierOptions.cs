namespace SharpMock.Core.PostCompiler.Construction.Classes
{
    internal class ClassModifierOptions : IClassModifierOptions
    {
        private readonly ClassConfiguration config;

        public ClassModifierOptions(ClassConfiguration config)
        {
            this.config = config;
        }

        public IClassBuilder Static
        {
            get
            {
                config.IsStatic = true;
                return Options();
            }
        }

        public IClassBuilder Abstract
        {
            get
            {
                config.IsAbstract = true;
                return Options();
            }
        }

        public IClassBuilder Concrete
        {
            get { return Options(); }
        }

        private IClassBuilder Options()
        {
            return new ClassBuilder(config);
        }
    }
}