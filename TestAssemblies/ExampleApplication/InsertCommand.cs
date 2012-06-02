namespace ExampleApplication
{
    public class InsertCommand
    {
        public Model Arg { get; set; }

        public void Execute()
        {
            Arg.Id = Dao.Insert(Arg);
        }
    }
}