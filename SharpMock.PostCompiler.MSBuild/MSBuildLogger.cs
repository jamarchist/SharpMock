using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ILogger = SharpMock.Core.Diagnostics.ILogger;

namespace SharpMock.PostCompiler.MSBuild
{
    internal class MSBuildLogger : ILogger
    {
        private readonly TaskLoggingHelper log;

        public MSBuildLogger(TaskLoggingHelper log)
        {
            this.log = log;
        }

        public void WriteInfo(string message, params object[] arguments)
        {
            log.LogMessage(message, arguments);
        }

        public void WriteDebug(string message, params object[] arguments)
        {
            log.LogWarning(message, arguments);
        }

        public void WriteTrace(string message, params object[] arguments)
        {
            log.LogMessage(MessageImportance.Low, message, arguments);
        }

        public void WriteError(string message, params object[] arguments)
        {
            log.LogError(message, arguments);
        }
    }
}
