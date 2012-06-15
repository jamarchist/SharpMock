using System;

namespace SharpMock.Core.Diagnostics
{
    internal class NullLogger : ILogger
    {
        public void WriteInfo(string message, params object[] arguments)
        {
            // System.Diagnostics.Debug.WriteLine(String.Format(message, arguments));
        }

        public void WriteDebug(string message, params object[] arguments)
        {
            // throw new NotImplementedException();
        }

        public void WriteTrace(string message, params object[] arguments)
        {
            // throw new NotImplementedException();
        }

        public void WriteError(string message, params object[] arguments)
        {
            // throw new NotImplementedException();
        }
    }
}
