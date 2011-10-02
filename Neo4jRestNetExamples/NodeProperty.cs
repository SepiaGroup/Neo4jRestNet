using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Core;

namespace Example
{
	public class NodeProperty : NodePropertyBase, IEquatable<string>
	{

		private static List<string> _Values = new List<string>();
		protected new string _value;

		public static readonly NodeProperty FirstName = new NodeProperty("FirstName");
		public static readonly NodeProperty LastName = new NodeProperty("LastName");
		public static readonly NodeProperty Password = new NodeProperty("PWD");
		public static readonly NodeProperty UserID = new NodeProperty("UID");

		protected NodeProperty(string Value)
		{
			this._value = Value;
		}

		public static new void Initialize()
		{
			Init(FirstName);
			Init(LastName);
			Init(Password);
		}

		protected static void Init(NodeProperty initValue)
		{
			_Values.Add(initValue._value);
		}

		public static implicit operator NodeProperty(string Value)
		{
			if (_Values.Contains(Value))
			{
				return new NodeProperty(Value);
			}
			else
			{
				throw new Exception(string.Format("Value ({0}) is not a valid NodeProperty", Value));
			}
		}

		public static implicit operator string(NodeProperty Value)
		{
			return Value.ToString();
		}

		public override string ToString()
		{
			return this._value.ToString();
		}

		public static bool TryParse(string inValue, out NodeProperty outValue)
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

		public bool Equals(NodeProperty other)
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

		public new bool Equals(string other)
		{
			return (_Values.Contains(other) && this._value == other) ? true : false;
		}

		public override bool Equals(Object obj)
		{
			return Equals(obj as NodeProperty);
		}

		public override int GetHashCode()
		{
			int index;
			for (index = 0; index < _Values.Count() && !_Values[index].Equals(this._value); index++) ;
			return index;
		}

		public static bool operator ==(NodeProperty Value1, NodeProperty Value2)
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

		public static bool operator !=(NodeProperty Value1, NodeProperty Value2)
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
