using System;
using System.Collections.Generic;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class MethodBuilder : IMethodBuilder
    {
        private readonly MethodConfiguration config;

        public MethodBuilder(MethodConfiguration config)
        {
            this.config = config;
        }

        public IMethodBuilder Named(string methodName)
        {
            config.Name = methodName;
            return this;
        }

        public IMethodBuilder WithParameters(Dictionary<string, Type> parameters)
        {
            foreach (var parameter in parameters) config.Parameters.Add(parameter);
            return this;
        }

        public IMethodBuilder WithBody(VoidAction<ICodeBuilder> code)
        {
            config.MethodBody = code;
            return this;
        }

        public IMethodBuilder Returning<TReturnType>()
        {
            config.ReturnType = typeof (TReturnType);
            return this;
        }
    }
}