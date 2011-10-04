using System.Collections.Generic;
using Neo4jRestNet.Core;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Core
{
    public class RelationshipDirection
    {
		public static readonly RelationshipDirection In = new RelationshipDirection("in");
		public static readonly RelationshipDirection Out = new RelationshipDirection("out");
		public static readonly RelationshipDirection All = new RelationshipDirection("all");

		private string _Direction;

		private RelationshipDirection(string Direction)
		{
			_Direction = Direction;
		}

		public override string ToString()
		{
			return _Direction;
		}
    }
}