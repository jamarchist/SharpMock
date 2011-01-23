using SharpMock.Core.DelegateTypes;

namespace SharpMock.Core.Syntax
{
    public interface IFaker
    {
        void CallsTo(VoidAction methodToRecord);
    }
}
