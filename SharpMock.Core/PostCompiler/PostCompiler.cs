using Microsoft.Cci;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.PostCompiler.Core;

namespace SharpMock.Core.PostCompiler
{
    public class PostCompiler
	{
		private readonly PostCompilerArgs postCompilerArgs;
	    private readonly IMetadataHost host;
	    private readonly IAssembly sharpMockDelegateTypes;
	    private readonly IUnit sharpMockCore;
	    private readonly ILogger log;

		public PostCompiler(PostCompilerArgs postCompilerArgs, ILogger log)
		{
		    this.log = log;

            if (postCompilerArgs.AreValid())
            {
                var nameTable = new NameTable();
                host = new PeReader.DefaultHost(nameTable);

                sharpMockCore = host.LoadUnitFrom(
                            System.Reflection.Assembly.GetExecutingAssembly().Location
                        );
                sharpMockDelegateTypes = sharpMockCore as IAssembly;

                host.Errors += host_Errors;            
            }

			this.postCompilerArgs = postCompilerArgs;

#if DEBUG && LAUNCH_DEBUGGER
            System.Diagnostics.Debugger.Launch();
#endif
        }
        
        public void InterceptSpecifications()
        {
            // get mutable test assembly
            // add sharpmockdelegatetypes assembly reference
            // load referenced assemblies into host
            // scan for interception specifications
            // add interception targets
            // replace specified static method calls
            // save assembly
            // serialize specs

            var pipeline = new PostCompilerPipeline();
            pipeline.AddStep(new GetMutableTestAssembly());
            pipeline.AddStep(new AddReferenceToSharpMockTypesAssembly());
            pipeline.AddStep(new LoadReferencesIntoHost());
            pipeline.AddStep(new ScanForInterceptionSpecifications());
            pipeline.AddStep(new AddInterceptionTargetsToAssembly());
            pipeline.AddStep(new ReplaceInterceptedMethodsWithAlternativeInvocations());
            pipeline.AddStep(new SaveTestAssembly());
            pipeline.AddStep(new SerializeAllSpecifications());

            var ctx = new PostCompilerContext();
            ctx.Args = postCompilerArgs;
            ctx.Host = host;
            ctx.Log = log;
            ctx.SharpMockCore = sharpMockCore;
            ctx.SharpMockDelegateTypes = sharpMockDelegateTypes;
            ctx.Registry = new ReplacementRegistry(log);

            pipeline.Execute(ctx);
        }

        public void InterceptAllStaticMethodCalls()
        {
            // get mutable test assembly
            // add sharpmockdelegatetypes assembly reference
            // load referenced assemblies into host
            // scan for interception specifications
            // add interception targets
            // replace specified static method calls
            // save assembly

            var pipeline = new PostCompilerPipeline();
            pipeline.AddStep(new GetMutableTargetAssembly());
            pipeline.AddStep(new AddReferenceToSharpMockTypesAssembly());
            pipeline.AddStep(new LoadReferencesIntoHost());
            pipeline.AddStep(new ScanForStaticMethods());
            pipeline.AddStep(new AddInterceptionTargetsToAssembly());
            pipeline.AddStep(new ReplaceStaticMethodCalls());
            pipeline.AddStep(new SaveTargetAssembly());

            var ctx = new PostCompilerContext();
            ctx.Args = postCompilerArgs;
            ctx.Host = host;
            ctx.Log = log;
            ctx.SharpMockCore = sharpMockCore;
            ctx.SharpMockDelegateTypes = sharpMockDelegateTypes;
            ctx.Registry = new ReplacementRegistry(log);

            pipeline.Execute(ctx);
        }

        private void host_Errors(object sender, Microsoft.Cci.ErrorEventArgs e)
        {
            foreach (var error in e.Errors)
            {
                log.WriteError(error.Message); 
            }
        }
	}
}
