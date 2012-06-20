using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SharpMock.Core.PostCompiler.Construction.Assemblies;
using SharpMock.Core.Utility;

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

        protected FieldInfo GetFieldFromClass(string className, string fieldName)
        {
            var @class = GetTypeFromAssembly(className);
            return @class.GetField(fieldName, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
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
                    try
                    {
                        File.Delete(AssemblyFileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to delete generated assembly: {0}", ex.Message);    
                    }
                }                
            }
        }
    }
}