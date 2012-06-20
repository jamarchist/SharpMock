using System;
using SharpMock.Core.Diagnostics;
using SharpMock.PostCompiler.Core;

namespace SharpMock.PostCompiler
{
	class Program
	{
		static void Main(string[] args)
		{
			var postCompilerArgs = new PostCompilerArgs(args);
			var postCompiler = new SharpMock.Core.PostCompiler.PostCompiler(postCompilerArgs, new ConsoleLogger());

            postCompiler.InterceptSpecifications();
            postCompiler.InterceptAllStaticMethodCalls();

            Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}
