namespace SharpMock.PostCompiler.Core
{
	public class PostCompilerArgs
	{
		private readonly string[] args;

		public PostCompilerArgs(string[] args)
		{
			this.args = args;
		}

		public bool AreValid()
		{
			return args.Length == 2;
		}

		public string TestAssemblyPath
		{
			get { return args[0]; }
		}

		public string ReferencedAssemblyPath
		{
			get { return args[1]; }
		}
	}
}