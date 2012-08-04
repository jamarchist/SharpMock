namespace SharpMock.Core.Utility
{
    public static class Generate
    {
        public static int HashCode(int seed, int factor, params object[] properties)
        {
            int hash = seed;
            foreach (var property in properties)
            {
                hash = (factor * hash) +  property.GetHashCode();
            }

            return hash;
        }

        public static bool Equality<T>(T @this, object that, params Function<T, object>[] getters) where T : class
        {
            var other = that as T;
            if (other == null) return false;

            foreach (var property in getters)
            {
                if (!property(@this).Equals(property(other)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
