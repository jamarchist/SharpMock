using System;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;

namespace SharpMock.Core.Syntax
{
    public class Faker : IFaker
    {
        public IFakerOptions CallsTo(VoidAction methodToRecord)
        {
            InterceptorRegistry.Record();

            methodToRecord();
            var expectations = InterceptorRegistry.GetCurrentRecorder().GetExpectations();

            var interceptor = new CompoundInterceptor(
                new EquivalentCallsMatch(expectations.Method),
                new InvokeReplacementCall(() => expectations.Replacement), new InvokeOriginalCall()
                );

            InterceptorRegistry.AddInterceptor(interceptor);

            InterceptorRegistry.StopRecording();

            return new FakerOptions(expectations);
        }

        //public void CallsTo(VoidAction method, VoidAction<IFakerOptions> by)
        //{
        //    InterceptorRegistry.Record();
        //    //
        //    System.Diagnostics.Debugger.Launch();
        //    method();
        //    var expectations = InterceptorRegistry.GetCurrentRecorder().GetExpectations();
        //    by(new FakerOptions(expectations));

        //    var interceptor = new CompoundInterceptor(
        //        new EquivalentCallsMatch(expectations.Method),
        //        new ReplaceCall(expectations.Replacement), new InvokeCall()
        //        );

        //    InterceptorRegistry.AddInterceptor(interceptor);

        //    //
        //    InterceptorRegistry.StopRecording();
        //}
    }
}