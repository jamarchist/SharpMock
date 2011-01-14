namespace SharpMock.Core.PostCompiler
{
	public class CommandLineArgs
	{
		private readonly string[] args;

		public CommandLineArgs(string[] args)
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
