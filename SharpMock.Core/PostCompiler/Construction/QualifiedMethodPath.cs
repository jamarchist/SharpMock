using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public class QualifiedMethodPath : IEnumerable<string>
    {
        private readonly Stack<string> elements = new Stack<string>();
        private string className;

        public IEnumerator<string> GetEnumerator()
        {
            return new Queue<string>(elements).GetEnumerator();
            //return new Stack<string>(elements).GetEnumerator();
        }

        public string GetClassName()
        {
            return className;
            //var copy = new Stack<string>(elements);
            //string className = null;
            //while (copy.Count > 0)
            //{
            //    className = copy.Pop();
            //}
            //return className;
        }

        public override string ToString()
        {
            var path = new StringBuilder();
            var copy = new Stack<string>(elements);

            foreach (var element in copy)
            {
                if (IsFirstElementIn(element, copy))
                {
                    path.Append('.');
                }

                path.AppendFormat("{0}", element);    
            }

            return path.ToString();            
        }

        public void AddPathElement(string pathElement)
        {
            elements.Push(pathElement);
            //elements.Enqueue(pathElement);
        }

        public void AddClassName(string classNameElement)
        {
            //AddPathElement(classNameElement);
            className = classNameElement;
        }

        public override bool Equals(object obj)
        {
            return ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static bool IsFirstElementIn(string element, Stack<string> elements)
        {
            var copy = new Stack<string>(elements);
            string copyElement = null;
            while (copy.Count > 0)
            {
                copyElement = copy.Pop();
            }

            return element == copyElement;
            //return element == copy.Dequeue();
        }
    }
}
