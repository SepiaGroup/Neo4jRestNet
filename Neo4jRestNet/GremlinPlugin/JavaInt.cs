using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.GremlinPlugin
{
	public class JavaInt : IJavaObject
	{
		private StringBuilder _sb = new StringBuilder();

		public JavaInt()
		{
		}

		public JavaInt(IJavaObject javaObject)
		{
			_sb.Append(javaObject.ToString());
		}


		public JavaInt Append(string query)
		{
			if (_sb.Length == 0)
			{
				_sb.Append("it");
			}

			_sb.Append(".").Append(query);

			return this;
		}

		public JavaInt Append(string Format, params object[] args)
		{
			if (_sb.Length == 0)
			{
				_sb.Append("it");
			}

			_sb.Append(".").Append(string.Format(Format, args));

			return this;
		}

		public override string ToString()
		{
			return _sb.ToString();
		}

		public static bool operator ==(JavaInt ji, object other)
		{
			return true;
		}

		public static bool operator ==(object other, JavaInt ji)
		{
			return true;
		}

		public static bool operator !=(JavaInt ji, object other)
		{
			return false;
		}

		public static bool operator !=(object other, JavaInt ji)
		{
			return false;
		}

		public static bool operator !(JavaInt ji)
		{
			return false;
		}

		public static implicit operator bool(JavaInt ji)
		{
			return true;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}
}
