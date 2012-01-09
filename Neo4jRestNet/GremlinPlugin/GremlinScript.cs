using System.Linq;
using System.Text;
using Neo4jRestNet.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.GremlinPlugin
{
	public class GremlinScript : IJavaObject
	{
		private StringBuilder _sb = new StringBuilder();
		private List<object> _parameters = new List<object>();

		public GremlinScript()
		{
		}

		public GremlinScript(Node node)
		{
			Append("g.v({0})", node.Id);
			// _sb.AppendFormat("g.v({0})", node.Id);
		}

		public GremlinScript(Relationship relationship)
		{
			Append("g.e({0})", relationship.Id);
			//_sb.AppendFormat("g.e({0})", relationship.Id);
		}

		public GremlinScript(IJavaObject javaObject)
		{
			_sb.Append(javaObject.ToString());
		}

		public GremlinScript Append(string query, bool prepend = true)
		{
			if (prepend)
			{
				return Append(query);
			}

			_sb.Append(query);

			return this;
		}

		public GremlinScript Append(string query)
		{
			if (_sb.Length == 0 && !string.IsNullOrWhiteSpace(query) && query.StartsWith("."))
			{
				_sb.Append("it");
			}

			_sb.Append(query);

			return this; 
		}

		public GremlinScript Append(string format, params object[] parameters)
		{
			var startIndex = _parameters.Count;

			_parameters.AddRange(parameters);

			var parameterNames = parameters.Select((p, index) => string.Concat("p", startIndex + index)).ToArray<object>();

			_sb.Append(string.Format(format, parameterNames));

			return this;
		}
/*
		public override string ToString()
		{
			return _sb.ToString();
		}
*/
		public string GetScript()
		{
			var joScript = new JObject{{"script", _sb.ToString()}};

			if (_parameters.Any())
			{
				var count = 0;
				var joParams = new JObject();
				foreach (var p in _parameters)
				{
					joParams.Add(string.Format("p{0}", count++), JToken.FromObject(p));
				}

				joScript.Add("params", joParams);
			}

			return joScript.ToString(Formatting.None);
		}

		public static bool operator ==(GremlinScript gs, object other)
		{
			return false;
		}

		public static bool operator ==(object other, GremlinScript gs)
		{
			return false;
		}

		public static bool operator !=(GremlinScript gs, object other)
		{
			return false;
		}

		public static bool operator !=(object other, GremlinScript gs)
		{
			return false;
		}

		public static bool operator !(GremlinScript gs)
		{
			return false;
		}

		public static implicit operator bool(GremlinScript gs)
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
