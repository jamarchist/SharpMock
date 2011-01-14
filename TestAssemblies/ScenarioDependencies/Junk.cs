namespace ScenarioDependencies
{
    public delegate TReturn Fn<TArg1, TReturn>(TArg1 arg1);

    class Junk
    {
        public static string MethodA(int arg1)
        {
            Fn<int, string> anon = x => Original(x);

            return null;
        }

        public static string Original(int a1)
        {
            return null;
        }
    }
}
