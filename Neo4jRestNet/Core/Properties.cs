using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Neo4jRestNet.Core
{
	public class Properties
	{
		private readonly IDictionary<string, object> _properties;

		public Properties()
		{
			_properties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
		}

		public Properties(IDictionary<string, object> properties)
		{
			_properties = properties;
		}

		private static T Unbox<T>(object o)
		{

			Type t = typeof(T);
			Type u = Nullable.GetUnderlyingType(t);

			if (u != null)
			{
				if (o == null)
					return default(T);

				return (T)Convert.ChangeType(o, u);
			}
			
			return (T)Convert.ChangeType(o, t);
		}

		public T GetProperty<T>(string key)
		{
			if (!_properties.Keys.Contains(key)) throw new Exception(string.Format("Failed to retrieve property {0}", key));
			object property = _properties[key];
			if (property != null && property is ICollection)
			{
				throw new Exception("Do not support retrieving of arrays");
				// return (T)property;
			}
			return Unbox<T>(property);
		}

		public object GetProperty(string key)
		{
			if (!_properties.Keys.Contains(key)) throw new Exception(string.Format("Failed to retrieve property {0}", key));
			object property = _properties[key];
			if (property != null && property is ICollection)
			{
				throw new Exception("Do not support retrieving of arrays");
				// return (T)property;
			}
			return property;
		}

		private object DefaultTypeValue<T>()
		{
			if (typeof(T) == typeof(string))
			{
				return string.Empty;
			}
			
			if (typeof(T) == typeof(int))
			{
				return 0;
			}
			
			if (typeof(T) == typeof(long))
			{
				return 0L;
			}
			
			if (typeof(T) == typeof(float))
			{
				return 0F;
			}
			
			if (typeof(T) == typeof(decimal))
			{
				return 0D;
			}
			
			if (typeof(T) == typeof(bool))
			{
				return false;
			}

			return null;
		}

		public T GetPropertyOrDefault<T>(string key)
		{
			if (_properties.Keys.Contains(key))
			{
				return Unbox<T>(_properties[key]);
			}

			return (T)DefaultTypeValue<T>();
		}

		public T GetPropertyOrOther<T>(string key, T otherValue)
		{

			if (_properties.Keys.Contains(key))
			{
				return Unbox<T>(_properties[key]);
			}

			return otherValue;
		}

		public IEnumerable<string> GetPropertyKeys()
		{
			return _properties.Keys;
		}

		public bool HasProperty(string key)
		{
			return _properties.Keys.Contains(key);
		}

		public object RemoveProperty(string key)
		{
			// Return old value
			object oldValue = _properties.Keys.Contains(key) ? _properties[key] : null;
			// Remove from properties
			_properties.Remove(key);

			return oldValue;
		}
		/*
				public void SetProperty<T>(PropertyBase key, T value)
				{
					_properties[key] = value;
				}
		*/
		public void SetProperty<T>(string key, T value)
		{
			_properties[key] = value;
		}

		public static Properties ParseJson(string jsonProperties)
		{
			if (String.IsNullOrEmpty(jsonProperties))
			{
				return null;
			}

			var properties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

			JObject joProperties = JObject.Parse(jsonProperties);

			foreach (var kvpProperty in joProperties)
			{
				properties.Add(kvpProperty.Key, ParseToken(kvpProperty.Value));
			}

			return new Properties(properties);
		}

		public override string ToString()
		{
			var jo = new JObject();

			foreach (var kvp in _properties.Where(kvp => kvp.Value != null))
			{
				jo.Add(kvp.Key, JToken.FromObject(kvp.Value));
			}
			return jo.ToString(Formatting.None);
		}

		public Dictionary<string, object> ToDictionary()
		{
			return (Dictionary<string, object>)_properties;
		}

		private static object ParseToken(JToken token)
		{
			switch (token.Type)
			{
				//				case JTokenType.Object:
				//					return Parse((JObject)token);
				//				case JTokenType.Array:
				//					return ParseArray(token);
				case JTokenType.Integer:
					return token.Value<int>();
				case JTokenType.Float:
					return token.Value<float>();
				case JTokenType.String:
					return token.Value<string>();
				case JTokenType.Boolean:
					return token.Value<bool>();
				case JTokenType.Date:
					return token.Value<DateTime>();
				case JTokenType.Raw:
					return token.Value<byte[]>();
				case JTokenType.Bytes:
					return token.Value<byte[]>();
				case JTokenType.Null:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}