using System;

namespace SharpMock.Core.PostCompiler.Construction.Fields
{
    public interface IFieldBuilder
    {
        IFieldBuilder Named(string fieldName);
        IFieldBuilder OfType<TFieldType>();
        IFieldBuilder OfType(Type fieldType);
    }
}