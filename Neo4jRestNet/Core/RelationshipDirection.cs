namespace Neo4jRestNet.Core
{
    public class RelationshipDirection
    {
		public static readonly RelationshipDirection In = new RelationshipDirection("in");
		public static readonly RelationshipDirection Out = new RelationshipDirection("out");
		public static readonly RelationshipDirection All = new RelationshipDirection("all");

		private readonly string _direction;

		private RelationshipDirection(string direction)
		{
			_direction = direction;
		}

		public override string ToString()
		{
			return _direction;
		}
    }
}