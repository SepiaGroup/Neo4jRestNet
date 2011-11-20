﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using Neo4jRestNet.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    internal class Neo4jRestApi
    {
        public static HttpStatusCode GetRoot(string dbUrl, out string response)
        {
            return HttpRest.Get(dbUrl, out response);
        }

        public static HttpStatusCode CreateNode(string dbUrl, string jsonProperties, out string response)
        {
            return HttpRest.Post(string.Concat(dbUrl, "/node"), string.IsNullOrWhiteSpace(jsonProperties) ? null : jsonProperties, out response);
        }

        public static HttpStatusCode GetNode(string dbUrl, long nodeId, out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/node/", nodeId.ToString()), out response);
        }

        public static HttpStatusCode SetPropertiesOnNode(string dbUrl, long nodeId, string jsonProperties)
        {
            return HttpRest.Put(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties"), jsonProperties);
        }

        public static HttpStatusCode GetPropertiesOnNode(string dbUrl, long nodeId, out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties"), out response);
        }

        public static HttpStatusCode RemovePropertiesFromNode(string dbUrl, long nodeId)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties"));
        }

        public static HttpStatusCode SetPropertyOnNode(string dbUrl, long nodeId, string propertyName, object value)
        {
            return HttpRest.Put(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties/", propertyName),
                                JToken.FromObject(value).ToString(Formatting.None));
        }

        public static HttpStatusCode GetPropertyOnNode(string dbUrl, long nodeId, string propertyName,
                                                       out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties/", propertyName),
                                out response);
        }

        public static HttpStatusCode RemovePropertyFromNode(string dbUrl, long nodeId, string propertyName)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/node/", nodeId.ToString(), "/properties/", propertyName));
        }

        public static HttpStatusCode DeleteNode(string dbUrl, long nodeId)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/node/", nodeId.ToString()));
        }

        public static HttpStatusCode CreateRelationship(string dbUrl, long fromNodeId, string toNodeSelf, string name,
                                                        string jsonProperties, out string response)
        {
            var jo = new JObject
                         {
                             {"to", toNodeSelf},
                             {"data", JToken.Parse(string.IsNullOrWhiteSpace(jsonProperties) ? "{}" : jsonProperties)},
                             {"type", name}
                         };

            return HttpRest.Post(string.Concat(dbUrl, "/node/", fromNodeId.ToString(), "/relationships"),
                                 jo.ToString(Formatting.None), out response);
        }

        public static HttpStatusCode SetPropertiesOnRelationship(string dbUrl, long relationshipId,
                                                                 string jsonProperties)
        {
            return HttpRest.Put(string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties"),
                                jsonProperties);
        }

        public static HttpStatusCode GetPropertiesOnRelationship(string dbUrl, long relationshipId, out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties"),
                                out response);
        }

        public static HttpStatusCode RemovePropertiesFromRelationship(string dbUrl, long relationshipId)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/node/", relationshipId.ToString(), "/properties"));
        }

        public static HttpStatusCode SetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName,
                                                               object value)
        {
            return
                HttpRest.Put(
                    string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties/", propertyName),
                    JToken.FromObject(value).ToString());
        }

        public static HttpStatusCode GetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName,
                                                               out string response)
        {
            return
                HttpRest.Get(
                    string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties/", propertyName),
                    out response);
        }

        public static HttpStatusCode RemovePropertyFromRelationship(string dbUrl, long relationshipId,
                                                                    string propertyName)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/relationship/", relationshipId.ToString(), "/properties/",
                                              propertyName));
        }

        public static HttpStatusCode DeleteRelationship(string dbUrl, long relationshipId)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/relationship/", relationshipId.ToString()));
        }

        public static HttpStatusCode GetRelationshipsOnNode(string dbUrl, long nodeId, RelationshipDirection direction,
                                                            IEnumerable<string> relationships, out string response)
        {
            if (direction == null)
            {
                direction = RelationshipDirection.All;
            }

            if (relationships == null || relationships.Count() == 0)
            {
                return
                    HttpRest.Get(
                        string.Concat(dbUrl, "/node/", nodeId.ToString(), "/relationships/", direction.ToString()),
                        out response);
            }

            return
                HttpRest.Get(
                    string.Concat(dbUrl, "/node/", nodeId.ToString(), "/relationships/", direction.ToString(), "/",
                                  string.Join("&", relationships)), out response);
        }

        public static HttpStatusCode GetRelationshipTypes(string dbUrl, out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/relationship/types"), out response);
        }

        public static HttpStatusCode CreateNodeIndex(string dbUrl, string indexName, string jsonConfig,
                                                     out string response)
        {
            var jo = new JObject { { "name", indexName }, { "config", jsonConfig } };
            return HttpRest.Post(string.Concat(dbUrl, "/index/node"), jo.ToString(Formatting.None), out response);
        }

        public static HttpStatusCode CreateRelationshipIndex(string dbUrl, string indexName, string jsonConfig,
                                                             out string response)
        {
            var jo = new JObject { { "name", indexName }, { "config", jsonConfig } };
            return HttpRest.Post(string.Concat(dbUrl, "/index/relationship"), jo.ToString(Formatting.None), out response);
        }

        public static HttpStatusCode DeleteNodeIndex(string dbUrl, string indexName)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", indexName));
        }

        public static HttpStatusCode DeleteRelationshipIndex(string dbUrl, string indexName)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName));
        }

        public static HttpStatusCode ListNodeIndexes(string dbUrl, out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/index/node"), out response);
        }

        public static HttpStatusCode ListRelationshipIndexes(string dbUrl, out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/index/relationship"), out response);
        }

        public static HttpStatusCode AddNodeToIndex(string dbUrl, long nodeId, string indexName, string key,
                                                    object value, out string response)
        {
            var self = string.Concat(dbUrl, "/", nodeId.ToString());
            return AddNodeToIndex(dbUrl, self, indexName, key, value, out response);
        }

        public static HttpStatusCode AddNodeToIndex(string dbUrl, string nodeSelf, string indexName, string key,
                                                    object value, out string response)
        {
            var obj = new { value, uri = nodeSelf, key };
            return HttpRest.Post(string.Concat(dbUrl, "/index/node/", indexName),
                                 JToken.FromObject(obj).ToString(Formatting.None), out response);
        }

        public static HttpStatusCode AddRelationshipToIndex(string dbUrl, long relationshipId, string indexName,
                                                            string key, object value, out string response)
        {
            var self = string.Concat(dbUrl, "/", relationshipId.ToString());
            return AddRelationshipToIndex(dbUrl, self, indexName, key, value, out response);
        }

        public static HttpStatusCode AddRelationshipToIndex(string dbUrl, string relationshipself, string indexName,
                                                            string key, object value, out string response)
        {
            return
                HttpRest.Post(
                    string.Concat(dbUrl, "/index/relationship/", indexName, "/", key, "/",
                                  JToken.FromObject(value).ToString(Formatting.None)), relationshipself, out response);
        }

        public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName, string key,
                                                         object value)
        {
            return
                HttpRest.Delete(string.Concat(dbUrl, "/index/node/", indexName, "/", key, "/",
                                              JToken.FromObject(value).ToString(Formatting.None), "/", nodeId));
        }

        public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName, string key)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", indexName, "/", key, "/", nodeId));
        }

        public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/index/node/", indexName, "/", nodeId));
        }

        public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName,
                                                                 string key, object value)
        {
            return
                HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName, "/", key, "/",
                                              JToken.FromObject(value).ToString(Formatting.None), "/", relationshipId));
        }

        public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName,
                                                                 string key)
        {
            return
                HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName, "/", key, "/", relationshipId));
        }

        public static HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName)
        {
            return HttpRest.Delete(string.Concat(dbUrl, "/index/relationship/", indexName, "/", relationshipId));
        }

        public static HttpStatusCode GetNode(string dbUrl, string indexName, string key, object value,
                                             out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/index/node/", indexName, "/", key, "/", value.ToString()), 
                out response);
        }

        public static HttpStatusCode GetNode(string dbUrl, string indexName, string searchQuery, out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/index/node/", indexName, "?query=", searchQuery), out response);
        }

        public static HttpStatusCode GetRelationship(string dbUrl, string indexName, string key, object value,
                                                     out string response)
        {
            return
                HttpRest.Get(
                    string.Concat(dbUrl, "/index/relationship/", indexName, "/", key, "/",
                                  JToken.FromObject(value).ToString(Formatting.None)), out response);
        }

        public static HttpStatusCode GetRelationship(string dbUrl, string indexName, string searchQuery,
                                                     out string response)
        {
            return HttpRest.Get(string.Concat(dbUrl, "/index/relationship/", indexName, "?query=", searchQuery),
                                out response);
        }

        public static HttpStatusCode Traverse(string dbUrl, long nodeId, Order order, Uniqueness uniqueness,
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
                    string.Concat(dbUrl, "/node/", nodeId, "/traverse/",
                                  returnType == null ? ReturnType.Node.ToString() : returnType.ToString()),
                    jo.ToString(Formatting.None), out response);
        }

        public static HttpStatusCode PathBetweenNodes(string dbUrl, long fromNodeId, long toNodeId,
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

            return HttpRest.Post(string.Concat(dbUrl, "/node/", fromNodeId, commandPath), jo.ToString(Formatting.None),
                                 out response);
        }
    }
}