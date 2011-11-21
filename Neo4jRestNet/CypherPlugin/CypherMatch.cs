using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherMatch : ICypherObject
	{

		private StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherMatch Node()
		{
			return Node(string.Empty);
		}

		public CypherMatch Node(string Name)
		{
			_sb.AppendFormat(" ({0}) ", Name);

			return this;
		}

		#endregion

		#region Relationship

		public CypherMatch Relationship()
		{
			return Relationship(string.Empty);
		}

		public CypherMatch Relationship(string Name)
		{
			_sb.AppendFormat(" ({0}) ", Name);

			return this;
		}

		#endregion

		#region Any

		public CypherMatch Any(string Name)
		{
			_sb.AppendFormat("-[{0}]-", Name);

			return this;
		}

		public CypherMatch Any(string Name, string Relationship)
		{
			_sb.AppendFormat("-[{0}:{1}]-", Name, Relationship);

			return this;
		}

		public CypherMatch Any(string Name, string Relationship, int MinHops, int MaxHops)
		{
			_sb.AppendFormat("-[{0}:{1}*{2}..{3}]-", Name, Relationship, MinHops, MaxHops);

			return this;
		}

		#endregion

		#region To

		public CypherMatch To(string Relationship)
		{
			_sb.AppendFormat("-[:{0}]->", Relationship);

			return this;
		}

		public CypherMatch To(string Name, string Relationship)
		{
			_sb.AppendFormat("-[{0}:{1}]->", Name, Relationship);

			return this;
		}

		public CypherMatch To(string Name, string Relationship, int MinHops, int MaxHops)
		{
			_sb.AppendFormat("-[{0}:{1}*{2}..{3}]->", Name, Relationship, MinHops, MaxHops);

			return this;
		}

		#endregion

		#region From

		public CypherMatch From(string Relationship)
		{
			_sb.AppendFormat("<-[:{0}]-", Relationship);

			return this;
		}

		public CypherMatch From(string Name, string Relationship)
		{
			_sb.AppendFormat("<-[{0}:{1}]-", Name, Relationship);

			return this;
		}

		public CypherMatch From(string Name, string Relationship, int MinHops, int MaxHops)
		{
			_sb.AppendFormat("<-[{0}:{1}*{2}..{3}]-", Name, Relationship, MinHops, MaxHops);

			return this;
		}

		#endregion

		#region Path

		public CypherMatch SortestPath(string Name, string FromNodeName, string ToNodeName, int MaxHops)
		{
			_sb.AppendFormat("{0}=shortestPath({1}-[*..{2}]->{3))", Name, FromNodeName, ToNodeName, MaxHops);

			return this;
		}

		public CypherMatch Path(string Name, string FromNodeName, string ToNodeName)
		{
			_sb.AppendFormat("{0}={1}-->{2))", Name, FromNodeName, ToNodeName);

			return this;
		}

		#endregion

		public ICypherObject Append(string value)
		{
			_sb.Append(value);

			return this;
		}

		public ICypherObject Append(string Format, params object[] args)
		{
			_sb.Append(string.Format(Format, args));

			return this;
		}

		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
