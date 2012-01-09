using System.Text;
using System.Collections.Generic;
using System;
using Neo4jRestNet.Core;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherReturn : ICypherObject
	{

		private readonly StringBuilder _sb = new StringBuilder();
		private bool _isStringEmpty = true; // used becuase string could contain Distinct 
		private readonly List<Type> _returnTypes = new List<Type>();

		#region Node

		public CypherReturn Node(string name)
		{
			_sb.AppendFormat("{1} {0}", name, _isStringEmpty ? string.Empty : ",");
			
			_isStringEmpty = false;

			_returnTypes.Add(typeof(Node));

			return this;
		}
		
		public CypherReturn Node(string name, string property)
		{
			return Node(name, property, typeof (object));
		}

		public CypherReturn Node(string name, string property, Type propertyType)
		{
			_sb.AppendFormat("{2} {0}.{1}", name, property, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(propertyType);

			return this;
		}

		#endregion

		#region Relationship

		public CypherReturn Relationship(string name)
		{
			_sb.AppendFormat("{1} {0}", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(Relationship));

			return this;
		}

		public CypherReturn Relationship(string name, string property)
		{
			return Relationship(name, property, typeof(object));
		}

		public CypherReturn Relationship(string name, string property, Type propertyType)
		{
			_sb.AppendFormat("{2} {0}.{1}", name, property, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(propertyType);

			return this;
		}

		#endregion

		#region Path

		public CypherReturn Path(string name)
		{
			_sb.AppendFormat("{1} {0}", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(Path));

			return this;
		}

		#endregion

		#region Length

		public CypherReturn Length(string name)
		{
			_sb.AppendFormat("{1} length({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(int));

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

		#region Count
		
		public CypherReturn Count(string name)
		{
			_sb.AppendFormat("{1} count({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(int));

			return this;
		}
		
		#endregion

		#region Type
		
		public CypherReturn Type(string name)
		{
			_sb.AppendFormat("{1} type({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(string));

			return this;
		}
		
		#endregion

		#region Id

		public CypherReturn Id(string name)
		{
			_sb.AppendFormat("{1} id({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(long));

			return this;
		}

		#endregion
	
		#region Sum
		
		public CypherReturn Sum(string name)
		{
			_sb.AppendFormat("{1} sum({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(string));

			return this;
		}
		
		#endregion

		#region Avg

		public CypherReturn Avg(string name)
		{
			_sb.AppendFormat("{1} avg({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(float));

			return this;
		}

		#endregion

		#region Max

		public CypherReturn Max(string name)
		{
			_sb.AppendFormat("{1} max({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(float));

			return this;
		}

		#endregion

		#region Min

		public CypherReturn Min(string name)
		{
			_sb.AppendFormat("{1} min({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			_returnTypes.Add(typeof(float));

			return this;
		}

		#endregion

		public IEnumerable<Type> GetReturnTypes
		{
			get { return _returnTypes; }
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
