using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.GremlinPlugin
{
	public class JavaObject : IJavaObject
	{
		private StringBuilder _sb = new StringBuilder();

		public JavaObject()
		{
		}

		public JavaObject(IJavaObject javaObject)
		{
			_sb.Append(javaObject.ToString());
		}

		public JavaObject Append(string query)
		{
			if (_sb.Length == 0)
			{
				_sb.Append("it");
			}

			_sb.Append(".").Append(query);

			return this;
		}

		public JavaObject Append(string Format, params object[] args)
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

		public static bool operator ==(JavaObject jo, object other)
		{
			return false;
		}

		public static bool operator ==(object other, JavaObject jo)
		{
			return false;
		}

		public static bool operator !=(JavaObject jo, object other)
		{
			return false;
		}

		public static bool operator !=(object other, JavaObject jo)
		{
			return false;
		}

		public static bool operator !(JavaObject jo)
		{
			return false;
		}

		public static implicit operator bool(JavaObject jo)
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
