using System;
using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class MethodCallModel
    {
        public string TargetName { get; set; }
        public string MethodName { get; set; }
        public List<IExpression> Arguments { get; set; }
        public Type[] ArgumentTypes { get; set; }
        public ITypeReference ReturnType { get; set; }
        public bool IsStatic { get; set; }
        public bool IsVirtual { get; set; }
        public IMethodReference Reference { get; set; }
        public bool IsSetter { get; set; }
        public bool IsGetter { get; set; }
    }
}