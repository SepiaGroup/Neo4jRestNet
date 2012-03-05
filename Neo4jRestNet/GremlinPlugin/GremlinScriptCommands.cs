using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Core;
using System.Linq.Expressions;

namespace Neo4jRestNet.GremlinPlugin
{
	public static class GremlinScriptCommands
	{
		#region g()

		public static GremlinScript g(this GremlinScript query, Node node)
		{
			return query.Append("g.v({0})", node.Id);
		}

		public static GremlinScript g(this GremlinScript query, IEnumerable<Node> nodes)
		{
			return query.Append("g.v({0})", string.Join(",", nodes.Select(s => s.Id).ToArray()));
		}

		public static GremlinScript g(this GremlinScript query, Relationship relationship)
		{
			return query.Append("g.e({0})", relationship.Id);
		}

		public static GremlinScript g(this GremlinScript query, IEnumerable<Relationship> relationships)
		{
			return query.Append("g.e({0})", string.Join(",", relationships.Select(s => s.Id).ToArray()));
		}
		
		public static GremlinScript gV(this GremlinScript query, long Id)
		{
			return query.Append("g.v({0})", Id);
		}

		public static GremlinScript gV(this GremlinScript query, IEnumerable<long> Ids)
		{
			return query.Append("g.v({0})", string.Join(",", Ids));
		}
		
		public static GremlinScript gV(this GremlinScript query, Node node)
		{
			return gV(query, node.Id);
		}

		public static GremlinScript gV(this GremlinScript query, IEnumerable<Node> nodes)
		{
			return gV(query, nodes.Select(s => s.Id));
		}

		public static GremlinScript gE(this GremlinScript query, long Id)
		{
			return query.Append("g.e({0})", Id);
		}

		public static GremlinScript gE(this GremlinScript query, IEnumerable<long> Ids)
		{
			return query.Append("g.e({0})", string.Join(",", Ids));
		}
		
		public static GremlinScript gE(this GremlinScript query, Relationship relationship)
		{
			return gE(query, relationship.Id);
		}

		public static GremlinScript gE(this GremlinScript query, IEnumerable<Relationship> relationships)
		{
			return gE(query, relationships.Select(s => s.Id));
		}
	
		#endregion

		#region In()

		public static GremlinScript In(this GremlinScript query)
		{
			return query.Append(".in()");
		}

		public static GremlinScript In(this GremlinScript query, params Enum[] relationshipName)
		{
			return In(query, relationshipName.Select(r => r.ToString()).ToArray());
		}

		public static GremlinScript In(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".in({0})", string.Join(",", relationshipName));
		}

		#endregion

		#region InE()

		public static GremlinScript InE(this GremlinScript query)
		{
			return query.Append(".inE()");
		}

		public static GremlinScript InE(this GremlinScript query, params Enum[] relationshipName)
		{
			var names = relationshipName.Select(r => r.ToString()).ToArray();

			return InE(query, names);
		}

