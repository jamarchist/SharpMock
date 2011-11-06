using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SharpMock.Core.PostCompiler.Construction.Assemblies;

namespace ConstructionTests
{
    public class BaseConstructionTests
    {
        protected IAssemblyBuilder AssemblyBuilder { get; private set; }
        protected string AssemblyName { get; private set; }
        protected string AssemblyFileName { get; private set; }

        private string GenerateAssemblyName()
        {
            return String.Format("test-{0}", Guid.NewGuid());
        }

        protected MethodInfo GetMethodFromClass(string className, string methodName)
        {
            var @class = GetTypeFromAssembly(className);
            return @class.GetMethod(methodName);
        }

        protected Type GetTypeFromAssembly(string typeName)
        {
            var assembly = Assembly.ReflectionOnlyLoadFrom(AssemblyFileName);
            return assembly.GetType(String.Format("{0}.{1}", AssemblyName, typeName));
        }

        [SetUp]
        public void CreateAssemblyBuilder()
        {
            AssemblyBuilder = new AssemblyBuilder();
            AssemblyName = GenerateAssemblyName();
            AssemblyFileName = String.Format("{0}.dll", AssemblyName);
        }

        [TearDown]
        public void DeleteTestFile()
        {
            try
            {
                PeVerify.VerifyAssembly(AssemblyFileName);
            }
            finally
            {
                if (File.Exists(AssemblyFileName))
                {
                    File.Delete(AssemblyFileName);
                }                
            }
        }
    }
}