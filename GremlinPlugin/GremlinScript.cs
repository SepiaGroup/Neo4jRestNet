using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Core;

namespace Neo4jRestNet.GremlinPlugin
{
	public class GremlinScript
	{
		private StringBuilder _query = new StringBuilder();
		 
		public GremlinScript()
		{
		}

		public GremlinScript(Node node)
		{
			_query.AppendFormat("g.v({0})", (long)node.NodeId);
		}
		
		public GremlinScript(Relationship relationship)
		{
			_query.AppendFormat("g.e({0})", (long)relationship.RelationshipId);
		}
		
		public GremlinScript Append(string query)
		{
			_query.Append(query);

			return this;
		}

		public GremlinScript Append(string Format, params object[] args)
		{
			_query.Append(string.Format(Format, args));

			return this;
		}

		public override string ToString()
		{
			return _query.ToString();
		}
	}
}
