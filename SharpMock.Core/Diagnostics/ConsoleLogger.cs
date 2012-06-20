using System;

namespace SharpMock.Core.Diagnostics
{
    public class ConsoleLogger : ILogger
    {
        public void WriteInfo(string message, params object[] arguments)
        {
            Console.WriteLine(message, arguments);
        }

        public void WriteDebug(string message, params object[] arguments)
        {
            using (new ForegroundConsoleColor(ConsoleColor.Gray))
            {
                Console.WriteLine(message, arguments);
            }
        }

        public void WriteTrace(string message, params object[] arguments)
        {
            using (new ForegroundConsoleColor(ConsoleColor.Blue))
            {
                Console.WriteLine(message, arguments);
            }
        }

        public void WriteError(string message, params object[] arguments)
        {
            using (new ForegroundConsoleColor(ConsoleColor.Red))
            {
                Console.WriteLine(message, arguments);
            }
        }

        private class ForegroundConsoleColor : IDisposable
        {
            private readonly ConsoleColor originalColor;

            public ForegroundConsoleColor(ConsoleColor useColor)
            {
                originalColor = Console.ForegroundColor;

                Console.ForegroundColor = useColor;
            }

            public void Dispose()
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }
}
