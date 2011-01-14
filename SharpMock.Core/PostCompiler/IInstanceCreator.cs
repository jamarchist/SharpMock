using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core
{
    public interface IInstanceCreator
    {
        CreateObjectInstance New<TReflectionType>();
    }

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
