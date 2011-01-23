using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class InstanceCreator : IInstanceCreator
    {
        private readonly IUnitReflector reflector;

        public InstanceCreator(IUnitReflector reflector)
        {
            this.reflector = reflector;
        }

        public CreateObjectInstance New<TReflectionType>()
        {
            var createObjectInstance = new CreateObjectInstance();
            var objectType = reflector.Get<TReflectionType>();
            createObjectInstance.Type = objectType.ResolvedType;
            createObjectInstance.MethodToCall = reflector.From<TReflectionType>().GetConstructor();

            return createObjectInstance;
        }
    }
}