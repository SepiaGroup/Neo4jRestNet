using System;
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

		public CypherMatch Any()
		{
			_sb.Append("--");

			return this;
		}

		public CypherMatch Any(bool optional)
		{
			_sb.Append(optional ? "-[?]-" : "--");

			return this;
		}
		
		public CypherMatch Any(string name)
		{
			_sb.AppendFormat("-[{0}]-", name);

			return this;
		}

		public CypherMatch Any(Enum name)
		{
			return Any(name.ToString());
		}

		public CypherMatch Any(string name, string relationship)
		{
			if(string.IsNullOrWhiteSpace(relationship))
			{
				_sb.AppendFormat("-[{0}]-", name);
			}
			else
			{
				_sb.AppendFormat("-[{0}:{1}]-", name, relationship);	
			}

			return this;
		}

		public CypherMatch Any(string name, Enum relationship)
		{
			return Any(name, relationship.ToString());
		}

		public CypherMatch Any(string name, string relationship, int minHops, int maxHops)
		{
			_sb.AppendFormat("-[{0}:{1}*{2}..{3}]-", name, relationship, minHops, maxHops);

			return this;
		}
		
		public CypherMatch Any(string name, Enum relationship, int minHops, int maxHops)
		{
			return Any(name, relationship.ToString(), minHops, maxHops);
		}

		#endregion

		#region To

		public CypherMatch To()
		{
			_sb.Append("-->");

			return this;
		}

		public CypherMatch To(bool optional)
		{
			_sb.Append(optional ? "-[?]->" : "-->");

			return this;
		}

		public CypherMatch To(string relationship)
		{
			_sb.AppendFormat("-[:{0}]->", relationship);

			return this;
		}

		public CypherMatch To(Enum relationship)
		{
			return To(relationship.ToString());
		}

		public CypherMatch To(string relationship, bool optional)
		{
			_sb.AppendFormat("-[{1}:{0}]->", relationship, optional ? "?" : string.Empty);

			return this;
		}
		
		public CypherMatch To(Enum relationship, bool optional)
		{
			return To(relationship.ToString(), optional);
		}

		public CypherMatch To(string name, string relationship)
		{
			if (string.IsNullOrWhiteSpace(relationship))
			{
				_sb.AppendFormat("-[{0}]->", name);
			}
			else
			{
				_sb.AppendFormat("-[{0}:{1}]->", name, relationship);
			}

			return this;
		}
		
		public CypherMatch To(string name, Enum relationship)
		{
			return To(name, relationship.ToString());
		}

		public CypherMatch To(string name, string relationship, bool optional)
		{
			_sb.AppendFormat("-[{0}{2}:{1}]->", name, relationship, optional ? "?" : string.Empty);
			
			return this;
		}

		public CypherMatch To(string name, Enum relationship, bool optional)
		{
			return To(name, relationship.ToString(), optional);
		}

		public CypherMatch To(string name, string relationship, int minHops, int maxHops, bool optional)
		{
			_sb.AppendFormat("-[{0}{4}:{1}*{2}..{3}]->", name, relationship, minHops, maxHops, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch To(string name, Enum relationship, int minHops, int maxHops, bool optional)
		{
			return To(name, relationship.ToString(), minHops, maxHops, optional);
		}

		#endregion

		#region From

		public CypherMatch From()
		{
			_sb.Append("<--");

			return this;
		}

		public CypherMatch From(bool optional)
		{
			_sb.Append(optional ? "<-[?]-" : "<--");

			return this;
		}
		
		public CypherMatch From(string relationship)
		{
			_sb.AppendFormat("<-[:{0}]-", relationship);

			return this;
		}

		public CypherMatch From(Enum relationship)
		{
			return From(relationship.ToString());
		}

		public CypherMatch From(string relationship, bool optional)
		{
			_sb.AppendFormat("<-[{1}:{0}]-", relationship, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch From(Enum relationship, bool optional)
		{
			return From(relationship.ToString(), optional);
		}

		public CypherMatch From(string name, string relationship)
		{
			if (string.IsNullOrWhiteSpace(relationship))
			{
				_sb.AppendFormat("<-[{0}]-", name);
			}
			else
			{
				_sb.AppendFormat("<-[{0}:{1}]-", name, relationship);
			}
			
			return this;
		}

		public CypherMatch From(string name, Enum relationship)
		{
			return From(name, relationship.ToString());
		}

		public CypherMatch From(string name, string relationship, bool optional)
		{
			_sb.AppendFormat("<-[{0}{2}:{1}]-", name, relationship, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch From(string name, Enum relationship, bool optional)
		{
			return From(name, relationship.ToString(), optional);
		}

		public CypherMatch From(string name, string relationship, int minHops, int maxHops)
		{
			_sb.AppendFormat("<-[{0}:{1}*{2}..{3}]-", name, relationship, minHops, maxHops);

			return this;
		}

		public CypherMatch From(string name, Enum relationship, int minHops, int maxHops)
		{
			return From(name, relationship.ToString(), minHops, maxHops);
		}
		
		public CypherMatch From(string name, string relationship, int minHops, int maxHops, bool optional)
		{
			_sb.AppendFormat("<-[{0}{4}:{1}*{2}..{3}]-", name, relationship, minHops, maxHops, optional ? "?" : string.Empty);

			return this;
		}

		public CypherMatch From(string name, Enum relationship, int minHops, int maxHops, bool optional)
		{
			return From(name, relationship.ToString(), minHops, maxHops, optional);
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

		public CypherMatch Path(string name, string fromNodeName, string relationship, string toNodeName)
		{
			_sb.AppendFormat("{0}={1}-[:{2}]->{3})", name, fromNodeName, relationship, toNodeName);

			return this;
		}

		public CypherMatch Path(string name, string fromNodeName, Enum relationship, string toNodeName)
		{
			return Path(name, fromNodeName, relationship.ToString(), toNodeName);
		}

		public CypherMatch Path(string name, string fromNodeName, string relationship, int minHops, int maxHops, string toNodeName)
		{
			_sb.AppendFormat("{0}={1}-[:{2}*{3}..{4}]->{5})", name, fromNodeName, relationship, minHops, maxHops, toNodeName);

			return this;
		}

		public CypherMatch Path(string name, string fromNodeName, Enum relationship, int minHops, int maxHops, string toNodeName)
		{
			return Path(name, fromNodeName, relationship.ToString(), minHops, maxHops, toNodeName);
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
