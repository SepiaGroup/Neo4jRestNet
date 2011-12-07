using System;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherOrderBy : ICypherObject
	{

		private readonly StringBuilder _sb = new StringBuilder();
		private bool _isStringEmpty = true; // used becuase string could contain Distinct 

		#region Node

		public CypherOrderBy Node(string name, string property)
		{
			_sb.AppendFormat("{0} {1}.{2}", _isStringEmpty ? string.Empty : ",", name, property);

			_isStringEmpty = false;

			return this;
		}

		public CypherOrderBy Node(string name, Enum property)
		{
			return Node(name, property.ToString());
		}
		
		public CypherOrderBy Node(string name, string property, bool decending)
		{
			_sb.AppendFormat("{0} {1}.{2} {3}", _isStringEmpty ? string.Empty : ",", name, property, decending ? "DESC" : string.Empty);

			_isStringEmpty = false;

			return this;
		}

		public CypherOrderBy Node(string name, Enum property, bool decending)
		{
			return Node(name, property.ToString(), decending);
		}

		#endregion

		#region CypherRelationship

		public CypherOrderBy Relationship(string name, string property)
		{
			_sb.AppendFormat("{0} {1}.{2}", _isStringEmpty ? string.Empty : ",", name, property);

			_isStringEmpty = false;

			return this;
		}

		public CypherOrderBy Relationship(string name, Enum property)
		{
			return Relationship(name, property.ToString());
		}

		public CypherOrderBy Relationship(string name, string property, bool decending)
		{
			_sb.AppendFormat("{0} {1}.{2} {3}", _isStringEmpty ? string.Empty : ",", name, property, decending ? "DESC" : string.Empty);

			_isStringEmpty = false;

			return this;
		}

		public CypherOrderBy Relationship(string name, Enum property, bool decending)
		{
			return Relationship(name, property.ToString(), decending);
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
