using System;

namespace SharpMock.Core
{
    public class AssertionFailedException : ApplicationException
    {
        public AssertionFailedException():base("The assertion failed."){}
    }
}
