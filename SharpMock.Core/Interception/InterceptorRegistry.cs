using System.Collections.Generic;
using SharpMock.Core.Interception.Interceptors;

namespace SharpMock.Core.Interception
{
    public static class InterceptorRegistry
    {
        private static readonly List<IInterceptor> interceptors = new List<IInterceptor>();
        private static bool isRecording = false;
        private static RecordingInterceptor recorder;

        public static void AddInterceptor(IInterceptor interceptor)
        {
            interceptors.Add(interceptor);
        }

        internal static IList<IInterceptor> GetInterceptors()
        {
            if (isRecording)
            {
                recorder = new RecordingInterceptor();
                return new List<IInterceptor>{ recorder };
            }

            return interceptors;
        }

        public static void Clear()
        {
            interceptors.Clear();
        }

        public static void Record()
        {
            isRecording = true;
        }

        public static void StopRecording()
        {
            isRecording = false;
        }

        public static RecordingInterceptor GetCurrentRecorder()
        {
            return recorder;
        }
    }
}
