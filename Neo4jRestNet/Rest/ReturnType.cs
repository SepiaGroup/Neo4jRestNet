namespace Neo4jRestNet.Rest
{
    public class ReturnType
    {
		public static readonly ReturnType Node = new ReturnType("node");
		public static readonly ReturnType Relationship = new ReturnType("relationship");
		public static readonly ReturnType Path = new ReturnType("path");
		public static readonly ReturnType FullPath = new ReturnType("fullpath");

		private readonly string _returnType;

		private ReturnType(string returnType)
		{
			_returnType = returnType;
		}

        public override string ToString()
        {
			return _returnType;
        }
    }
}