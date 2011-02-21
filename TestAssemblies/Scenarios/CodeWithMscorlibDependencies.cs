using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Scenarios
{
    public class CodeWithMscorlibDependencies
    {
        public Tuple<bool, int, decimal> ParsesHardCodedStrings()
        {
            var actualBool = Boolean.Parse("not true");
            var actualInt = int.Parse("four");
            var actualDecimal = decimal.Parse("three point one four");

            return new Tuple<bool, int, decimal>(actualBool, actualInt, actualDecimal);
        }

        public Delegate CreatesDelegate()
        {
            var methodInfo = typeof (Console).GetMethod("WriteLine", new[] {typeof (string)});
            return Delegate.CreateDelegate(typeof (Console), methodInfo);
        }

        public string GetsMachineName()
        {
            return Environment.MachineName;
        }

        public DateTime GetsCurrentDateTime()
        {
            return DateTime.Now;
        }

        public bool ChecksIfFileExists()
        {
            return System.IO.File.Exists(@"C:\Temp\iknowthisfiledoesntexist.txt");
        }
    }
}
