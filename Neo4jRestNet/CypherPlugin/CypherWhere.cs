using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherWhere : ICypherObject, IComparable
	{
		private StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherWhere Node(string Name) 
		{
			_sb.AppendFormat("{0}", Name);

			return this;
		}

		#endregion

		#region Relationship

		public CypherWhere Relationship(string Name)
		{
			_sb.AppendFormat("{0}", Name);

			return this;
		}

		#endregion

		#region RegEx

		public CypherWhere RegEx(string regEx)
		{
			_sb.AppendFormat(" =~ /{0}/", regEx);

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
		
		public static bool operator ==(CypherWhere cypherWhere, object other)
		{
			return false;
		}

		public static bool operator !=(CypherWhere cypherWhere, object other)
		{
			return false;
		}

		public static bool operator !(CypherWhere cypherWhere)
		{
			return false;
		}

		public static implicit operator bool(CypherWhere cypherWhere)
		{
			return true;
		}

		public static bool operator <(CypherWhere o1, object o2)
		{
			return false;
		}
		public static bool operator >(CypherWhere o1, object o2)
		{
			return false;
		}  

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
	}
}
