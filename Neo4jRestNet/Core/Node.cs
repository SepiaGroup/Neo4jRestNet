using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Neo4jRestNet.Rest;
using System.Configuration;

namespace Neo4jRestNet.Core
{
	public class Node : IGraphObject, IEquatable<Node>
	{
		private enum NodeProperty
		{
			NodeType
		}

		private static readonly string _defaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');

		private string _dbUrl;
        private string _self;
        private Properties _properties;

		public long Id { get; private set; }
		public EncryptId EncryptedId { get; private set; }
        public string OriginalNodeJson { get; private set; }

		protected Node() { }

		#region GetRootNode

		public static Node GetRootNode()
		{
			return GetRootNode(_defaultDbUrl);
		}

		public static Node GetRootNode(string dbUrl)
		{
			string response;
			var status = Neo4jRestApi.GetRoot(dbUrl, out response);
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
				throw new Exception("Invalid json");
			}

			var node = new Node {Self = referenceNode.Value<string>(), _properties = null};

		    return node;
		}

		#endregion

		#region GetNode

		public static Node GetNode(EncryptId nodeId)
		{
			return GetNode(_defaultDbUrl, nodeId);
		}

		public static Node GetNode(string dbUrl, EncryptId nodeId)
		{
			string response;
			var status = Neo4jRestApi.GetNode(dbUrl, (long)nodeId, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Node not found (node id:{0})", (long)nodeId));
			}

			return InitializeFromNodeJson(response);
		}

		public static IEnumerable<Node> GetNode(string indexName, string key, object value)
		{
			return GetNode(_defaultDbUrl, indexName, key, value);
		}

