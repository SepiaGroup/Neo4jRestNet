using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Neo4jRestNet.Core;
using System.Net;

namespace Neo4jRestNet.Rest
{
	class Neo4jRestApi
	{
		public static HttpStatusCode GetRoot(string dbUrl, out string Response)
		{
			return HttpRest.Get(dbUrl, out Response);
		}

		public static HttpStatusCode CreateNode(string dbUrl, string jsonProperties, out string Response)
		{
			if (string.IsNullOrWhiteSpace(jsonProperties))
				return HttpRest.Post(string.Concat(dbUrl, "/node"), null, out Response);
			else
				return HttpRest.Post(string.Concat(dbUrl, "/node"), jsonProperties, out Response);
		}

		public static HttpStatusCode GetNode(string dbUrl, long NodeId, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/node/", NodeId.ToString()), out Response);
		}

		public static HttpStatusCode SetPropertiesOnNode(string dbUrl, long NodeId, string jsonProperties)
		{
			return HttpRest.Put(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/properties"), jsonProperties);
		}

		public static HttpStatusCode GetPropertiesOnNode(string dbUrl, long NodeId, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/properties"), out Response);
		}

		public static HttpStatusCode RemovePropertiesFromNode(string dbUrl, long NodeId)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/properties"));
		}

		public static HttpStatusCode SetPropertyOnNode(string dbUrl, long NodeId, string PropertyName, object Value)
		{
			return HttpRest.Put(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/properties/", PropertyName), JToken.FromObject(Value).ToString(Formatting.None));
		}

		public static HttpStatusCode GetPropertyOnNode(string dbUrl, long NodeId, string PropertyName, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/properties/", PropertyName), out Response);
		}

		public static HttpStatusCode RemovePropertyFromNode(string dbUrl, long NodeId, string PropertyName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/properties/", PropertyName));
		}

		public static HttpStatusCode DeleteNode(string dbUrl, long NodeId)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/node/", NodeId.ToString()));
		}

		public static HttpStatusCode CreateRelationship(string dbUrl, long FromNodeId, string ToNodeSelf, string Name, string jsonProperties, out string Response)
		{
			JObject jo = new JObject();
			jo.Add("to", ToNodeSelf);
			jo.Add("data", JToken.Parse(string.IsNullOrWhiteSpace(jsonProperties) ? "{}" : jsonProperties));
			jo.Add("type", Name);

			return HttpRest.Post(string.Concat(dbUrl, "/node/", FromNodeId.ToString(), "/relationships"), jo.ToString(Formatting.None), out Response);
		}

		public static HttpStatusCode SetPropertiesOnRelationship(string dbUrl, long RelationshipId, string jsonProperties)
		{
			return HttpRest.Put(string.Concat(dbUrl, "/relationship/", RelationshipId.ToString(), "/properties"), jsonProperties);
		}

		public static HttpStatusCode GetPropertiesOnRelationship(string dbUrl, long RelationshipId, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/relationship/", RelationshipId.ToString(), "/properties"), out Response);
		}

		public static HttpStatusCode RemovePropertiesFromRelationship(string dbUrl, long RelationshipId)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/node/", RelationshipId.ToString(), "/properties"));
		}

		public static HttpStatusCode SetPropertyOnRelationship(string dbUrl, long RelationshipId, string PropertyName, object Value)
		{
			return HttpRest.Put(string.Concat(dbUrl, "/relationship/", RelationshipId.ToString(), "/properties/", PropertyName), JToken.FromObject(Value).ToString());
		}

		public static HttpStatusCode GetPropertyOnRelationship(string dbUrl, long RelationshipId, string PropertyName, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/relationship/", RelationshipId.ToString(), "/properties/", PropertyName), out Response);
		}

		public static HttpStatusCode RemovePropertyFromRelationship(string dbUrl, long RelationshipId, string PropertyName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/relationship/", RelationshipId.ToString(), "/properties/", PropertyName));
		}

