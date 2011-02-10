using System;
using SharpMock.Core.Interception;

namespace SharpMock.Core.Syntax
{
    public class Faker : IFaker
    {
        public void CallsTo(VoidAction methodToRecord)
        {
            InterceptorRegistry.Record();
            methodToRecord();
            InterceptorRegistry.StopRecording();
        }
    }
}