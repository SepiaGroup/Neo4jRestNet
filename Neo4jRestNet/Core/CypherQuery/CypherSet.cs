using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Core.CypherQuery
{
	public class CypherSet
	{
		private readonly StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherSet Node(string name, Properties properties)
		{
			_sb.Append(properties.Aggregate(new StringBuilder(), (sb, p) => sb.AppendFormat("{0}{1}.{2} = {3}",
																	sb.Length == 0 ? " " : ",",
																	name,
																	p.Key,
																	JToken.FromObject(p.Value).ToString(Formatting.None, new IsoDateTimeConverter())
																	))
						);					

			return this;
		}

		#endregion

		#region Relationship

		public CypherSet Relationship(string name, Properties properties)
		{
			_sb.Append(properties.Aggregate(new StringBuilder(), (sb, p) => sb.AppendFormat("{0}{1}.{2} = {3}",
																	sb.Length == 0 ? " " : ",",
																	name,
																	p.Key,
																	JToken.FromObject(p.Value).ToString(Formatting.None, new IsoDateTimeConverter())
																	))
						);

			return this;
		}

		#endregion

		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
