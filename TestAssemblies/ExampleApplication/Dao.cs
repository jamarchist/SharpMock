using System;

namespace ExampleApplication
{
    public static class Dao
    {
        private static int idSeed = 0;

        public static int Insert(Model newModel)
        {
            Console.WriteLine("Inserting '{0}'", newModel.Name);
            return ++idSeed;
        }
    }
}