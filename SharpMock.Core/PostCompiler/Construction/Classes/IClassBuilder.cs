using SharpMock.Core.PostCompiler.Construction.Fields;
using SharpMock.Core.PostCompiler.Construction.Methods;

namespace SharpMock.Core.PostCompiler.Construction.Classes
{
    public interface IClassBuilder
    {
        IClassBuilder Named(string className);
        IClassBuilder InNamespace(string namespaceName);
        IClassBuilder With(VoidAction<IMethodAccessibilityOptions> method);
        IClassBuilder WithField(VoidAction<IFieldAccessibilityOptions> field);
    }
}