﻿using System;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.Core.CypherQuery
{
	public class CypherWhere : ICypherObject, IComparable
	{
		private readonly StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherWhere Node()
		{
			_sb.Append("()");

			return this;
		}

		public CypherWhere Node(string name) 
		{
			_sb.AppendFormat("{0}", name);

			return this;
		}

		public CypherWhere NodeHas(string name, string propertyName)
		{
			_sb.Append(string.Format(" has({0}.{1}) ", name, propertyName));

			return this;
		}

		#endregion

		#region Relationship

		public CypherWhere Relationship(string name)
		{
			_sb.AppendFormat("{0}", name);

			return this;
		}

		public CypherWhere RelationshipHas(string name, string propertyName)
		{
			_sb.Append(string.Format(" has({0}.{1}) ", name, propertyName));

			return this;
		}

		#endregion

		#region RelationshipType

		public CypherWhere RelationshipType(string name)
		{
			_sb.AppendFormat("type({0})", name);

			return this;
		}

		#endregion

		#region RegEx

		public CypherWhere RegEx(string regEx)
		{
			_sb.AppendFormat(" =~ /{0}/", regEx);

			return this;
		}

		public CypherWhere RegEx(string regEx, bool caseInsensitive)
		{
			_sb.AppendFormat(" =~ /{0}{1}/", caseInsensitive ? "(?i)" : string.Empty, regEx);

			return this;
		}

		#endregion

		#region To

		public CypherWhere To()
		{
			_sb.Append("-->");

			return this;
		}

		public CypherWhere To(string relationship)
		{
			_sb.AppendFormat("-[:{0}]->", relationship);

			return this;
		}

		public CypherWhere To(Enum relationship)
		{
			return To(relationship.ToString());
		}

		#endregion

		#region From

		public CypherWhere From()
		{
			_sb.Append("<--");

			return this;
		}

		public CypherWhere From(string relationship)
		{
			_sb.AppendFormat("<-[:{0}]-", relationship);

			return this;
		}

		public CypherWhere From(Enum relationship)
		{
			return From(relationship.ToString());
		}

		#endregion

		#region In

		public CypherWhere In<T>(params T[] items)
		{
			_sb.Append(string.Format(" in [{0}]", 
				items.Aggregate(new StringBuilder(), (sb, item) => sb.AppendFormat("{1}{2}{0}{2}", item, sb.Length > 0 ? "," : string.Empty, typeof(T) == typeof(string) ? "'" : string.Empty))));
			
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

		public static bool operator <=(CypherWhere o1, object o2)
		{
			return false;
		}
		public static bool operator >=(CypherWhere o1, object o2)
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
