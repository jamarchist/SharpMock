using System;
using System.Collections.Generic;
using System.Text;

namespace TestUtilities
{
    public class AssemblyLocations
    {
        private readonly string testAssemblyPath;
        private readonly string targetAssemblyPath;

        public AssemblyLocations(string testAssemblyPath, string targetAssemblyPath)
        {
            this.testAssemblyPath = testAssemblyPath;
            this.targetAssemblyPath = targetAssemblyPath;
        }

        public string TargetAssemblyPath
        {
            get { return targetAssemblyPath; }
        }

        public string TestAssemblyPath
        {
            get { return testAssemblyPath; }
        }

        public string NUnitConsoleRunnerPath
        {
            get { return @"C:\Projects\github\SharpMock\packages\NUnit.Runners.2.6.0.12051\tools\nunit-console.exe"; }
        }

        
    }
}