		public static HttpStatusCode DeleteRelationship(string dbUrl, long RelationshipId)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/relationship/", RelationshipId.ToString()));
		}

		public static HttpStatusCode GetRelationshipsOnNode(string dbUrl, long NodeId, RelationshipDirection Direction, IEnumerable<string> Relationships, out string Response)
		{
			if (Direction == null)
			{
				Direction = RelationshipDirection.All;
			}

			if (Relationships == null || Relationships.Count() == 0)
			{
				return HttpRest.Get(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/relationships/", Direction.ToString()), out Response);
			}
			else
			{
				return HttpRest.Get(string.Concat(dbUrl, "/node/", NodeId.ToString(), "/relationships/", Direction.ToString(), "/", string.Join("&", Relationships)), out Response);
			}
		}

		public static HttpStatusCode GetRelationshipTypes(string dbUrl, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/relationship/types"), out Response);
		}

		public static HttpStatusCode CreateNodeIndex(string dbUrl, string IndexName, string jsonConfig, out string Response)
		{
			JObject jo = new JObject();
			jo.Add("name", IndexName);
			jo.Add("config", jsonConfig);

			return HttpRest.Post(string.Concat(dbUrl, "/index/node"), jo.ToString(Formatting.None), out Response);
		}

		public static HttpStatusCode CreateRelationshipIndex(string dbUrl, string IndexName, string jsonConfig, out string Response)
		{
			JObject jo = new JObject();
			jo.Add("name", IndexName);
			jo.Add("config", jsonConfig);

			return HttpRest.Post(string.Concat(dbUrl, "/index/relationship"), jo.ToString(Formatting.None), out Response);
		}

		public static HttpStatusCode DeleteNodeIndex(string dbUrl, string IndexName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", IndexName));
		}

		public static HttpStatusCode DeleteRelationshipIndex(string dbUrl, string IndexName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", IndexName));
		}

		public static HttpStatusCode ListNodeIndexes(string dbUrl, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/node"), out Response);
		}

		public static HttpStatusCode ListRelationshipIndexes(string dbUrl, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/relationship"), out Response);
		}

		public static HttpStatusCode AddNodeToIndex(string dbUrl, long NodeId, string IndexName, string Key, object Value, out string Response)
		{
			string Self = string.Concat(dbUrl, "/", NodeId.ToString());
			return AddNodeToIndex(dbUrl, Self, IndexName, Key, Value, out Response);
		}

		public static HttpStatusCode AddNodeToIndex(string dbUrl, string NodeSelf, string IndexName, string Key, object Value, out string Response)
		{
			return HttpRest.Post(string.Concat(dbUrl, "/index/node/", IndexName, "/", Key, "/", JToken.FromObject(Value).ToString(Formatting.None)), NodeSelf, out Response);
		}

		public static HttpStatusCode AddRelationshipToIndex(string dbUrl, long RelationshipId, string IndexName, string Key, object Value, out string Response)
		{
			string Self = string.Concat(dbUrl, "/", RelationshipId.ToString());
			return AddRelationshipToIndex(dbUrl, Self, IndexName, Key, Value, out Response);
		}

		public static HttpStatusCode AddRelationshipToIndex(string dbUrl, string RelationshipSelf, string IndexName, string Key, object Value, out string Response)
		{
			return HttpRest.Post(string.Concat(dbUrl, "/index/relationship/", IndexName, "/", Key, "/", JToken.FromObject(Value).ToString(Formatting.None)), RelationshipSelf, out Response);
		}

		public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long NodeId, string IndexName, string Key, object Value)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", IndexName, "/", Key, "/", JToken.FromObject(Value).ToString(Formatting.None), "/", NodeId));
		}

		public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long NodeId, string IndexName, string Key)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", IndexName, "/", Key, "/", NodeId));
		}

		public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long NodeId, string IndexName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", IndexName, "/", NodeId));
		}

		public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long RelationshipId, string IndexName, string Key, object Value)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", IndexName, "/", Key, "/", JToken.FromObject(Value).ToString(Formatting.None), "/", RelationshipId));
		}

		public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long RelationshipId, string IndexName, string Key)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", IndexName, "/", Key, "/", RelationshipId));
		}

		public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long RelationshipId, string IndexName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", IndexName, "/", RelationshipId));
		}

		public static HttpStatusCode GetNode(string dbUrl, string IndexName, string Key, object Value, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/node/", IndexName, "/", Key, "/", JToken.FromObject(Value).ToString(Formatting.None)), out Response);
		}

		public static HttpStatusCode GetNode(string dbUrl, string IndexName, string SearchQuery, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/node/", IndexName, "?query=", SearchQuery), out Response);
		}

		public static HttpStatusCode GetRelationship(string dbUrl, string IndexName, string Key, object Value, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/relationship/", IndexName, "/", Key, "/", JToken.FromObject(Value).ToString(Formatting.None)), out Response);
		}

		public static HttpStatusCode GetRelationship(string dbUrl, string IndexName, string SearchQuery, out string Response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/relationship/", IndexName, "?query=", SearchQuery), out Response);
		}

		public static HttpStatusCode Traverse(string dbUrl, long NodeId, Order order, Uniqueness uniqueness, IEnumerable<TraverseRelationship> relationships, PruneEvaluator pruneEvaluator, ReturnFilter returnFilter, int? MaxDepth, ReturnType returnType, out string Response)
		{
			JObject jo = new JObject();

			jo.Add(order == null ? Order.DepthFirst.ToJson() : order.ToJson());
			jo.Add(uniqueness == null ? Uniqueness.NodePath.ToJson() : uniqueness.ToJson());

			JArray ja = new JArray();
			foreach (TraverseRelationship r in relationships)
			{
				ja.Add(r.ToJson());
			}
			jo.Add(new JProperty("relationships", ja));

			jo.Add(pruneEvaluator == null ? PruneEvaluator.None.ToJson() : pruneEvaluator.ToJson());

			jo.Add(returnFilter == null ? ReturnFilter.AllButStartNode.ToJson() : returnFilter.ToJson());

			if (MaxDepth == null)
			{
				MaxDepth = 1;
			}

			jo.Add("max_depth", MaxDepth.Value);

			return HttpRest.Post(string.Concat(dbUrl, "/node/", NodeId, "/traverse/", returnType == null ? ReturnType.Node.ToString() : returnType.ToString()), jo.ToString(Formatting.None), out Response);
		}

		public static HttpStatusCode PathBetweenNodes(string dbUrl, long FromNodeId, long ToNodeId, IEnumerable<TraverseRelationship> relationships, int MaxDepth, PathAlgorithm algorithm, bool ReturnAllPaths, out string Response)
		{
			JObject jo = new JObject();

			jo.Add("to", string.Concat(dbUrl, "/node/", ToNodeId.ToString()));

			JArray ja = new JArray();
			foreach (TraverseRelationship r in relationships)
			{
				ja.Add(r.ToJson());
			}
			jo.Add(new JProperty("relationships", ja));

			jo.Add("max_depth", MaxDepth);

			jo.Add("algorithm", algorithm.ToString());

			string commandPath = ReturnAllPaths == true ? "/paths" : "/path";

			return HttpRest.Post(string.Concat(dbUrl, "/node/", FromNodeId, commandPath), jo.ToString(Formatting.None), out Response);
		}

	}
}
