using System;
using SharpMock.Core.Interception;

namespace SharpMock.Core.Syntax
{
    public class Faker : IFaker
    {
        public IFakerOptions CallsTo(VoidAction methodToRecord)
        {
            InterceptorRegistry.Record();

            methodToRecord();
            var expectations = InterceptorRegistry.GetCurrentRecorder().GetExpectations();

            InterceptorRegistry.StopRecording();

            return new FakerOptions(expectations);
        }
    }
}