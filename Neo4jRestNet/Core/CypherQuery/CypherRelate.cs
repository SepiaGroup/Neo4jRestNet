using System;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.Core.CypherQuery
{
	public class CypherCreateUnique
	{
		private readonly StringBuilder _sb = new StringBuilder();

		#region Node

		public CypherCreateUnique Node(string name)
		{
			_sb.AppendFormat(" {0}", name);

			return this;
		}

		public CypherCreateUnique Node(string name, Properties properties)
		{
			_sb.AppendFormat(" ({0} {1})", name, properties.ToString(false));

			return this;
		}

		#endregion

		#region To

		public CypherCreateUnique To(string relationship, Properties properties = null)
		{
			if (properties == null)
			{
				_sb.AppendFormat("-[:{0}]->", relationship);
			}
			else
			{
				_sb.AppendFormat("-[:{0} {1}]->", relationship, properties.ToString(false));
			}

			return this;
		}

		public CypherCreateUnique To(Enum relationship, Properties properties = null)
		{
			return To(relationship.ToString(), properties);
		}

		public CypherCreateUnique To(string name, string relationship, Properties properties = null)
		{
			if (properties == null)
			{
				_sb.AppendFormat("-[{0}:{1}]->", name, relationship);
			}
			else
			{
				_sb.AppendFormat("-[{0}:{1} {2}]->", name, relationship, properties.ToString(false));
			}

			return this;
		}

		public CypherCreateUnique To(string name, Enum relationship, Properties properties = null)
		{
			return To(name, relationship.ToString(), properties);
		}

		#endregion

		#region From

		public CypherCreateUnique From(string relationship, Properties properties = null)
		{
			if (properties == null)
			{
				_sb.AppendFormat("<-[:{0}]-", relationship);
			}
			else
			{
				_sb.AppendFormat("<-[:{0} {1}]-", relationship, properties.ToString(false));
			}

			return this;
		}

		public CypherCreateUnique From(Enum relationship, Properties properties = null)
		{
			return From(relationship.ToString(), properties);
		}

		public CypherCreateUnique From(string name, string relationship, Properties properties = null)
		{
			if (properties == null)
			{
				_sb.AppendFormat("<-[{0}:{1}]-", name, relationship);
			}
			else
			{
				_sb.AppendFormat("<-[{0}:{1} {2}]-", name, relationship, properties.ToString(false));
			}

			return this;
		}

		public CypherCreateUnique From(string name, Enum relationship, Properties properties = null)
		{
			return From(name, relationship.ToString(), properties);
		}

		#endregion

		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}
