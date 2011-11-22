namespace ScenarioDependencies
{
    public class SomeConcreteClass
    {
        public void SomeMethod()
        {
            throw new MethodNotInterceptedException("SomeConcreteClass.SomeMethod should have been intercepted but was not.");
        }
    }
}