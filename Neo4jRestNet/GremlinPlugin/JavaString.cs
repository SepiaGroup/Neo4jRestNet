﻿using System.Text;

namespace Neo4jRestNet.GremlinPlugin
{
	public class JavaString : IJavaObject
	{
		private StringBuilder _sb = new StringBuilder();

		public JavaString()
		{
		}

		public JavaString(IJavaObject javaObject)
		{
			_sb.Append(javaObject.ToString());
		}


		public JavaString Append(string query)
		{
			if (_sb.Length == 0)
			{
				_sb.Append("it");
			}

			_sb.Append(".").Append(query);

			return this;
		}

		public JavaString Append(string format, params object[] args)
		{
			if (_sb.Length == 0)
			{
				_sb.Append("it");
			}

			_sb.Append(".").Append(string.Format(format, args));

			return this;
		}

		public override string ToString()
		{
			return _sb.ToString();
		}

		public static bool operator ==(JavaString js, object other)
		{
			return false;
		}

		public static bool operator ==(object other, JavaString js)
		{
			return false;
		}

		public static bool operator !=(JavaString js, object other)
		{
			return false;
		}

		public static bool operator !=(object other, JavaString js)
		{
			return false;
		}

		public static bool operator !(JavaString js)
		{
			return false;
		}

		public static implicit operator bool(JavaString js)
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
