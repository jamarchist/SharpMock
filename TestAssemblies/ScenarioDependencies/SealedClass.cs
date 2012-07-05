using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioDependencies
{
    public sealed class SealedClass
    {
        public int SomeField;

        public void VoidReturnNoParameters()
        {
            throw new MethodNotInterceptedException(
                "SealedClass.VoidReturnNoParameters should have been intercepted, but was called instead.");
        }

        public string StringReturnNoParameters()
        {
            throw new MethodNotInterceptedException(
                "SealedClass.StringReturnNoParameters should have been intercepted, but was called instead.");
        }

        public string StringReturnOneParameter(int param1)
        {
            throw new MethodNotInterceptedException(
                "SealedClass.StringReturnOneParameters should have been intercepted, but was called instead.");
        }
    }
}
