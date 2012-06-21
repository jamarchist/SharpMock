using System;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Syntax
{
    public static class Replace //: IReplace
    {
        public static IReplacementOptions CallsTo(VoidAction methodToRecord)
        {
            return RecordCallsTo(methodToRecord);
        }

        public static IReplacementOptions CallsTo<TResult>(Function<TResult> propertyToRecord)
        {
            return RecordCallsTo(propertyToRecord);
        }

        public static IReplacementOptions CallsTo<TInstanceType>(VoidAction<TInstanceType> instanceMethodToRecord)
        {
            return RecordCallsTo(instanceMethodToRecord);
        }

        private static IReplacementOptions RecordCallsTo(Delegate method)
        {
            InterceptorRegistry.Record();

            var arguments = method.FakeInvocationArguments();
            method.DynamicInvoke(arguments);

            var expectations = InterceptorRegistry.GetCurrentRecorder().GetExpectations();

            var interceptor = new CompoundInterceptor(
                new LazyMatch(() => expectations.MatchingStrategy),
                new Assert(() => expectations.Assertions),
                new LazyIntercept(() => expectations.Invoker)
            );

            InterceptorRegistry.AddInterceptor(interceptor);

            InterceptorRegistry.StopRecording();

            return new ReplacementOptions(expectations);
        }
    }
}