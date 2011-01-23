namespace SharpMock.Core.Syntax
{
    public class Faker : IFaker
    {
        public void CallsTo(VoidAction methodToRecord)
        {
            methodToRecord();
        }
    }
}