using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherStart
	{

		private StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherStart Node(string Name, long Id)
		{
			string comma = _sb.Length == 0 ? string.Empty : ",";
			_sb.AppendFormat("{2} {0}=node({1})", Name, Id, comma);

			return this;
		}

		public CypherStart Node(string Name, params long[] Ids)
		{
			string ids = string.Join(",", Ids);
			string comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{2} {0}=node({1})", Name, ids, comma);

			return this;
		}

		public CypherStart Node(string Name, string IndexName, string ParameterName, object Value)
		{
			string comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{4} {0}=node:{1}({2}='{3}')", Name, IndexName, ParameterName, Value, comma);

			return this;
		}

		public CypherStart Node(string Name, string IndexName, string Query)
		{
			string comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{3} {0}=node:{1}('{2}')", Name, IndexName, Query, comma);

			return this;
		}

		#endregion

		#region Relationship

		public CypherStart Relationship(string Name, long Id)
		{
			string comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{2} {0}=relationship({1})", Name, Id, comma);

			return this;
		}

		public CypherStart Relationship(string Name, params long[] Ids)
		{
			string ids = string.Join(",", Ids);
			string comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{2} {0}=relationship({1})", Name, ids,comma);

			return this;
		}

		public CypherStart Relationship(string Name, string IndexName, string ParameterName, object Value)
		{
			string comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{4} {0}=relationship:{1}({2}='{3}')", Name, IndexName, ParameterName, Value, comma);

			return this;
		}

		public CypherStart Relationship(string Name, string IndexName, string Query)
		{
			string comma = _sb.Length == 0 ? string.Empty : ",";

			_sb.AppendFormat("{2} {0}=relationship:{1}('{2}')", Name, IndexName, Query, comma);

			return this;
		}

		#endregion
	
		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
