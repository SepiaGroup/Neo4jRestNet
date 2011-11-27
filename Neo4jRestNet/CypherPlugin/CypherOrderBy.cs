using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherOrderBy : ICypherObject
	{

		private StringBuilder _sb = new StringBuilder();
		private bool _IsStringEmpty = true; // used becuase string could contain Distinct 

		#region Node

		public CypherOrderBy Node(string Name, string Property)
		{
			_sb.AppendFormat("{0} {1}.{2}", _IsStringEmpty ? string.Empty : ",", Name, Property);

			_IsStringEmpty = false;

			return this;
		}

		public CypherOrderBy Node(string Name, string Property, bool Decending)
		{
			_sb.AppendFormat("{0} {1}.{2} {3}", _IsStringEmpty ? string.Empty : ",", Name, Property, Decending ? "DESC" : string.Empty);

			_IsStringEmpty = false;

			return this;
		}

		#endregion

		#region CypherRelationship

		public CypherOrderBy Relationship(string Name, string Property)
		{
			_sb.AppendFormat("{0} {1}.{2}", _IsStringEmpty ? string.Empty : ",", Name, Property);

			_IsStringEmpty = false;

			return this;
		}

		public CypherOrderBy Relationship(string Name, string Property, bool Decending)
		{
			_sb.AppendFormat("{0} {1}.{2} {3}", _IsStringEmpty ? string.Empty : ",", Name, Property, Decending ? "DESC" : string.Empty);

			_IsStringEmpty = false;

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
