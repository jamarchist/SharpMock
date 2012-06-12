using System.Collections.Generic;
using System.Text;

namespace SharpMock.Core.Interception.Helpers
{
    public class ReverseStringBuilder
    {
        private readonly Stack<string> buffer = new Stack<string>();
 
        public void Prepend(string text)
        {
            buffer.Push(text);
        }

        public string Pop()
        {
            return buffer.Pop();
        }

        public override string ToString()
        {
            var forwardStringBuilder = new StringBuilder();
            while (buffer.Count > 0)
            {
                forwardStringBuilder.Append(buffer.Pop());
            }

            return forwardStringBuilder.ToString();
        }
    }
}
