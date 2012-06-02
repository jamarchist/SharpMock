using System;
using Microsoft.Build.Utilities;
using SharpMock.PostCompiler.Core;

namespace SharpMock.PostCompiler.MSBuild
{
    public class InterceptSpecifications : AppDomainIsolatedTask
    {
        public string SearchLocation { get; set; }
        public string SpecificationAssembly { get; set; }

        public override bool Execute()
        {
            try
            {
                var specPath = System.IO.Path.Combine(SearchLocation, SpecificationAssembly);
                
                var compiler = new SharpMock.Core.PostCompiler.PostCompiler(
                    new PostCompilerArgs(new[] { specPath, String.Empty }));

                compiler.InterceptSpecifications();
            }
            catch (Exception e)
            {
                Log.LogError("InterceptMethodsTask failed with the following error: '{0}'.{1}{2}",
                             e.Message, Environment.NewLine, e.StackTrace);
                return false;
            }

            return true;
        }
    }
}