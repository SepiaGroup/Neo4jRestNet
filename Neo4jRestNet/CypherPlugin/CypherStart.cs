using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherStart
	{

		private readonly StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherStart Node(string name, long id)
		{
			var comma = _sb.Length == 0 ? string.Empty : ",";
			_sb.AppendFormat("{2} {0}=node({1})", name, id, comma);

			return this;
		}

		public CypherStart Node(string name, params long[] ids)
		{
			var strIds = string.Join(",", ids);
			var comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{2} {0}=node({1})", name, strIds, comma);

			return this;
		}

		public CypherStart Node(string name, string indexName, string parameterName, object value)
		{
			var comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{4} {0}=node:{1}({2}=\"{3}\")", name, indexName, parameterName, value, comma);

			return this;
		}

		public CypherStart Node(string name, string indexName, string query)
		{
			var comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{3} {0}=node:{1}({2})", name, indexName, query, comma);

			return this;
		}

		#endregion

		#region Relationship

		public CypherStart Relationship(string name, long id)
		{
			var comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{2} {0}=relationship({1})", name, id, comma);

			return this;
		}

		public CypherStart Relationship(string name, params long[] ids)
		{
			var strIds = string.Join(",", ids);
			var comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{2} {0}=relationship({1})", name, strIds,comma);

			return this;
		}

		public CypherStart Relationship(string name, string indexName, string parameterName, object value)
		{
			var comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{4} {0}=relationship:{1}({2}='{3}')", name, indexName, parameterName, value, comma);

			return this;
		}

		public CypherStart Relationship(string name, string indexName, string query)
		{
			var comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{3} {0}=relationship:{1}('{2}')", name, indexName, query, comma);

			return this;
		}

		#endregion
	
		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
