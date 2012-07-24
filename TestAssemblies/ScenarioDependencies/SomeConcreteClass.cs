namespace ScenarioDependencies
{
    public class SomeConcreteClass
    {
        public int SomeField;

        public string SomeProperty
        {
            get
            {
                throw new MethodNotInterceptedException("SomeConcreteClass.SomeProperty.get");
            }
            set
            {
                throw new MethodNotInterceptedException("SomeConcreteClass.SomeProperty.set");
            }
        }

        public void SomeMethod()
        {
            throw new MethodNotInterceptedException("SomeConcreteClass.SomeMethod");
        }
    }
}