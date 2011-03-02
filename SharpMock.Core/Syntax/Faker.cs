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
            return RecordCallsTo(methodToRecord);
        }

        public IFakerOptions CallsTo<TResult>(Function<TResult> propertyToRecord)
        {
            return RecordCallsTo(propertyToRecord);
        }

        public IFakerOptions CallsTo<TInstanceType>(VoidAction<TInstanceType> instanceMethodToRecord)
        {
            var sealedType = typeof (TInstanceType);
            object untypedArg = null;

            untypedArg = sealedType.IsValueType ? Activator.CreateInstance(sealedType) : null;

            TInstanceType cheater = (TInstanceType) untypedArg;
            return RecordCallsTo(instanceMethodToRecord, cheater);
        }

        private static IFakerOptions RecordCallsTo<TInstanceType>(Delegate method, TInstanceType defaultArgument)
        {
            InterceptorRegistry.Record();

            //System.Diagnostics.Debugger.Launch();
            //var arguments = new object[] {defaultArgument};

            //var voidActionOfSealedType = (VoidAction<TInstanceType>)method;
            //voidActionOfSealedType(defaultArgument);

            //method.DynamicInvoke(arguments);
            //method.DynamicInvoke(null);

            method.DynamicInvoke(new object[] { defaultArgument });

            var expectations = InterceptorRegistry.GetCurrentRecorder().GetExpectations();

            var interceptor = new CompoundInterceptor(
                new EquivalentCallsMatch(expectations.Method),
                new Assert(() => expectations.Assertions),
                new InvokeCall(() => expectations.Replacement)
            );

            InterceptorRegistry.AddInterceptor(interceptor);

            InterceptorRegistry.StopRecording();

            return new FakerOptions(expectations);
        }

        private static IFakerOptions RecordCallsTo(Delegate method)
        {
            InterceptorRegistry.Record();

            //System.Diagnostics.Debugger.Launch();

            var parameters = method.Method.GetParameters();
            if (parameters.Length == 0)
            {
                method.DynamicInvoke(null);
            }
            else
            {
                var firstParameter = parameters[0];
                if (firstParameter.ParameterType.IsValueType)
                {
                    method.DynamicInvoke(Activator.CreateInstance(firstParameter.ParameterType));
                }
                else
                {
                    method.DynamicInvoke(new object[]{ null });
                }
            }

            //method.DynamicInvoke(null);
            var expectations = InterceptorRegistry.GetCurrentRecorder().GetExpectations();

            var interceptor = new CompoundInterceptor(
                new EquivalentCallsMatch(expectations.Method),
                new Assert(() => expectations.Assertions),
                new InvokeCall(() => expectations.Replacement)
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