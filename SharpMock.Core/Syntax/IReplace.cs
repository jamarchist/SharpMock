using System;

namespace SharpMock.Core.Syntax
{
    public interface IReplace
    {
        IReplacementOptions CallsTo(VoidAction methodToRecord);
        //void CallsTo(VoidAction method, VoidAction<IReplacementOptions> by);
        IReplacementOptions CallsTo<TResult>(Function<TResult> propertyToRecord);
        IReplacementOptions CallsTo<TInstanceType>(VoidAction<TInstanceType> instanceMethodToRecord);
    }
}