		public static IEnumerable<Node> GetNode(string dbUrl, string indexName, string key, object value)
		{
			string response;
			var status = Neo4jRestApi.GetNode(dbUrl, indexName, key, value, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}

		public static IEnumerable<Node> GetNode(string indexName, string searchQuery)
		{
			return GetNode(_defaultDbUrl, indexName, searchQuery);
		}

		public static IEnumerable<Node> GetNode(string dbUrl, string indexName, string searchQuery)
		{
			string response;
			var status = Neo4jRestApi.GetNode(dbUrl, indexName, searchQuery, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}

		#endregion

		#region CreateNode

		public static Node CreateNode(string nodeType)
		{
			var properties = new Properties();
			properties.SetProperty(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(_defaultDbUrl, properties.ToString());
		}

		public static Node CreateNode(string dbUrl, string nodeType)
		{
			var properties = new Properties();
			properties.SetProperty(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(dbUrl, properties.ToString());
		}

		public static Node CreateNode(string nodeType, Properties properties)
		{
			properties.SetProperty(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(_defaultDbUrl, properties.ToString());
		}

		public static Node CreateNode(string dbUrl, string nodeType, Properties properties)
		{
			properties.SetProperty(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(dbUrl, properties.ToString());
		}

		public static Node CreateNode(string nodeType, IDictionary<string, object> properties)
		{
			properties.Add(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(_defaultDbUrl, JObject.FromObject(properties).ToString(Formatting.None));
		}

		public static Node CreateNode(string dbUrl, string nodeType, IDictionary<string, object> properties)
		{
			properties.Add(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(dbUrl, JObject.FromObject(properties).ToString(Formatting.None));
		}

		private static Node CreateNodeFromJson(string dbUrl, string jsonProperties)
		{
			string response;
			var status = Neo4jRestApi.CreateNode(dbUrl, jsonProperties, out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating node (http response:{0})", status));
			}

			return InitializeFromNodeJson(response);
		}

		#endregion

        #region Delete

        public HttpStatusCode Delete()
        {
            var status = Neo4jRestApi.DeleteNode(_dbUrl, Id);
            if (status != HttpStatusCode.NoContent)
            {
                throw new Exception(string.Format("Error deleting node (node id:{0} http response:{1})", Id, status));
            }

            return status;
        }

        #endregion

		#region Initializers

		public static Node InitializeFromNodeJson(string nodeJson)
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

			return InitializeFromNodeJson(jo);
		}

		public static Node InitializeFromNodeJson(JObject nodeJson)
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

			var node = new Node
			               {
			                   Self = self.Value<string>(),
			                   _properties = Properties.ParseJson(properties.ToString(Formatting.None)),
			                   OriginalNodeJson = nodeJson.ToString(Formatting.None)
			               };

		    return node;
		}

		public static Node InitializeFromSelf(string self)
		{
			var node = new Node {Self = self, _properties = null};
		    return node;
		}

		public static bool IsSelfANode(string self)
		{
			var selfArray = self.Split('/');
			return (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node") ? true : false;
		}

		#endregion

		#region Self

		public string Self
		{
			get
			{
				return _self;
			}

			private set
			{
				if (!IsSelfANode(value))
				{
					throw new Exception(string.Format("Self is not a Node ({0})", Self));
				}

				// Make sure there is no trailing /
				var self = value.TrimEnd('/');

				var selfArray = self.Split('/');

				long nodeId;
				if (!long.TryParse(selfArray.Last(), out nodeId))
				{
					throw new Exception(string.Format("Invalid Self id ({0})", value));
				}

				_dbUrl = self.Substring(0, self.LastIndexOf("/node"));
				_self = self;

				// Set Id & NodeId values
				Id = nodeId;
				EncryptedId = nodeId;
			}
		}

		#endregion

		#region Properties

		private void LoadProperties(bool refresh)
		{
			if (refresh)
			{
				_properties = null;
			}

			if (_properties != null)
			{
				return;
			}

			string response;
			var status = Neo4jRestApi.GetPropertiesOnNode(_dbUrl, (long)EncryptedId, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving properties on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			_properties = Properties.ParseJson(response);
		}

		public Properties Properties
		{
			get
			{
				LoadProperties(false);
				return _properties;
			}
		}

		public void SaveProperties()
		{
			SaveProperties(Properties);
		}

		public void SaveProperties(Properties properties)
		{
			LoadProperties(false);
			if (Properties.HasProperty(NodeProperty.NodeType.ToString()))
			{
				properties.SetProperty(NodeProperty.NodeType.ToString(), Properties.GetProperty<string>(NodeProperty.NodeType.ToString()));
			}

			var status = Neo4jRestApi.SetPropertiesOnNode(_dbUrl, (long)EncryptedId, properties.ToString());
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error setting properties on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			LoadProperties(true);
		}

		#endregion

		#region Relationships

		public IEnumerable<Relationship> GetRelationships()
		{
			IEnumerable<string> names = null;
			return GetRelationships(RelationshipDirection.All, names);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction)
		{
			IEnumerable<string> names = null;
			return GetRelationships(direction, names);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, Relationship relationshipType)
		{
			return GetRelationships(direction, new List<string> { relationshipType.ToString() });
		}

		public IEnumerable<Relationship> GetRelationships(string name)
		{
			return GetRelationships(RelationshipDirection.All, new List<string> { name });
		}

		public IEnumerable<Relationship> GetRelationships(IEnumerable<string> names)
		{
			return GetRelationships(RelationshipDirection.All, names);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, string name)
		{
			return GetRelationships(direction, new List<string> { name });
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> names)
		{
			string response;
			var status = Neo4jRestApi.GetRelationshipsOnNode(_dbUrl, (long)EncryptedId, direction, names, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving relationships on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			return Relationship.ParseJson(response);
		}

		public Relationship CreateRelationshipTo(Node toNode, string relationshipType)
		{
			return CreateRelationshipTo(toNode, relationshipType, null);
		}

		public Relationship CreateRelationshipTo(Node toNode, string relationshipType, Properties relationshipProperties)
		{
			string response;
			var status = Neo4jRestApi.CreateRelationship(_dbUrl, 
																	(long)EncryptedId, 
																	toNode.Self, 
																	relationshipType, 
																	relationshipProperties == null ? null : relationshipProperties.ToString(), 
																	out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creationg relationship on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			return Relationship.ParseJson(response).First();
		}

		#endregion

		#region Traverse

		public IEnumerable<IGraphObject> Traverse(Order order, Uniqueness uniqueness, IEnumerable<TraverseRelationship> relationships, PruneEvaluator pruneEvaluator, ReturnFilter returnFilter, int? maxDepth, ReturnType returnType)
		{
			string response;
			var status = Neo4jRestApi.Traverse(_dbUrl, (long)EncryptedId, order, uniqueness, relationships, pruneEvaluator, returnFilter, maxDepth, returnType, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error traversing nodes (node id:{0} status code:{1})", (long)EncryptedId, status));
			}

			if (returnType.ToString() == ReturnType.Node.ToString())
			{
				return ParseJson(response);
			}

		    if (returnType.ToString() == ReturnType.Relationship.ToString())
		    {
		        return Relationship.ParseJson(response);
		    }

		    if (returnType.ToString() == ReturnType.Path.ToString() || returnType.ToString() == ReturnType.FullPath.ToString())
		    {
		        return Path.ParseJson(response);
		    }

		    throw new Exception(string.Format("Return type not implemented (type:{0})", returnType));
		}

		#endregion

		#region Index

		public static Node AddNodeToIndex(long nodeId, string indexName, string key, object value)
		{
			return AddNodeToIndex(_defaultDbUrl, nodeId, indexName, key, value);
		}

		public static Node AddNodeToIndex(string dbUrl, long nodeId, string indexName, string key, object value)
		{
			string response;
			var status = Neo4jRestApi.AddNodeToIndex(dbUrl, nodeId, indexName, key, value, out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating index for node (http response:{0})", status));
			}

			return InitializeFromNodeJson(response);
		}

        public static HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName)
		{
			return RemoveNodeFromIndex(_defaultDbUrl, nodeId, indexName);
		}

        public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName)
		{
			var status = Neo4jRestApi.RemoveNodeFromIndex(dbUrl, nodeId, indexName);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", nodeId, indexName, status));
			}

			return status;
		}

        public static HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key)
		{
			return RemoveNodeFromIndex(_defaultDbUrl, nodeId, indexName, key);
		}

        public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName, string key)
		{
			var status = Neo4jRestApi.RemoveNodeFromIndex(dbUrl, nodeId, indexName, key);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", nodeId, indexName, status));
			}

			return status;
		}

        public static HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key, object value)
		{
			return RemoveNodeFromIndex(_defaultDbUrl, nodeId, indexName, key, value);
		}

        public static HttpStatusCode RemoveNodeFromIndex(string dbUrl, long nodeId, string indexName, string key, object value)
		{
			var status = Neo4jRestApi.RemoveNodeFromIndex(dbUrl, nodeId, indexName, key, value);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", nodeId, indexName, status));
			}

			return status;
		}
		#endregion

		#region NodeType

		public string NodeType
		{
			get
			{
			    return _properties == null ? null : _properties.GetProperty<string>(NodeProperty.NodeType.ToString());
			}
		}

		#endregion

		#region ParseJson

		public static IEnumerable<Node> ParseJson(string jsonNodes)
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
		            nodes.Add(InitializeFromNodeJson(jo["root"].ToString(Formatting.None)));
		            break;

		        case JTokenType.Array:
		            nodes.AddRange(from JObject jsonNode in jo["root"] select InitializeFromNodeJson(jsonNode));
		            break;

		        default:
		            throw new Exception("Invalid node json");
		    }

		    return nodes;
		}

		#endregion

		#region IEquatable<Node> Members

		public bool Equals(Node other)
		{
			if (ReferenceEquals(other, null))
				return false;

		    if (ReferenceEquals(this, other))
		        return true;

		    if (EncryptedId == null || other.EncryptedId == null)
		        return false;

		    return EncryptedId.Equals(other.EncryptedId);
		}

		public override bool Equals(Object obj)
		{
			return Equals(obj as Node);
		}

		public override int GetHashCode()
		{
			return (int)EncryptedId;
		}

		public static bool operator ==(Node value1, Node value2)
		{
			if (ReferenceEquals(value1, value2))
			{
				return true;
			}

			return !ReferenceEquals(value1, null) && value1.Equals(value2);
		}

		public static bool operator !=(Node value1, Node value2)
		{
			if (ReferenceEquals(value1, value2))
			{
				return false;
			}

			if (ReferenceEquals(value1, null)) // Value2=null is covered by Equals
			{
				return false;
			}

			return !value1.Equals(value2);
		}

		#endregion
	}
}