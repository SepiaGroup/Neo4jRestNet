using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Core;

namespace Neo4jRestNet.GremlinPlugin
{
	public static class Commands
	{
		#region g()

		public static GremlinScript g(this GremlinScript query, Node node)
		{
			return query.Append(string.Format("g.v('{0}')", (long)node.NodeId));
		}

		public static GremlinScript g(this GremlinScript query, Relationship relationship)
		{
			return query.Append(string.Format("g.e('{0}')", (long)relationship.RelationshipId));
		}

		public static GremlinScript gV(this GremlinScript query, GEID geid)
		{
			return query.Append(string.Format("g.v('{0}')", (long)geid));
		}

		public static GremlinScript gE(this GremlinScript query, GEID geid)
		{
			return query.Append(string.Format("g.e('{0}')", (long)geid));
		}

		#endregion

		#region In()

		public static GremlinScript In(this GremlinScript query)
		{
			return query.Append(".in()");
		}

		public static GremlinScript In(this GremlinScript query, string RelationshipName)
		{
			return query.Append(string.Format(".in('{0}')", RelationshipName));
		}

		#endregion

		#region InE()

		public static GremlinScript InE(this GremlinScript query)
		{
			return query.Append(".inE()");
		}

		public static GremlinScript InE(this GremlinScript query, string RelationshipName)
		{
			return query.Append(string.Format(".inE('{0}')", RelationshipName));
		}

		#endregion

		#region InV()

		public static GremlinScript InV(this GremlinScript query)
		{
			return query.Append(".inV()");
		}

		public static GremlinScript InV(this GremlinScript query, string RelationshipName)
		{
			return query.Append(string.Format(".inV('{0}')", RelationshipName));
		}

		#endregion

		#region Out()

		public static GremlinScript Out(this GremlinScript query)
		{
			return query.Append(".out()");
		}

		public static GremlinScript Out(this GremlinScript query, string RelationshipName)
		{
			return query.Append(string.Format(".out('{0}')", RelationshipName));
		}

		#endregion

		#region OutE()

		public static GremlinScript OutE(this GremlinScript query)
		{
			return query.Append(".outE()");
		}

		public static GremlinScript OutE(this GremlinScript query, string RelationshipName)
		{
			return query.Append(string.Format(".outE('{0}')", RelationshipName));
		}

		#endregion

		#region OutV()

		public static GremlinScript OutV(this GremlinScript query)
		{
			return query.Append(".outV()");
		}

		public static GremlinScript OutV(this GremlinScript query, string RelationshipName)
		{
			return query.Append(string.Format(".outV('{0}')", RelationshipName));
		}

		#endregion

		#region Filter()

		public static GremlinScript Filter(this GremlinScript query, string Filter)
		{
			return query.Append(string.Format(".filter{{{0}}}", Filter));
		}

		public static GremlinScript Filter(this GremlinScript query, string Filter, params object[] args)
		{
			return query.Append(string.Format(".filter{{{0}}}", string.Format(Filter, args)));
		}

		#endregion

		#region Iterators()

		public static GremlinScript Sort(this GremlinScript query, string Sort, params object[] args)
		{
			return query.Append(string.Format(".sort({{{0}}})", string.Format(Sort, args)));
		}

		public static GremlinScript Reverse(this GremlinScript query)
		{
			return query.Append(".reverse()");
		}

		#endregion

		#region Conversions()

		public static GremlinScript ToList(this GremlinScript query)
		{
			return query.Append(".toList()");
		}

		public static GremlinScript IteratorToPipe(this GremlinScript query)
		{
			return query.Append("._()");
		}

		#endregion

		#region Misc

		public static GremlinScript As(this GremlinScript query, string Label)
		{
			return query.Append(string.Format(".as('{0}')", Label));
		}

		#endregion

		#region Table()

		public static GremlinScript NewTable(this GremlinScript query, string Name)
		{
			return query.Append(string.Format("{0} = new Table();", Name));
		}

		public static GremlinScript Table(this GremlinScript query, string Name)
		{
			return query.Append(string.Format(".table({0})", Name));
		}

		public static GremlinScript Table(this GremlinScript query, string Name, IEnumerable<string> ColumnNames)
		{
			StringBuilder names = new StringBuilder();
			foreach (string name in ColumnNames)
			{
				names.AppendFormat("{0}'{1}'", names.Length == 0 ? string.Empty : ", ", name);
			}

			return query.Append(string.Format(".table({0}, [{1}])", Name, names));
		}

		#endregion

	}
}
