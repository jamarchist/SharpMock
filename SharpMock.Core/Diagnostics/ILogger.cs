namespace SharpMock.Core.Diagnostics
{
    public interface ILogger
    {
        void WriteInfo(string message, params object[] arguments);
        void WriteDebug(string message, params object[] arguments);
        void WriteTrace(string message, params object[] arguments);
        void WriteError(string message, params object[] arguments);
    }
}
