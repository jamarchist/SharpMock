using System;
using Microsoft.Build.Utilities;
using SharpMock.PostCompiler.Core;

namespace SharpMock.PostCompiler.MSBuild
{
    public class InterceptAllStaticMethods : AppDomainIsolatedTask
    {
        public string SearchLocation { get; set; }
        public string TargetAssembly { get; set; }
        
        public override bool Execute()
        {
            try
            {
                var targetPath = System.IO.Path.Combine(SearchLocation, TargetAssembly);

                var compiler = new SharpMock.Core.PostCompiler.PostCompiler(
                    new PostCompilerArgs(new[] { String.Empty, targetPath }), new MSBuildLogger(Log));

                compiler.InterceptAllStaticMethodCalls();                                                    
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
