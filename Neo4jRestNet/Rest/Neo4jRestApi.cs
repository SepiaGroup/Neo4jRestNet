using System.Collections.Generic;
using System.Linq;
using System.Net;
using Neo4jRestNet.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
	public class Neo4jRestApi : INeo4jRestApi
	{
		private readonly string _dbUrl;
		public Neo4jRestApi(string dbUrl)
		{
			_dbUrl = dbUrl;
		}

		public HttpStatusCode GetRoot(out string response)
		{
			return HttpRest.Get(_dbUrl, out response);
		}

		public HttpStatusCode CreateNode(string jsonProperties, out string response)
		{
			return HttpRest.Post(string.Concat(_dbUrl, "/node"), string.IsNullOrWhiteSpace(jsonProperties) ? null : jsonProperties, out response);
		}

		public HttpStatusCode GetNode(long nodeId, out string response)
		{
			return HttpRest.Get(string.Concat(_dbUrl, "/node/", nodeId.ToString()), out response);
		}

		public HttpStatusCode SetPropertiesOnNode(long nodeId, string jsonProperties)
		{
			return HttpRest.Put(string.Concat(_dbUrl, "/node/", nodeId.ToString(), "/properties"), jsonProperties);
		}

		public HttpStatusCode GetPropertiesOnNode(long nodeId, out string response)
		{
			return HttpRest.Get(string.Concat(_dbUrl, "/node/", nodeId.ToString(), "/properties"), out response);
		}

		public HttpStatusCode RemovePropertiesFromNode(string dbUrl, long nodeId)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties"));
		}

		public HttpStatusCode SetPropertyOnNode(string dbUrl, long nodeId, string propertyName, object value)
		{
			return HttpRest.Put(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties/", propertyName),
								JToken.FromObject(value).ToString(Formatting.None));
		}

		public HttpStatusCode GetPropertyOnNode(string dbUrl, long nodeId, string propertyName, out string response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties/", propertyName), out response);
		}

		public HttpStatusCode RemovePropertyFromNode(string dbUrl, long nodeId, string propertyName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties/", propertyName));
		}

		public HttpStatusCode DeleteNode(long nodeId)
		{
			return HttpRest.Delete(string.Concat(_dbUrl, "/node/", nodeId.ToString()));
		}

		public HttpStatusCode CreateRelationship(long fromNodeId, string toNodeSelf, string name, string jsonProperties, out string response)
		{
			var jo = new JObject
                         {
                             {"to", toNodeSelf},
                             {"data", JToken.Parse(string.IsNullOrWhiteSpace(jsonProperties) ? "{}" : jsonProperties)},
                             {"type", name}
                         };

			return HttpRest.Post(string.Concat(_dbUrl, "/node/", fromNodeId.ToString(), "/relationships"), jo.ToString(Formatting.None), out response);
		}

		public HttpStatusCode SetPropertiesOnRelationship(string dbUrl, long relationshipId, string jsonProperties)
		{
			return HttpRest.Put(string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties"), jsonProperties);
		}

		public HttpStatusCode GetPropertiesOnRelationship(string dbUrl, long relationshipId, out string response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties"), out response);
		}

		public HttpStatusCode RemovePropertiesFromRelationship(string dbUrl, long relationshipId)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/node/", relationshipId.ToString(), "/properties"));
		}

		public HttpStatusCode SetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName, object value)
		{
			return
				HttpRest.Put(
					string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties/", propertyName),
					JToken.FromObject(value).ToString());
		}

		public HttpStatusCode GetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName, out string response)
		{
			return
				HttpRest.Get(
					string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties/", propertyName),
					out response);
		}

		public HttpStatusCode RemovePropertyFromRelationship(string dbUrl, long relationshipId, string propertyName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties/",
											  propertyName));
		}

		public HttpStatusCode DeleteRelationship(string dbUrl, long relationshipId)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/relationship/", relationshipId.ToString()));
		}

		public HttpStatusCode GetRelationshipsOnNode(long nodeId, RelationshipDirection direction, IEnumerable<string> relationships, out string response)
		{
			if (direction == null)
			{
				direction = RelationshipDirection.All;
			}

			if (relationships == null || relationships.Count() == 0)
			{
				return
					HttpRest.Get(
						string.Concat(_dbUrl, "/node/", nodeId.ToString(), "/relationships/", direction.ToString()),
						out response);
			}

			return
				HttpRest.Get(
					string.Concat(_dbUrl, "/node/", nodeId.ToString(), "/relationships/", direction.ToString(), "/",
								  string.Join("&", relationships)), out response);
		}

		public HttpStatusCode GetRelationshipTypes(string dbUrl, out string response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/relationship/types"), out response);
		}

		public HttpStatusCode CreateNodeIndex(string dbUrl, string indexName, string jsonConfig, out string response)
		{
			var jo = new JObject { { "name", indexName }, { "config", jsonConfig } };
			return HttpRest.Post(string.Concat(dbUrl, "/index/node"), jo.ToString(Formatting.None), out response);
		}

		public HttpStatusCode CreateRelationshipIndex(string dbUrl, string indexName, string jsonConfig, out string response)
		{
			var jo = new JObject { { "name", indexName }, { "config", jsonConfig } };
			return HttpRest.Post(string.Concat(dbUrl, "/index/relationship"), jo.ToString(Formatting.None), out response);
		}

		public HttpStatusCode DeleteNodeIndex(string dbUrl, string indexName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", indexName));
		}

		public HttpStatusCode DeleteRelationshipIndex(string dbUrl, string indexName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName));
		}

		public HttpStatusCode ListNodeIndexes(string dbUrl, out string response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/node"), out response);
		}

		public HttpStatusCode ListRelationshipIndexes(string dbUrl, out string response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/relationship"), out response);
		}

		public HttpStatusCode AddNodeToIndex(long nodeId, string indexName, string key, object value, out string response)
		{
			var self = string.Concat(_dbUrl, "/", nodeId.ToString());
			return AddNodeToIndex(self, indexName, key, value, out response);
		}

		public HttpStatusCode AddNodeToIndex(string nodeSelf, string indexName, string key, object value, out string response)
		{
			var obj = new { value, uri = nodeSelf, key };
			return HttpRest.Post(string.Concat(_dbUrl, "/index/node/", indexName),
								 JToken.FromObject(obj).ToString(Formatting.None), out response);
		}

		public HttpStatusCode AddRelationshipToIndex(string dbUrl, long relationshipId, string indexName, string key, object value, out string response)
		{
			var self = string.Concat(dbUrl, "/", relationshipId.ToString());
			return AddRelationshipToIndex(dbUrl, self, indexName, key, value, out response);
		}

		public HttpStatusCode AddRelationshipToIndex(string dbUrl, string relationshipself, string indexName, string key, object value, out string response)
		{
            var obj = new { value, uri = relationshipself, key };
			return HttpRest.Post(string.Concat(dbUrl, "/index/relationship/", indexName),
                                  JToken.FromObject(obj).ToString(Formatting.None), out response);
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key, object value)
		{
			return
				HttpRest.Delete(string.Concat(_dbUrl, "/index/node/", indexName, "/", key, "/",
											  JToken.FromObject(value).ToString(Formatting.None), "/", nodeId));
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key)
		{
			return HttpRest.Delete(string.Concat(_dbUrl, "/index/node/", indexName, "/", key, "/", nodeId));
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName)
		{
			return HttpRest.Delete(string.Concat(_dbUrl, "/index/node/", indexName, "/", nodeId));
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName, string key, object value)
		{
			return
				HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName, "/", key, "/",
											  JToken.FromObject(value).ToString(Formatting.None), "/", relationshipId));
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName, string key)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName, "/", key, "/", relationshipId));
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName)
		{
			return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName, "/", relationshipId));
		}

		public HttpStatusCode GetNode(string indexName, string key, object value, out string response)
		{
			return HttpRest.Get(string.Concat(_dbUrl, "/index/node/", indexName, "/", key, "/", value.ToString()), out response);
		}

		public HttpStatusCode GetNode(string indexName, string searchQuery, out string response)
		{
			return HttpRest.Get(string.Concat(_dbUrl, "/index/node/", indexName, "?query=", searchQuery), out response);
		}

		public HttpStatusCode GetRelationship(string dbUrl, string indexName, string key, object value, out string response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/relationship/", indexName, "/", key, "/", value.ToString()), out response);
		}

		public HttpStatusCode GetRelationship(string dbUrl, string indexName, string searchQuery, out string response)
		{
			return HttpRest.Get(string.Concat(dbUrl, "/index/relationship/", indexName, "?query=", searchQuery), out response);
		}

		public HttpStatusCode Traverse(long nodeId, Order order, Uniqueness uniqueness,
											  IEnumerable<TraverseRelationship> relationships,
											  PruneEvaluator pruneEvaluator, ReturnFilter returnFilter, int? maxDepth,
											  ReturnType returnType, out string response)
		{
			var jo = new JObject
                         {
                             order == null ? Order.DepthFirst.ToJson() : order.ToJson(),
                             uniqueness == null ? Uniqueness.NodePath.ToJson() : uniqueness.ToJson()
                         };

			var ja = new JArray();
			foreach (var r in relationships)
			{
				ja.Add(r.ToJson());
			}
			jo.Add(new JProperty("relationships", ja));

			jo.Add(pruneEvaluator == null ? PruneEvaluator.None.ToJson() : pruneEvaluator.ToJson());

			jo.Add(returnFilter == null ? ReturnFilter.AllButStartNode.ToJson() : returnFilter.ToJson());

			if (maxDepth == null)
			{
				maxDepth = 1;
			}

			jo.Add("max_depth", maxDepth.Value);

			return
				HttpRest.Post(
					string.Concat(_dbUrl, "/node/", nodeId, "/traverse/",
								  returnType == null ? ReturnType.Node.ToString() : returnType.ToString()),
					jo.ToString(Formatting.None), out response);
		}

		public HttpStatusCode PathBetweenNodes(string dbUrl, long fromNodeId, long toNodeId,
													  IEnumerable<TraverseRelationship> relationships, int maxDepth,
													  PathAlgorithm algorithm, bool returnAllPaths, out string response)
		{
			var jo = new JObject { { "to", string.Concat(dbUrl, "/node/", toNodeId.ToString()) } };

			var ja = new JArray();
			foreach (var r in relationships)
			{
				ja.Add(r.ToJson());
			}
			jo.Add(new JProperty("relationships", ja));

			jo.Add("max_depth", maxDepth);

			jo.Add("algorithm", algorithm.ToString());

			var commandPath = returnAllPaths ? "/paths" : "/path";

			return HttpRest.Post(string.Concat(dbUrl, "/node/", fromNodeId, commandPath), jo.ToString(Formatting.None), out response);
		}
	}
}