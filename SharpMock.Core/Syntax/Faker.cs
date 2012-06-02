using System;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Syntax
{
    public class Faker : IFaker
    {
        public IFakerOptions CallsTo(VoidAction methodToRecord)
        {
            return RecordCallsTo(methodToRecord);
        }

        public IFakerOptions CallsTo<TResult>(Function<TResult> propertyToRecord)
        {
            return RecordCallsTo(propertyToRecord);
        }

        public IFakerOptions CallsTo<TInstanceType>(VoidAction<TInstanceType> instanceMethodToRecord)
        {
            return RecordCallsTo(instanceMethodToRecord);
        }

        private static IFakerOptions RecordCallsTo(Delegate method)
        {
            InterceptorRegistry.Record();

            var arguments = method.FakeInvocationArguments();
            method.DynamicInvoke(arguments);

            var expectations = InterceptorRegistry.GetCurrentRecorder().GetExpectations();

            var interceptor = new CompoundInterceptor(
                new EquivalentCallsMatch(expectations.Method),
                new Assert(() => expectations.Assertions),
                new InsteadOfCall(() => expectations.Replacement)
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
        //        new ReplaceCall(expectations.Replacement), new InsteadOfCall()
        //        );

        //    InterceptorRegistry.AddInterceptor(interceptor);

        //    //
        //    InterceptorRegistry.StopRecording();
        //}
    }
}