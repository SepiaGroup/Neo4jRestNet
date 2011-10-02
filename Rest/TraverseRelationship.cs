using System.Collections.Generic;
using Neo4jRestNet.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Neo4jRestNet.Rest
{
    public class TraverseRelationship
    {
		private string _RelationshipName;
		private RelationshipDirection _Direction;

		public TraverseRelationship(string RelationshipName, RelationshipDirection Direction)
		{
			_RelationshipName = RelationshipName;
			_Direction = Direction;
		}

        public JObject ToJson()
        {
			JObject jo = new JObject();
			jo.Add("type", _RelationshipName);
			jo.Add("direction", _Direction.ToString());

			return jo;
			
        }
    }
}