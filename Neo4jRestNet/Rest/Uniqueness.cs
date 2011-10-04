using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class Uniqueness
    {
		public static readonly Uniqueness None = new Uniqueness("none");
		public static readonly Uniqueness NodeGlobal = new Uniqueness("node_global");
		public static readonly Uniqueness RelationshipGlobal = new Uniqueness("relationship_global");
		public static readonly Uniqueness NodePath = new Uniqueness("node_path");
		public static readonly Uniqueness RelationshipPath = new Uniqueness("relationship_path");
		public static readonly Uniqueness NodeRecent = new Uniqueness("node_recent");
		public static readonly Uniqueness RelationshipRecent = new Uniqueness("relationship_recent");

		private string _Uniqueness;

		private Uniqueness(string Uniqueness)
		{
			_Uniqueness = Uniqueness;
		}

        public JProperty ToJson()
        {
			return new JProperty("uniqueness", _Uniqueness);
        }
    }
}