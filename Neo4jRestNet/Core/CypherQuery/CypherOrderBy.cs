using System;
using System.Text;

namespace Neo4jRestNet.Core.CypherQuery
{
	public class CypherOrderBy : ICypherObject
	{
		public enum Order
		{ 
			Acending,
			Decending
		}

		private readonly StringBuilder _sb = new StringBuilder();
		private bool _isStringEmpty = true; // used becuase string could contain Distinct 

		#region Node

		public CypherOrderBy Node(string name, string property)
		{
			return OrderByClause(name, property, Order.Acending, false);
		}

		public CypherOrderBy Node(string name, Enum property)
		{
			return OrderByClause(name, property.ToString(), Order.Acending, false);
		}
		
		public CypherOrderBy Node(string name, Enum property, bool optional)
		{
			return OrderByClause(name, property.ToString(), Order.Acending, optional);
		}

		public CypherOrderBy Node(string name, string property, Order order)
		{
			return OrderByClause(name, property, Order.Acending, false);
		}

		public CypherOrderBy Node(string name, string property, Order order, bool optional)
		{
			return OrderByClause(name, property, Order.Acending, optional);
		}
		
		public CypherOrderBy Node(string name, Enum property, Order order)
		{
			return OrderByClause(name, property.ToString(), order, false);
		}

		public CypherOrderBy Node(string name, Enum property, Order order, bool optional)
		{
			return OrderByClause(name, property.ToString(), order, optional);
		}

		#endregion

		#region CypherRelationship

		public CypherOrderBy Relationship(string name, string property)
		{
			return OrderByClause(name, property, Order.Acending, false);
		}

		public CypherOrderBy Relationship(string name, Enum property)
		{
			return OrderByClause(name, property.ToString(), Order.Acending, false);
		}

		public CypherOrderBy Relationship(string name, Enum property, bool optional)
		{
			return OrderByClause(name, property.ToString(), Order.Acending, optional);
		}

		public CypherOrderBy Relationship(string name, string property, Order order)
		{
			return OrderByClause(name, property, Order.Acending, false);
		}

		public CypherOrderBy Relationship(string name, string property, Order order, bool optional)
		{
			return OrderByClause(name, property, Order.Acending, optional);
		}

		public CypherOrderBy Relationship(string name, Enum property, Order order)
		{
			return OrderByClause(name, property.ToString(), order, false);
		}

		public CypherOrderBy Relationship(string name, Enum property, Order order, bool optional)
		{
			return OrderByClause(name, property.ToString(), order, optional);
		}

		#endregion

		private CypherOrderBy OrderByClause(string name, string property, Order order, bool optional)
		{
			_sb.AppendFormat("{0} {1}.{2}{4} {3}", _isStringEmpty ? string.Empty : ",", name, property, order == Order.Decending ? "DESC" : string.Empty, optional ? "?" : string.Empty);

			_isStringEmpty = false;

			return this;
		}

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
