using System;
using SharpMock.PostCompiler.Core;

namespace SharpMock.PostCompiler
{
	class Program
	{
		static void Main(string[] args)
		{
			var postCompilerArgs = new PostCompilerArgs(args);
			var postCompiler = new Core.PostCompiler(postCompilerArgs);

            postCompiler.InterceptAllStaticMethodCalls();

            Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}
