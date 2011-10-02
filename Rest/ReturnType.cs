using System.Collections.Generic;
using Neo4jRestNet.Core;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class ReturnType
    {
		public static readonly ReturnType Node = new ReturnType("node");
		public static readonly ReturnType Relationship = new ReturnType("relationship");
		public static readonly ReturnType Path = new ReturnType("path");
		public static readonly ReturnType FullPath = new ReturnType("fullpath");

		private string _ReturnType;

		private ReturnType(string ReturnType)
		{
			_ReturnType = ReturnType;
		}

        public override string ToString()
        {
			return _ReturnType;
        }
    }
}