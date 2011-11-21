namespace SharpMock.Core.PostCompiler.Construction.Fields
{
    internal class FieldModifierOptions : IFieldModifierOptions
    {
        private readonly FieldConfiguration config;

        public FieldModifierOptions(FieldConfiguration config)
        {
            this.config = config;
        }

        public IFieldBuilder Static
        {
            get 
            { 
                config.IsStatic = true;
                return new FieldBuilder(config);
            }
        }

        public IFieldBuilder Instance
        {
            get
            {
                config.IsStatic = false;
                return new FieldBuilder(config);
            }
        }

        public IFieldModifierOptions Readonly
        {
            get
            {
                config.IsReadonly = true;
                return this;
            }
        }
    }
}