using System;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting app...");

            if (args.Length != 1) throw new ArgumentException("A name must be supplied.");

            var name = args[0];
            var newModel = new Model();
            newModel.Name = name;

            var insert = new InsertCommand();
            insert.Arg = newModel;
            insert.Execute();

            var insertedId = insert.Arg.Id;
            Console.WriteLine("Model id is {0}", insertedId);

            Console.WriteLine("Stopping app...");
        }
    }
}
