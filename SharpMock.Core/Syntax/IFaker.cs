using System;

namespace SharpMock.Core.Syntax
{
    public interface IFaker
    {
        IFakerOptions CallsTo(VoidAction methodToRecord);
    }
}
