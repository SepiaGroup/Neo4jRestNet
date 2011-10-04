using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.Core
{
	public class PropertyBase 
	{
		protected string _value;

		protected PropertyBase()
		{
		}

		protected PropertyBase(string Value)
		{
			this._value = Value;
		}

		public static implicit operator PropertyBase(string Value)
		{
			return new PropertyBase(Value);
		}

		public static implicit operator string(PropertyBase Value)
		{
			return Value.ToString();
		}

		public override string ToString()
		{
			return this._value.ToString();
		}

	}
}
