namespace SharpMock.Core.PostCompiler.Construction.Fields
{
    internal class FieldAccessibilityOptions : IFieldAccessibilityOptions
    {
        private readonly FieldConfiguration config;

        public FieldAccessibilityOptions(FieldConfiguration config)
        {
            this.config = config;
        }

        public IFieldModifierOptions Public
        {
            get { return Modifier(Accessibility.Public); }
        }

        public IFieldModifierOptions Private
        {
            get { return Modifier(Accessibility.Private); }
        }

        public IFieldModifierOptions Internal
        {
            get { return Modifier(Accessibility.Internal); }
        }

        public IFieldModifierOptions Protected
        {
            get { return Modifier(Accessibility.Protected); }
        }

        public IFieldModifierOptions ProtectedInternal
        {
            get { return Modifier(Accessibility.ProtectedInternal); }
        }

        private IFieldModifierOptions Modifier(Accessibility fieldAccessibility)
        {
            config.Accessibility = fieldAccessibility;
            return new FieldModifierOptions(config);
        }
    }
}
