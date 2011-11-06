using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public class ArrayVariableOptions<TElementType> : IArrayVariableOptions<TElementType>
    {
        private readonly string array;
        private readonly ILocalVariableBindings locals;
        private readonly IUnitReflector reflector;

        public ArrayVariableOptions(string array, ILocalVariableBindings locals, IUnitReflector reflector)
        {
            this.array = array;
            this.locals = locals;
            this.reflector = reflector;
        }

        public IArrayIndexerOptions<TElementType> this[int index]
        {
            get { return new ArrayIndexerOptions<TElementType>(index, array, locals, reflector); }
        }
    }
}