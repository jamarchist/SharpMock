using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public class CompileTimeConstantBuilder : ICompileTimeConstantBuilder
    {
        private readonly IUnitReflector reflector;

        public CompileTimeConstantBuilder(IUnitReflector reflector)
        {
            this.reflector = reflector;
        }

        public CompileTimeConstant Of<TConstantType>(TConstantType value)
        {
            var constant = new CompileTimeConstant();
            constant.Type = reflector.Get<TConstantType>();
            constant.Value = value;

            return constant;
        }
    }
}
