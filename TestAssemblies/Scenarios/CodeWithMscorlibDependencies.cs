using System;
using System.Collections.Generic;
using System.Text;

namespace Scenarios
{
    public class CodeWithMscorlibDependencies
    {
        public Tuple<bool, int, decimal> ParsesHardCodedStrings()
        {
            var actualBool = Boolean.Parse("not true");
            //var actualInt = int.Parse("four");
            var actualDecimal = decimal.Parse("three point one four");

            return new Tuple<bool, int, decimal>(actualBool, 0, actualDecimal);
        }
    }
}
