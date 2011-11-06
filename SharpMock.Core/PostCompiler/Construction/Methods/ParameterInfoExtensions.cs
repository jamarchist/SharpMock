using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal static class ParameterInfoExtensions
    {
        internal static IEnumerable<IParameterDefinition> ToParameterDefinitions(this ParameterInfo[] parameterInfos, ISignature method, IMetadataHost host, IUnitReflector reflector)
        {
            var definitions = new List<IParameterDefinition>();

            foreach (var parameter in parameterInfos)
            {
                var definition = new ParameterDefinition();
                definition.ContainingSignature = method;
                definition.Index = (ushort) Array.IndexOf(parameterInfos, parameter);
                definition.Name = host.NameTable.GetNameFor(parameter.Name);
                definition.Type = reflector.Get(parameter.ParameterType);

                definitions.Add(definition);
            }

            return definitions;
        }
    }
}
