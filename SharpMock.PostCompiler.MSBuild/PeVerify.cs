using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Utilities;

namespace SharpMock.PostCompiler.MSBuild
{
    public class PeVerify : Task
    {
        private class AssemblyResult
        {
            public string AssemblyPath { get; set; }
            public SharpMock.Core.Utility.PeVerifyResult Result { get; set; }
        }

        public string AllAssembliesIn { get; set; }
        public string SpecificAssembly { get; set; }

        public override bool Execute()
        {
            var assemblies = new List<string>();
            if (String.IsNullOrEmpty(AllAssembliesIn))
            {
                assemblies.Add(SpecificAssembly);
            }
            else
            {
                var files = Directory.GetFiles(AllAssembliesIn, "*.dll|*.exe");
                assemblies.AddRange(files);
            }

            var results = new List<AssemblyResult>();
            foreach (var assembly in assemblies)
            {
                var result = SharpMock.Core.Utility.PeVerify.VerifyAssembly(assembly);
                results.Add(new AssemblyResult { AssemblyPath = assembly, Result = result });
            }

            var failed = false;
            foreach (var assemblyResult in results)
            {
                if (assemblyResult.Result.Errors.Count > 0 || assemblyResult.Result.MetaDataErrors.Count > 0)
                {
                    failed = true;

                    Log.LogError("Assembly at '{0}' failed PE Verification.", assemblyResult.AssemblyPath);
                    foreach (var error in assemblyResult.Result.Errors)
                    {
                        Log.LogError("  {0}", error);
                    }

                    foreach (var metaDataError in assemblyResult.Result.MetaDataErrors)
                    {
                        Log.LogError("  {0}", metaDataError);
                    }
                }
            }

            return failed;
        }
    }
}
