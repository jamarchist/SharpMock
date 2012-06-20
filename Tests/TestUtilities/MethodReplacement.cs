namespace TestUtilities
{
    public class MethodReplacement
    {
        public object ReplacementArg1 { get; private set; }
        public void Call(object replacementArg)
        {
            ReplacementArg1 = replacementArg;
        }
    }
}