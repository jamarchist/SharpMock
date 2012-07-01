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
            var clone = new string[buffer.Count];
            buffer.CopyTo(clone, 0);

            var bufferCopy = new Stack<string>();
            for (var copyIndex = clone.Length - 1; copyIndex >= 0; copyIndex--)
            {
                bufferCopy.Push(clone[copyIndex]);
            }

            var forwardStringBuilder = new StringBuilder();
            while (bufferCopy.Count > 0)
            {
                forwardStringBuilder.Append(bufferCopy.Pop());
            }

            return forwardStringBuilder.ToString();
        }

        public bool HasString()
        {
            return buffer.Count > 0;
        }

        public string[] ToStringArray()
        {
            return ToString().Split('.');
        }
    }
}