		public static GremlinScript InE(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".inE({0})", string.Join(",", relationshipName));
		}

		#endregion

		#region InV()

		public static GremlinScript InV(this GremlinScript query)
		{
			return query.Append(".inV()");
		}

		public static GremlinScript InV(this GremlinScript query, params Enum[] relationshipName)
		{
			var names = relationshipName.Select(r => r.ToString()).ToArray();

			return InV(query, names);
		}

		public static GremlinScript InV(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".inV({0})",  string.Join(",", relationshipName));
		}

		#endregion

		#region Out()

		public static GremlinScript Out(this GremlinScript query)
		{
			return query.Append(".out()");
		}

		public static GremlinScript Out(this GremlinScript query, params Enum[] relationshipName)
		{
			var names = relationshipName.Select(r => r.ToString()).ToArray();

			return Out(query, names);
		}

		public static GremlinScript Out(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".out({0})", string.Join(",", relationshipName));
		}

		#endregion

		#region OutE()

		public static GremlinScript OutE(this GremlinScript query)
		{
			return query.Append(".outE()");
		}

		public static GremlinScript OutE(this GremlinScript query, params Enum[] relationshipName)
		{
			var names = relationshipName.Select(r => r.ToString()).ToArray();

			return OutE(query, names);
		}

		public static GremlinScript OutE(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".outE({0})", string.Join(",", relationshipName));
		}

		#endregion

		#region OutV()

		public static GremlinScript OutV(this GremlinScript query)
		{
			return query.Append(".outV()");
		}

		public static GremlinScript OutV(this GremlinScript query, params Enum[] relationshipName)
		{
			var names = relationshipName.Select(r => r.ToString()).ToArray();

			return OutV(query, names);
		}

		public static GremlinScript OutV(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".outV({0})", string.Join(",", relationshipName));
		}

		#endregion

		#region Back()

		public static GremlinScript Back(this GremlinScript query, string label)
		{
			return query.Append(string.Format(".back('{0}')", label));
		}

		public static GremlinScript Back(this GremlinScript query, int steps)
		{
			return query.Append(string.Format(".back({0})", steps));
		}

		#endregion

		#region BothE()

		public static GremlinScript BothE(this GremlinScript query)
		{
			return query.Append(".bothE()");
		}

		public static GremlinScript BothE(this GremlinScript query, params Enum[] relationshipName)
		{
			var names = relationshipName.Select(r => r.ToString()).ToArray();

			return BothE(query, names);
		}

		public static GremlinScript BothE(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".bothE({0})", string.Join(",", relationshipName));
		}

		#endregion

		#region BothV()

		public static GremlinScript BothV(this GremlinScript query)
		{
			return query.Append(".bothV()");
		}

		public static GremlinScript BothV(this GremlinScript query, params Enum[] relationshipName)
		{
			var names = relationshipName.Select(r => r.ToString()).ToArray();

			return BothV(query, names);
		}

		public static GremlinScript BothV(this GremlinScript query, params string[] relationshipName)
		{
			return query.Append(".bothV({0})", string.Join(",", relationshipName));
		}

		#endregion

		#region Filter()

		public static GremlinScript Filter(this GremlinScript query, string filter)
		{
			return query.Append(string.Format(".filter{{{0}}}", filter), false);
		}

		public static GremlinScript Filter(this GremlinScript query, string filter, params object[] args)
		{
			return query.Append(string.Format(".filter{{{0}}}", string.Format(filter, args)));
		}

		public static GremlinScript Filter(this GremlinScript query, Expression<Func<JavaObject, object>> func)
		{
			return query.Append(string.Format(".filter{{{0}}}", new ParseJavaLambda().Parse(func)), false);
		}

		public static GremlinScript Filter(this GremlinScript query, Expression<Func<GremlinScript, object>> func)
		{
			return query.Append(string.Format(".filter{{{0}}}", new ParseJavaLambda().Parse(func)), false);
		}

		#endregion

		#region Sort()

		public static GremlinScript Sort(this GremlinScript query, string sort, params object[] args)
		{
			return query.Append(string.Format(".sort({{{0}}})", string.Format(sort, args)), false);
		}

		public static GremlinScript Sort(this GremlinScript query, Expression<Func<JavaObject, object>> func)
		{
			return query.Append(string.Format(".sort{{{0}}}", new ParseJavaLambda().Parse(func)), false);
		}

		#endregion

		#region Iterators()

		public static GremlinScript Reverse(this GremlinScript query)
		{
			return query.Append(".reverse()");
		}

		public static GremlinScript Each(this GremlinScript query, string each, params object[] args)
		{
			return query.Append(string.Format(".each{{{0}}}", string.Format(each, args)));
		}

		public static GremlinScript Unique(this GremlinScript query)
		{
			return query.Append(".unique()");
		}

		#endregion

		#region Conversions()

		public static GremlinScript ToList(this GremlinScript query)
		{
			return query.Append(".toList()");
		}

		public static GremlinScript ToPipe(this GremlinScript query)
		{
			return query.Append("._()");
		}

		#endregion

		#region Misc

		public static GremlinScript As(this GremlinScript query, string label)
		{
			return query.Append(".as({0})", label);
		}

		public static GremlinScript Aggregate(this GremlinScript query, string variable)
		{
			return query.Append(".aggregate({0})", variable);
		}

		public static GremlinScript Except(this GremlinScript query, string variable)
		{
			return query.Append(".except({0})", variable);
		}

		public static GremlinScript Retain(this GremlinScript query, string variable)
		{
			return query.Append(".retain({0})", variable);
		}

		public static GremlinScript NodeIndexLookup(this GremlinScript query, IEnumerable<KeyValuePair<Enum, object>> propertyKeyValue)
		{
			var sb = new StringBuilder();
			foreach (var kvp in propertyKeyValue)
			{
				if (kvp.Value is string)
				{
					sb.AppendFormat("{0}'{1}':'{2}'", sb.Length > 0 ? "," : "", kvp.Key, kvp.Value);
				}
				else if (kvp.Value is bool)
				{
					sb.AppendFormat("{0}'{1}':{2}", sb.Length > 0 ? "," : "", kvp.Key, (bool)kvp.Value ? "true" : "false");
				}
				else
				{
					sb.AppendFormat("{0}'{1}':{2}", sb.Length > 0 ? "," : "", kvp.Key, kvp.Value);
				}
			}

			return query.Append("g.V[[{0}]]", sb.ToString());
		}

		public static GremlinScript NodeIndexLookup(this GremlinScript query, IEnumerable<KeyValuePair<string, object>> propertyKeyValue)
		{
			var sb = new StringBuilder();
			foreach (var kvp in propertyKeyValue)
			{
				if (kvp.Value is string)
				{
					sb.AppendFormat("{0}'{1}':'{2}'", sb.Length > 0 ? "," : "", kvp.Key, kvp.Value);
				}
				else if (kvp.Value is bool)
				{
					sb.AppendFormat("{0}'{1}':{2}", sb.Length > 0 ? "," : "", kvp.Key, (bool)kvp.Value ? "true" : "false");
				}
				else
				{
					sb.AppendFormat("{0}'{1}':{2}", sb.Length > 0 ? "," : "", kvp.Key, kvp.Value);
				}
			}

			return query.Append("g.V[[{0}]]", sb.ToString());
		}

		public static GremlinScript RelationshipIndexLookup(this GremlinScript query, IEnumerable<KeyValuePair<string, string>> propertyKeyValue)
		{
			var sb = new StringBuilder();
			foreach (var kvp in propertyKeyValue)
			{
				sb.AppendFormat("{0}'{1}':'{2}'", sb.Length > 0 ? "," : "", kvp.Key, kvp.Value);
			}

			return query.Append("g.E[[{0}]]", sb.ToString());
		}

		public static GremlinScript Table(this GremlinScript query, string name)
		{
			return query.Append(string.Format(".table({0})", name));
		}

		public static GremlinScript Table(this GremlinScript query, string name, params string[] columnNames)
		{
			string names = string.Join(",", columnNames.Select(r => string.Concat("'", r, "'")));
			return query.Append(string.Format(".table({0}, [{1}])", name, names));
		}

		#endregion

		#region Variable Definitions()

		public static GremlinScript NewTable(this GremlinScript query, string name)
		{
			return query.Append(string.Format("{0} = new Table();", name));
		}

		public static GremlinScript NewArray(this GremlinScript query, string name)
		{
			return query.Append(string.Format("{0} = [];", name));
		}

		#endregion

	}
}
