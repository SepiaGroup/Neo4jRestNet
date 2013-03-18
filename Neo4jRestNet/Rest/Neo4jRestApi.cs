using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Neo4jRestNet.Configuration;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Neo4jRestNet.Rest
{
	public static class Neo4jRestApi
	{
		public static HttpStatusCode GetRoot(string dbUrl, out string response)
		{
			return HttpRest.Get(dbUrl, out response);
		}

		public static HttpStatusCode CreateNode(string dbUrl, string jsonProperties, out string response)
		{
			return HttpRest.Post(Connection.GetServiceRoot(dbUrl).Node, string.IsNullOrWhiteSpace(jsonProperties) ? null : jsonProperties, out response);
		}

		public static HttpStatusCode GetNode(string dbUrl, long nodeId, out string response)
		{
			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString()), out response);
		}

		public static HttpStatusCode CreateUniqueNode(string dbUrl, string jsonProperties, string indexName, string key, object value, IndexUniqueness uniqueness, out string response)
		{
			var jo = new JObject
			         	{
			         		{"key", key}, 
							{"value", JToken.FromObject(value)}, 
							{"properties", JToken.Parse(string.IsNullOrWhiteSpace(jsonProperties) ? "{}" : jsonProperties)}
			         	};

			return HttpRest.Post(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName, "?uniqueness=", uniqueness), jo.ToString(Formatting.None, new IsoDateTimeConverter()), out response);
		}

		public static HttpStatusCode SetPropertiesOnNode(string dbUrl, long nodeId, string jsonProperties)
		{
			return HttpRest.Put(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/properties"), jsonProperties);
		}

		public static HttpStatusCode GetPropertiesOnNode(string dbUrl, long nodeId, out string response)
		{
			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/properties"), out response);
		}

		public static HttpStatusCode RemovePropertiesFromNode(string dbUrl, long nodeId)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/properties"));
		}

		public static HttpStatusCode SetPropertyOnNode(string dbUrl, long nodeId, string propertyName, object value)
		{
			return HttpRest.Put(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/properties/", propertyName),
								JToken.FromObject(value).ToString(Formatting.None, new IsoDateTimeConverter()));
		}

		public static HttpStatusCode GetPropertyOnNode(string dbUrl, long nodeId, string propertyName, out string response)
		{
			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/properties/", propertyName), out response);
		}

		public static HttpStatusCode RemovePropertyFromNode(string dbUrl, long nodeId, string propertyName)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/properties/", propertyName));
		}

		public static HttpStatusCode DeleteNode(string dbUrl, long nodeId)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString()));
		}

		public static HttpStatusCode CreateRelationship(string dbUrl, long startNodeId, long endNodeId, string name, string jsonProperties, out string response)
		{
			var jo = new JObject
                         {
                             {"to", string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", endNodeId)},
                             {"data", JToken.Parse(string.IsNullOrWhiteSpace(jsonProperties) ? "{}" : jsonProperties)},
                             {"type", name}
                         };

			return HttpRest.Post(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", startNodeId.ToString(), "/relationships"), jo.ToString(Formatting.None, new IsoDateTimeConverter()), out response);
		}

		public static HttpStatusCode CreateUniqueRelationship(string dbUrl, long startNodeId, long endNodeId, string name, string jsonProperties, string indexName, string key, object value, IndexUniqueness uniqueness, out string response)
		{
			var jo = new JObject
                         {
                             {"key", key},
							 {"value", JToken.FromObject(value)},
							 {"start", string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", startNodeId)},
							 {"end", string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", endNodeId)},
                             {"properties", JToken.Parse(string.IsNullOrWhiteSpace(jsonProperties) ? "{}" : jsonProperties)},
                             {"type", name}
                         };
			
			return HttpRest.Post(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName, "?uniqueness=", uniqueness), jo.ToString(), out response);
		}


		public static HttpStatusCode SetPropertiesOnRelationship(string dbUrl, long relationshipId, string jsonProperties)
		{
			return HttpRest.Put(string.Concat(Connection.GetServiceRoot(dbUrl).Relationship, "/", relationshipId.ToString(), "/properties"), jsonProperties);
		}

		public static HttpStatusCode GetPropertiesOnRelationship(string dbUrl, long relationshipId, out string response)
		{
			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).Relationship, "/", relationshipId.ToString(), "/properties"), out response);
		}

		public static HttpStatusCode RemovePropertiesFromRelationship(string dbUrl, long relationshipId)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", relationshipId.ToString(), "/properties"));
		}

		public static HttpStatusCode SetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName, object value)
		{
			return
				HttpRest.Put(
					string.Concat(Connection.GetServiceRoot(dbUrl).Relationship, "/", relationshipId.ToString(), "/properties/", propertyName),
					JToken.FromObject(value).ToString());
		}

		public static HttpStatusCode GetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName, out string response)
		{
			return
				HttpRest.Get(
					string.Concat(Connection.GetServiceRoot(dbUrl).Relationship, "/", relationshipId.ToString(), "/properties/", propertyName),
					out response);
		}

		public static HttpStatusCode RemovePropertyFromRelationship(string dbUrl, long relationshipId, string propertyName)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).Relationship, "/", relationshipId.ToString(), "/properties/",
											  propertyName));
		}

		public static HttpStatusCode DeleteRelationship(string dbUrl, long relationshipId)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).Relationship, "/", relationshipId.ToString()));
		}

		public static HttpStatusCode GetRelationshipsOnNode(string dbUrl, long nodeId, RelationshipDirection direction, IEnumerable<string> relationships, out string response)
		{
			if (direction == null)
			{
				direction = RelationshipDirection.All;
			}

			if (relationships == null || !relationships.Any())
			{
				return
					HttpRest.Get(
						string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/relationships/", direction.ToString()),
						out response);
			}

			return
				HttpRest.Get(
					string.Concat(Connection.GetServiceRoot(dbUrl).Node, "/", nodeId.ToString(), "/relationships/", direction.ToString(), "/",
								  string.Join("&", relationships)), out response);
		}

		public static HttpStatusCode GetRelationshipTypes(string dbUrl, out string response)
		{
			return HttpRest.Get(Connection.GetServiceRoot(dbUrl).RelationshipTypes, out response);
		}

		public static HttpStatusCode CreateNodeIndex(string dbUrl, string indexName, string jsonConfig, out string response)
		{
			var jo = new JObject { { "name", indexName }, { "config", jsonConfig } };
			return HttpRest.Post(Connection.GetServiceRoot(dbUrl).NodeIndex, jo.ToString(Formatting.None, new IsoDateTimeConverter()), out response);
		}

		public static HttpStatusCode CreateRelationshipIndex(string dbUrl, string indexName, string jsonConfig, out string response)
		{
			var jo = new JObject { { "name", indexName }, { "config", jsonConfig } };
			return HttpRest.Post(Connection.GetServiceRoot(dbUrl).RelationshipIndex, jo.ToString(Formatting.None, new IsoDateTimeConverter()), out response);
		}

		public static HttpStatusCode DeleteNodeIndex(string dbUrl, string indexName)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName));
		}

		public static HttpStatusCode DeleteRelationshipIndex(string dbUrl, string indexName)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName));
		}

		public static HttpStatusCode ListNodeIndexes(string dbUrl, out string response)
		{
			return HttpRest.Get(Connection.GetServiceRoot(dbUrl).NodeIndex, out response);
		}

		public static HttpStatusCode ListRelationshipIndexes(string dbUrl, out string response)
		{
			return HttpRest.Get(Connection.GetServiceRoot(dbUrl).RelationshipIndex, out response);
		}

		public static HttpStatusCode AddNodeToIndex(string dbUrl, long nodeId, string indexName, string key, object value, out string response, bool unique = false)
		{
			var self = string.Concat(dbUrl, "/", nodeId.ToString());

			return AddNodeToIndex(dbUrl, self, indexName, key, value, out response, unique);
		}

		public static HttpStatusCode AddNodeToIndex(string dbUrl, string nodeSelf, string indexName, string key, object value, out string response, bool unique = false)
		{
			var jo = new JObject
			            {
			             	{ "key", key }, 
							{ "value", JToken.FromObject(value) }, 
							{ "uri", nodeSelf }
			            };

			return HttpRest.Post(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName, unique ? "?unique" : string.Empty), jo.ToString(Formatting.None, new IsoDateTimeConverter()), out response);
		}

		public static HttpStatusCode AddRelationshipToIndex(string dbUrl, long relationshipId, string indexName, string key, object value, out string response, bool unique = false)
		{
			var self = string.Concat(dbUrl, "/", relationshipId.ToString());
			return AddRelationshipToIndex(dbUrl, self, indexName, key, value, out response, unique);
		}

		public static HttpStatusCode AddRelationshipToIndex(string dbUrl, string relationshipself, string indexName, string key, object value, out string response, bool unique = false)
		{
			var jo = new JObject
			            {
			             	{ "key", key }, 
							{ "value", JToken.FromObject(value) }, 
							{ "uri", relationshipself }
			            };

			return HttpRest.Post(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName, unique ? "?unique" : string.Empty),
								  jo.ToString(Formatting.None, new IsoDateTimeConverter()), out response);
		}

		public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName, string key, object value)
		{
			var strValue = value is string ? Uri.EscapeDataString(value.ToString()) : value.ToString();

			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName, "/", key, "/", strValue, "/", nodeId));
		}

		public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName, string key)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName, "/", key, "/", nodeId));
		}

		public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName, "/", nodeId));
		}

		public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName, string key, object value)
		{
			var strValue = value is string ? Uri.EscapeDataString(value.ToString()) : value.ToString();

			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName, "/", key, "/", strValue, "/", relationshipId));
		}

		public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName, string key)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName, "/", key, "/", relationshipId));
		}

		public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName)
		{
			return HttpRest.Delete(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName, "/", relationshipId));
		}

		public static HttpStatusCode GetNode(string dbUrl, string indexName, string key, object value, out string response)
		{
			var strValue = value is string ? Uri.EscapeDataString(value.ToString()) : value.ToString();

			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName, "/", key, "/", strValue), out response);
		}

		public static HttpStatusCode GetNode(string dbUrl, string indexName, string searchQuery, out string response)
		{
			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).NodeIndex, "/", indexName, "?query=", Uri.EscapeDataString(searchQuery)), out response);
		}

		public static HttpStatusCode GetRelationship(string dbUrl, long id, out string response)
		{
			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).Relationship, "/", id), out response);
		}

		public static HttpStatusCode GetRelationship(string dbUrl, string indexName, string key, object value, out string response)
		{
			var strValue = value is string ? Uri.EscapeDataString(value.ToString()) : value.ToString();

			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName, "/", key, "/", strValue), out response);
		}

		public static HttpStatusCode GetRelationship(string dbUrl, string indexName, string searchQuery, out string response)
		{
			return HttpRest.Get(string.Concat(Connection.GetServiceRoot(dbUrl).RelationshipIndex, "/", indexName, "?query=", Uri.EscapeDataString(searchQuery)), out response);
		}


		//public static HttpStatusCode PathBetweenNodes(string dbUrl, long fromNodeId, long toNodeId,
		//                                              IEnumerable<TraverseRelationship> relationships, int maxDepth,
		//                                              PathAlgorithm algorithm, bool returnAllPaths, out string response)
		//{
		//    var jo = new JObject { { "to", string.Concat(GetServiceRoot(dbUrl).Node, "/", toNodeId.ToString()) } };

		//    var ja = new JArray();
		//    foreach (var r in relationships)
		//    {
		//        ja.Add(r.ToJson());
		//    }
		//    jo.Add(new JProperty("relationships", ja));

		//    jo.Add("max_depth", maxDepth);

		//    jo.Add("algorithm", algorithm.ToString());

		//    var commandPath = returnAllPaths ? "/paths" : "/path";

		//    return HttpRest.Post(string.Concat(GetServiceRoot(dbUrl).Node, "/", fromNodeId, commandPath), jo.ToString(Formatting.None, new IsoDateTimeConverter()), out response);
		//}
	}
}