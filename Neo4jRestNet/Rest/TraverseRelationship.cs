using Neo4jRestNet.Core;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class TraverseRelationship
    {
		private readonly string _relationshipName;
		private readonly RelationshipDirection _direction;

		public TraverseRelationship(string relationshipName, RelationshipDirection direction)
		{
			_relationshipName = relationshipName;
			_direction = direction;
		}

        public JObject ToJson()
        {
			return new JObject {{"type", _relationshipName}, {"direction", _direction.ToString()}};
        }
    }
}