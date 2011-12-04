using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherMatch : ICypherObject
	{

		private readonly StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherMatch Node()
		{
			return Node(string.Empty);
		}

		public CypherMatch Node(string name)
		{
			_sb.AppendFormat(" ({0}) ", name);

			return this;
		}

		#endregion

		#region Relationship

		public CypherMatch Relationship()
		{
			return Relationship(string.Empty);
		}

		public CypherMatch Relationship(string name)
		{
			_sb.AppendFormat(" ({0}) ", name);

			return this;
		}

		#endregion

		#region Any

		public CypherMatch Any(string name)
		{
			_sb.AppendFormat("-[{0}]-", name);

			return this;
		}

		public CypherMatch Any(string name, string relationship)
		{
			_sb.AppendFormat("-[{0}:{1}]-", name, relationship);

			return this;
		}

		public CypherMatch Any(string name, string relationship, int minHops, int maxHops)
		{
			_sb.AppendFormat("-[{0}:{1}*{2}..{3}]-", name, relationship, minHops, maxHops);

			return this;
		}

		#endregion

		#region To

		public CypherMatch To(string relationship)
		{
			_sb.AppendFormat("-[:{0}]->", relationship);

			return this;
		}
		
		public CypherMatch To(string relationship, bool optional)
		{
			_sb.AppendFormat("-[{1}:{0}]->", relationship, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch To(string name, string relationship)
		{
			_sb.AppendFormat("-[{0}:{1}]->", name, relationship);

			return this;
		}

		public CypherMatch To(string name, string relationship, bool optional)
		{
			_sb.AppendFormat("-[{0}{2}:{1}]->", name, relationship, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch To(string name, string relationship, int minHops, int maxHops, bool optional)
		{
			_sb.AppendFormat("-[{0}{4}:{1}*{2}..{3}]->", name, relationship, minHops, maxHops, optional ? "?" : string.Empty);

			return this;
		}

		#endregion

		#region From

		public CypherMatch From(string relationship)
		{
			_sb.AppendFormat("<-[:{0}]-", relationship);

			return this;
		}

		public CypherMatch From(string relationship, bool optional)
		{
			_sb.AppendFormat("<-[{1}:{0}]-", relationship, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch From(string name, string relationship)
		{
			_sb.AppendFormat("<-[{0}:{1}]-", name, relationship);

			return this;
		}

		public CypherMatch From(string name, string relationship, bool optional)
		{
			_sb.AppendFormat("<-[{0}{2}:{1}]-", name, relationship, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch From(string name, string relationship, int minHops, int maxHops)
		{
			_sb.AppendFormat("<-[{0}:{1}*{2}..{3}]-", name, relationship, minHops, maxHops);

			return this;
		}

		public CypherMatch From(string name, string relationship, int minHops, int maxHops, bool optional)
		{
			_sb.AppendFormat("<-[{0}{4}:{1}*{2}..{3}]-", name, relationship, minHops, maxHops, optional ? "?" : string.Empty);

			return this;
		}

		#endregion

		#region Path

		public CypherMatch SortestPath(string name, string fromNodeName, string toNodeName, int maxHops)
		{
			_sb.AppendFormat("{0}=shortestPath({1}-[*..{2}]->{3})", name, fromNodeName, maxHops, toNodeName);

			return this;
		}

		public CypherMatch Path(string name, string fromNodeName, string toNodeName)
		{
			_sb.AppendFormat("{0}={1}-->{2})", name, fromNodeName, toNodeName);

			return this;
		}

		#endregion

		public ICypherObject Append(string value)
		{
			_sb.Append(value);

			return this;
		}

		public ICypherObject Append(string format, params object[] args)
		{
			_sb.Append(string.Format(format, args));

			return this;
		}

		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
