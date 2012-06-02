namespace TestUtilities
{
    public static class MethodReplacement
    {
        public static object ReplacementArg1 { get; private set; }
        public static void Call(object replacementArg)
        {
            ReplacementArg1 = replacementArg;
        }
    }
}