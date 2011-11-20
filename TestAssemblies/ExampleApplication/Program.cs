using System;
using System.Runtime.CompilerServices;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("A name must be supplied.");

            var name = args[0];
            var newModel = new Model();
            newModel.Name = name;

            var insert = new InsertCommand();
            insert.Arg = newModel;
            insert.Execute();

            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
    }

    public static class Dao
    {
        private static int idSeed = 0;

        public static int Insert(Model newModel)
        {
            Console.WriteLine("Inserting '{0}'", newModel.Name);
            return ++idSeed;
        }
    }

    public class InsertCommand
    {
        public Model Arg { get; set; }

        public void Execute()
        {
            Dao.Insert(Arg);
        }
    }

    public class Model
    {
        internal string Name { get; set; }
    }
}
