using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Core;

namespace Neo4jRestNet.GremlinPlugin
{
	public class GremlinScript : IJavaObject
	{
		private StringBuilder _sb = new StringBuilder();

		public GremlinScript()
		{
		}

		public GremlinScript(Node node)
		{
			_sb.AppendFormat("g.v({0})", node.Id);
		}

		public GremlinScript(Relationship relationship)
		{
			_sb.AppendFormat("g.e({0})", relationship.Id);
		}

		public GremlinScript(IJavaObject javaObject)
		{
			_sb.Append(javaObject.ToString());
		}
	
		public GremlinScript Append(string query)
		{
			if (_sb.Length == 0 && !string.IsNullOrWhiteSpace(query) && query.StartsWith("."))
			{
				_sb.Append("it");
			}

			_sb.Append(query);

			return this; 
		}

		public GremlinScript Append(string Format, params object[] args)
		{
			_sb.Append(string.Format(Format, args));

			return this;
		}

		public override string ToString()
		{
			return _sb.ToString();
		}

		public static bool operator ==(GremlinScript gs, object other)
		{
			return false;
		}

		public static bool operator ==(object other, GremlinScript gs)
		{
			return false;
		}

		public static bool operator !=(GremlinScript gs, object other)
		{
			return false;
		}

		public static bool operator !=(object other, GremlinScript gs)
		{
			return false;
		}

		public static bool operator !(GremlinScript gs)
		{
			return false;
		}

		public static implicit operator bool(GremlinScript gs)
		{
			return true;
		}
		
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
