using System;

namespace SharpMock.Core.PostCompiler.Construction.Fields
{
    internal class FieldBuilder : IFieldBuilder
    {
        private readonly FieldConfiguration config;

        public FieldBuilder(FieldConfiguration config)
        {
            this.config = config;
        }

        public IFieldBuilder Named(string fieldName)
        {
            config.Name = fieldName;
            return this;
        }

        public IFieldBuilder OfType<TFieldType>()
        {
            config.FieldType = typeof(TFieldType);
            return this;
        }

        public IFieldBuilder OfType(Type fieldType)
        {
            config.FieldType = fieldType;
            return this;
        }
    }
}
