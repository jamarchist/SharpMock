using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public class ArrayIndexerOptions<TElementType> : IArrayIndexerOptions<TElementType>
    {
        private readonly ILocalVariableBindings locals;
        private readonly IUnitReflector reflector;
        private readonly string array;
        private readonly int index;

        public ArrayIndexerOptions(int index, string array, ILocalVariableBindings locals, IUnitReflector reflector)
        {
            this.index = index;
            this.locals = locals;
            this.reflector = reflector;
            this.array = array;
        }

        public IStatement Assign(IExpression expression)
        {
            var indexer = new ArrayIndexer();
            indexer.IndexedObject = locals[array];
            indexer.Indices.Add(new CompileTimeConstant { Type = reflector.Get<int>(), Value = index });
            indexer.Type = reflector.Get<TElementType>();

            var target = new TargetExpression();
            target.Definition = indexer;
            target.Instance = locals[array];
            target.Type = reflector.Get<TElementType[]>();

            var assignment = new Assignment();
            assignment.Type = reflector.Get<TElementType>();
            assignment.Source = expression;
            assignment.Target = target;

            var @do = new ExpressionStatement();
            @do.Expression = assignment;

            return @do;
        }

        public IStatement Assign(string localVariable)
        {
            return Assign(locals[localVariable]);
        }
    }
}