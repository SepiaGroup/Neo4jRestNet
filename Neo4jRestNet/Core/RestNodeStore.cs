using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using Neo4jRestNet.Configuration;
using Neo4jRestNet.Core.Exceptions;
using Neo4jRestNet.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Core
{
	public class RestNodeStore : INodeStore
	{
		public long Id { get; private set; }
		public string DbUrl { get; private set; }
		public string Self { get; private set; }
		public string OriginalNodeJson {get; private set; }

		#region Creators

		public RestNodeStore()
		{
			Id = int.MinValue;
			DbUrl = null;
			Self = null;
			OriginalNodeJson = null;
		}

		private RestNodeStore(ConnectionElement connection, long id, string nodeJson = null)
		{
			DbUrl = connection.DbUrl;
			Id = id;
			Self = string.Concat(Connection.GetServiceRoot(DbUrl).Node, "/", Id);
			OriginalNodeJson = nodeJson;
		}

		private RestNodeStore(string self, string nodeJson = null)
		{
			ParseSelf(self);
			OriginalNodeJson = nodeJson;
		}

		#endregion

		#region GetNode

		public Node GetRootNode(ConnectionElement connection)
		{
			string response;
			var status = Neo4jRestApi.GetRoot(Connection.GetDatabaseEndpoint(connection.DbUrl).Data, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error getting root node (http response:{0})", status));
			}

			JObject jo;
			try
			{
				jo = JObject.Parse(response);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid json", e);
			}

			JToken referenceNode;
			if (!jo.TryGetValue("reference_node", out referenceNode))
			{
				throw new NodeNotFoundException("Reference node not found");
			}

			var graphStore = new RestNodeStore(referenceNode.Value<string>());
			
			return new Node(graphStore);
		}

		public Node GetNode(ConnectionElement connection, long nodeId)
		{
			string response;
			var status = Neo4jRestApi.GetNode(connection.DbUrl, nodeId, out response);

			if (status == HttpStatusCode.NotFound)
			{
				throw new NodeNotFoundException(string.Format("Node({0})",nodeId));
			}

			return CreateNodeFromJson(response);
		}

		public IEnumerable<Node> GetNode(ConnectionElement connection, string indexName, string key, object value)
		{
			string response;
			var status = Neo4jRestApi.GetNode(connection.DbUrl, indexName, key, value, out response);
			if(status == HttpStatusCode.OK)
			{
				return ParseNodeJson(response);
			}

			if(status == HttpStatusCode.NotFound)
			{
				return new List<Node>();
			}

			throw new Exception(string.Format("Index not found in (index:{0})", indexName));
		}

		public IEnumerable<Node> GetNode(ConnectionElement connection, string indexName, string searchQuery)
		{
			string response;
			var status = Neo4jRestApi.GetNode(connection.DbUrl, indexName, searchQuery, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseNodeJson(response);
		}

		#endregion

		#region CreateNode

		public Node CreateNode(ConnectionElement connection, Properties properties)
		{
			string response;
			var status = Neo4jRestApi.CreateNode(connection.DbUrl, properties.ToString(), out response);

			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating node (http response:{0})", status));
			}

			return ParseNodeJson(response).First();
		}

		public Node CreateUniqueNode(ConnectionElement connection, Properties properties, string indexName, string key, object value, IndexUniqueness uniqueness)
		{
			string response;
			var status = Neo4jRestApi.CreateUniqueNode(connection.DbUrl, properties.ToString(), indexName, key, value, uniqueness, out response);

			if (status == HttpStatusCode.Created)
			{
				return ParseNodeJson(response).First();	
			}

			// Create unique node but index mapping already exists
			if(status == HttpStatusCode.OK)
			{
				return null;
			}
			
			throw new Exception(string.Format("Error creating node (http response:{0})", status));
		}

		#endregion

		#region Initilize

		public Node Initilize(ConnectionElement connection, long id, Properties properties)
		{
			return new Node(new RestNodeStore(connection, id), properties);
		}

		public Node Initilize(string selfUri, Properties properties)
		{
			return new Node(new RestNodeStore(selfUri), properties);
		}

		#endregion

		#region DeleteNode

		public HttpStatusCode DeleteNode()
		{
			var status = Neo4jRestApi.DeleteNode(DbUrl, Id);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error deleting node (node id:{0} http response:{1})", Id, status));
			}

			return status;
		}

		#endregion

		#region Index

		public Node AddToIndex(ConnectionElement connection, Node node, string indexName, string key, object value, bool unique = false)
		{
			string response;
			var status = Neo4jRestApi.AddNodeToIndex(connection.DbUrl, node.Id, indexName, key, value, out response, unique);

			if (status == HttpStatusCode.Created)
			{
				return ParseNodeJson(response).First();
			}

			// Add a node to an index but mapping already exists
			if(unique && status == HttpStatusCode.OK)
			{
				return null;  
			}

			throw new Exception(string.Format("Error adding node to index (http response:{0})", status));
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName)
		{
			var status = Neo4jRestApi.RemoveNodeFromIndex(connection.DbUrl, node.Id, indexName);

			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", node.Id, indexName, status));
			}

			return true;
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key)
		{
			var status = Neo4jRestApi.RemoveNodeFromIndex(connection.DbUrl, node.Id, indexName, key);

			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} key:{2} http response:{3})", node.Id, indexName, key, status));
			}

			return true;
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key, object value)
		{
			var status = Neo4jRestApi.RemoveNodeFromIndex(connection.DbUrl, node.Id, indexName, key, value);

			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} key:{2} http response:{3})", node.Id, indexName, key, status));
			}

			return true;
		}
		#endregion

		#region Properties

		public Properties GetProperties()
		{
			string response;
			var status = Neo4jRestApi.GetPropertiesOnNode(DbUrl, Id, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving properties on node (node id:{0} http response:{1})", Id, status));
			}

			return Properties.ParseJson(response);
		}

		public void SaveProperties(Properties properties)
		{
			var status = Neo4jRestApi.SetPropertiesOnNode(DbUrl, Id, properties.ToString());
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error setting properties on node (node id:{0} http response:{1})", Id, status));
			}

		}

		#endregion

		#region ParseNodeJson

		public static IEnumerable<Node> ParseNodeJson(string jsonNodes)
		{
			if (String.IsNullOrEmpty(jsonNodes))
			{
				return null;
			}

			var nodes = new List<Node>();

			// The Json passed in can be a JObject or JArray - this is to test for that.
			var jo = JObject.Parse(string.Concat("{\"root\":", jsonNodes, "}"));

			switch (jo["root"].Type)
			{
				case JTokenType.Object:
					nodes.Add(CreateNodeFromJson(jo["root"].ToString(Formatting.None, new IsoDateTimeConverter())));
					break;

				case JTokenType.Array:
					nodes.AddRange(from JObject jsonNode in jo["root"] select CreateNodeFromJson(jsonNode));
					break;

				default:
					throw new Exception("Invalid node json");
			}

			return nodes;
		}

		#endregion

		#region ParseSelf

		private void ParseSelf(string self)
		{
			if (!IsSelfANode(self))
			{
				throw new Exception(string.Format("Self is not a Node ({0})", self));
			}

			// Make sure there is no trailing /
			self = self.TrimEnd('/');

			var selfArray = self.Split('/');

			long nodeId;
			if (!long.TryParse(selfArray.Last(), out nodeId))
			{
				throw new Exception(string.Format("Invalid Self id ({0})", self));
			}

			var url = new Uri(self);
			
			DbUrl = string.Format("{0}://{1}", url.Scheme, url.Authority);

			// Set Id & NodeId values
			Id = nodeId;

			Self = self;
		}

		private static bool IsSelfANode(string self)
		{
			var selfArray = self.Split('/');
			return (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node");
		}

		#endregion

		#region CreateNodeFromJson

		private static Node CreateNodeFromJson(string nodeJson)
		{
			JObject jo;

			try
			{
				jo = JObject.Parse(nodeJson);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid node json", e);
			}

			return CreateNodeFromJson(jo);
		}

		public static Node CreateNodeFromJson(JObject nodeJson)
		{
			JToken self;
			if (!nodeJson.TryGetValue("self", out self))
			{
				throw new Exception("Invalid node json");
			}

			JToken properties;
			if (!nodeJson.TryGetValue("data", out properties))
			{
				throw new Exception("Invalid node json");
			}

			return new Node(new RestNodeStore(self.Value<string>(), nodeJson.ToString(Formatting.None, new IsoDateTimeConverter())), Properties.ParseJson(properties.ToString(Formatting.None, new IsoDateTimeConverter())));
		}

		public static Node CreateNodeFromSelf(string self)
		{
			return new Node(new RestNodeStore(self));
		}

		#endregion

		#region GetRelationships

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> relationshipTypes)
		{
			string response;
			var status = Neo4jRestApi.GetRelationshipsOnNode(DbUrl, Id, direction, relationshipTypes, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving relationships on node (node id:{0} http response:{1})", Id, status));
			}

			return RestRelationshipStore.ParseRelationshipJson(response);
		}

		#endregion

		#region CreateRelationship

		public Relationship CreateRelationship(Node startNode, Node endNode, string relationshipType, Properties properties)
		{
			string response;
			var status = Neo4jRestApi.CreateRelationship(DbUrl,
														startNode.Id,
														endNode.Id,
														relationshipType,
														properties == null ? null : properties.ToString(),
														out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creationg relationship on node (node id:{0} http response:{1})", Id, status));
			}

			return RestRelationshipStore.ParseRelationshipJson(response).First();
		}

		#endregion

	}
}
