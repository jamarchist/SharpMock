using System.Collections.Generic;

namespace SharpMock.Core.Utility
{
    public class EquatableEnumerable<T> where T : class
    {
        private readonly List<T> @this;
       
        public EquatableEnumerable(List<T> @this)
        {
            this.@this = @this;
        }

        private List<T> GetList()
        {
            return @this;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EquatableEnumerable<T>;
            if (other == null) return false;

            var that = other.GetList();

            if (@this.Count != that.Count) return false;
            for (var itemIndex = 0; itemIndex < @this.Count; itemIndex++)
            {
                if (!@this[itemIndex].Equals(that[itemIndex]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Generate.HashCode(13, 11, @this.As<object>().ToArray());
        }
    }
}