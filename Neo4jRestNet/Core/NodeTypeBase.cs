using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.Core
{
	public class NodeTypeBase : PropertyBase, IEquatable<string>
	{
		private static List<string> _Values = new List<string>();
		protected new string _value;

		public static readonly NodeTypeBase Type = new NodeTypeBase("_Type");

		protected NodeTypeBase()
		{
		}

		protected NodeTypeBase(string Value)
		{
			this._value = Value;
		}

		public static void Initialize()
		{
			Init(Type);
		}

		protected static void Init(NodeTypeBase initValue)
		{
			_Values.Add(initValue._value);
		}

		public static implicit operator NodeTypeBase(string Value)
		{
			if (_Values.Contains(Value))
			{
				return new NodeTypeBase(Value);
			}
			else
			{
				throw new Exception(string.Format("Value ({0}) is not a valid NodeTypeBase", Value));
			}
		}

		public static implicit operator string(NodeTypeBase Value)
		{
			return Value.ToString();
		}

		public override string ToString()
		{
			return this._value.ToString();
		}

		public static bool TryParse(string inValue, out NodeTypeBase outValue)
		{
			if (inValue == null)
			{
				outValue = null;
				return false;
			}

			if (_Values.Contains(inValue))
			{
				outValue = inValue;
				return true;
			}
			else
			{
				outValue = null;
				return false;
			}
		}

		#region IEquatable<string> Members

		public bool Equals(NodeTypeBase other)
		{
			if (ReferenceEquals(other, null))
				return false;
			else if (ReferenceEquals(this, other))
				return true;
			else if (this._value == null || other._value == null)
				return false;
			else if (this._value.Equals(other._value))
				return true;

			return false;
		}

		public bool Equals(string other)
		{
			return (_Values.Contains(other) && this._value == other) ? true : false;
		}

		public override bool Equals(Object obj)
		{
			return Equals(obj as NodeTypeBase);
		}

		public override int GetHashCode()
		{
			int index;
			for (index = 0; index < _Values.Count() && !_Values[index].Equals(this._value); index++) ;
			return index;
		}

		public static bool operator ==(NodeTypeBase Value1, NodeTypeBase Value2)
		{
			if (object.ReferenceEquals(Value1, Value2))
			{
				return true;
			}
			if (object.ReferenceEquals(Value1, null)) // Value2=null is covered by Equals
			{
				return false;
			}
			return Value1.Equals(Value2);
		}

		public static bool operator !=(NodeTypeBase Value1, NodeTypeBase Value2)
		{
			if (object.ReferenceEquals(Value1, Value2))
			{
				return false;
			}
			if (object.ReferenceEquals(Value1, null)) // Value2=null is covered by Equals
			{
				return false;
			}
			return !Value1.Equals(Value2);
		}

		#endregion
	}
}
