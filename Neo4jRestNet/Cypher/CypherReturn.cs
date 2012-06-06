﻿using System.Text;
using System.Collections.Generic;
using System;
using Neo4jRestNet.Core;

namespace Neo4jRestNet.Cypher
{
	public class CypherReturn : ICypherObject
	{

		private readonly StringBuilder _sb = new StringBuilder();
		private bool _isStringEmpty = true; // used becuase string could contain Distinct 
		internal readonly List<Type> ReturnTypes = new List<Type>();

		#region Node

		public CypherReturn Node(string name)
		{
			_sb.AppendFormat("{1} {0}", name, _isStringEmpty ? string.Empty : ",");
			
			_isStringEmpty = false;

			ReturnTypes.Add(typeof(Node));

			return this;
		}

		#endregion

		#region Relationship

		public CypherReturn Relationship(string name)
		{
			_sb.AppendFormat("{1} {0}", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(Relationship));

			return this;
		}

		#endregion

		#region Path

		public CypherReturn Path(string name)
		{
			_sb.AppendFormat("{1} {0}", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(Path));

			return this;
		}

		#endregion

		#region Length

		public CypherReturn Length(string name)
		{
			_sb.AppendFormat("{1} length({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(int));

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

		#region As

		public CypherReturn As(string alias)
		{
			_sb.AppendFormat(" as {0}", alias);

			_isStringEmpty = false;

			return this;
		}

		#endregion

		#region Count

		public CypherReturn Count()
		{
			_sb.AppendFormat("{0} count('*')", _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(int));

			return this;
		}

		public CypherReturn Count(string name)
		{
			_sb.AppendFormat("{1} count({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(int));

			return this;
		}

		public CypherReturn Count(string name, bool distinct)
		{
			_sb.AppendFormat("{1} count({2}{0})", name, _isStringEmpty ? string.Empty : ",", distinct ? "distinct " : string.Empty);

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(int));

			return this;
		}

		public CypherReturn Coalesce<T>(string elements)
		{
			_sb.AppendFormat("{1} Coalesce({0})", elements, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(T));

			return this;
		}

		#endregion

		#region Type
		
		public CypherReturn Type(string name)
		{
			_sb.AppendFormat("{1} type({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(string));

			return this;
		}
		
		#endregion

		#region Id

		public CypherReturn Id(string name)
		{
			_sb.AppendFormat("{1} id({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(long));

			return this;
		}

		#endregion
	
		#region Sum
		
		public CypherReturn Sum(string name)
		{
			_sb.AppendFormat("{1} sum({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(string));

			return this;
		}
		
		#endregion

		#region Avg

		public CypherReturn Avg(string name)
		{
			_sb.AppendFormat("{1} avg({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(float));

			return this;
		}

		#endregion

		#region Max

		public CypherReturn Max(string name)
		{
			_sb.AppendFormat("{1} max({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(float));

			return this;
		}

		#endregion

		#region Min

		public CypherReturn Min(string name)
		{
			_sb.AppendFormat("{1} min({0})", name, _isStringEmpty ? string.Empty : ",");

			_isStringEmpty = false;

			ReturnTypes.Add(typeof(float));

			return this;
		}

		#endregion

		public IEnumerable<Type> GetReturnTypes
		{
			get { return ReturnTypes; }
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
