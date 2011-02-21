using System;

namespace SharpMock.Core.Syntax
{
    public interface IFaker
    {
        IFakerOptions CallsTo(VoidAction methodToRecord);
        //void CallsTo(VoidAction method, VoidAction<IFakerOptions> by);
        IFakerOptions CallsTo<TResult>(Function<TResult> propertyToRecord);
        IFakerOptions CallsTo<TInstanceType>(VoidAction<TInstanceType> instanceMethodToRecord);
    }
}
