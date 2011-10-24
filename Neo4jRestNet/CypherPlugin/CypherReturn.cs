using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherReturn : ICypherObject
	{

		private StringBuilder _sb = new StringBuilder();
		private bool _IsStringEmpty = true; // used becuase string could contain Distinct 

		#region Node

		public CypherReturn Node(string Name)
		{
			_sb.AppendFormat("{1} {0}", Name, _IsStringEmpty ? string.Empty : ",");
			
			_IsStringEmpty = false;

			return this;
		}

		#endregion

		#region CypherReturn

		public CypherReturn Relationship(string Name)
		{
			_sb.AppendFormat("{1} {0}", Name, _IsStringEmpty ? string.Empty : ",");

			_IsStringEmpty = false;

			return this;
		}

		#endregion

		#region Path

		public CypherReturn Path(string Name)
		{
			_sb.AppendFormat("{1} {0}", Name, _IsStringEmpty ? string.Empty : ",");

			_IsStringEmpty = false;

			return this;
		}

		#endregion

		#region Distinct

		public CypherReturn Distinct()
		{
			_sb.Append(" distinct ");

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
