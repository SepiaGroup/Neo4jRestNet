using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.Core.CypherQuery
{
	public class CypherDelete
	{
		private readonly StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherDelete Node(string name)
		{
			Append("{0}", name);

			return this;
		}

		public CypherDelete Node(string name, string propertyName)
		{
			Append("{0}.{1}", name, propertyName);

			return this;
		}

		public CypherDelete Node(string name, IEnumerable<string> propertyName)
		{
			Append("{0}", propertyName.Aggregate(new StringBuilder(), (sb, propName) => sb.AppendFormat("{0}{1}.{2}", sb.Length == 0 ? string.Empty : ",", name, propName)));

			return this;
		}


		public CypherDelete Node(string name, Enum propertyName)
		{
			Append("{0}.{1}", name, propertyName.ToString());

			return this;
		}

		public CypherDelete Node(string name, IEnumerable<Enum> propertyName)
		{
			Append("{0}", propertyName.Aggregate(new StringBuilder(), (sb, propName) => sb.AppendFormat("{0}{1}.{2}", sb.Length == 0 ? string.Empty : ",", name, propName.ToString())));

			return this;
		}

		#endregion

		#region Relationship

		public CypherDelete Relationship(string name)
		{
			Append("{0}", name);

			return this;
		}

		public CypherDelete Relationship(string name, string propertyName)
		{
			Append("{0}.{1}", name, propertyName);

			return this;
		}

		public CypherDelete Relationship(string name, IEnumerable<string> propertyName)
		{
			Append("{0}", propertyName.Aggregate(new StringBuilder(), (sb, propName) => sb.AppendFormat("{0}{1}.{2}", sb.Length == 0 ? string.Empty : ",", name, propName)));

			return this;
		}


		public CypherDelete Relationship(string name, Enum propertyName)
		{
			Append("{0}.{1}", name, propertyName.ToString());

			return this;
		}

		public CypherDelete Relationship(string name, IEnumerable<Enum> propertyName)
		{
			Append("{0}", propertyName.Aggregate(new StringBuilder(), (sb, propName) => sb.AppendFormat("{0}{1}.{2}", sb.Length == 0 ? string.Empty : ",", name, propName.ToString())));

			return this;
		}

		#endregion

		private void Append(string format, params object[] args)
		{
			_sb.AppendFormat("{1}{0}", string.Format(format, args), _sb.Length == 0 ? " " : ",");
		}

		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
