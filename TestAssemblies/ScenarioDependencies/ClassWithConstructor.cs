namespace ScenarioDependencies
{
    public class ClassWithConstructor
    {
        public ClassWithConstructor()
        {
            throw new MethodNotInterceptedException("ClassWithConstructor.ctor()");
        }

        public ClassWithConstructor(string ignored)
        {
            // 
        }
    }
}
