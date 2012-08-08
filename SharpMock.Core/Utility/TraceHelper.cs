using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMock.Core.Utility
{
    public static class TraceHelper
    {
        public static string GetDebuggerDisplay(object debuggable)
        {
            var debuggerDisplays = debuggable.GetType()
                .GetCustomAttributes(typeof (DebuggerDisplayAttribute), false);
            if (debuggerDisplays.Length > 0)
            {
                var display = debuggerDisplays[0] as DebuggerDisplayAttribute;
                var tokens = ParseTokens(display.Value);
                var values = EvaluateTokens(tokens, debuggable);

                return ReplaceTokens(display.Value, values);
            }

            return debuggable.ToString();
        }

        private static List<string> ParseTokens(string debuggerDisplay)
        {
            var tokens = new List<string>();
            var openBracePosition = debuggerDisplay.IndexOf('{');
            while (openBracePosition >= 0)
            {
                var closeBracePosition = debuggerDisplay.IndexOf('}');
                var token = debuggerDisplay.Substring(openBracePosition, (closeBracePosition - openBracePosition + 1));
                if (!tokens.Contains(token))
                {
                    tokens.Add(token);                    
                }

                debuggerDisplay = debuggerDisplay.Substring(closeBracePosition + 1, debuggerDisplay.Length - closeBracePosition - 1);
                openBracePosition = debuggerDisplay.IndexOf('{');
            }

            return tokens;
        }

        private static Dictionary<string, string> EvaluateTokens(List<string> tokens, object target)
        {
            var values = new Dictionary<string, string>();
            foreach (var token in tokens)
            {
                var tokenExpression = token.TrimStart('{').TrimEnd('}');
                var properties = tokenExpression.Split(new char[] {'.'});

                var propertyValue = target;
                for (int propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
                {
                    var propertyName = properties[propertyIndex];
                    propertyValue = propertyValue.GetType().GetProperty(propertyName).GetValue(propertyValue, null);
                }

                values.Add(token, propertyValue == null ? "<null>" : propertyValue.ToString());
            }

            return values;
        }

        private static string ReplaceTokens(string tokenized, Dictionary<string, string> tokens)
        {
            foreach (var token in tokens.Keys)
            {
                tokenized = tokenized.Replace(token, tokens[token]);
            }

            return tokenized;
        }
    }
}
