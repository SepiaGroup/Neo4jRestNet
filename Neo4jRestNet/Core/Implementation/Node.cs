using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System;
using Neo4jRestNet.Core.Interface;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Neo4jRestNet.Rest;

namespace Neo4jRestNet.Core.Implementation
{
	public class Node : INode, IEquatable<Node>
	{
		private enum NodeProperty
		{
			NodeType
		}

		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private readonly Neo4jRestApi _restApi;
		private string _selfDbUrl;
        private string _self;
        private Properties _properties;

		public long Id { get; private set; }
		public EncryptId EncryptedId { get; private set; }
        public string OriginalJsonNode { get; private set; }

		#region Constructor

		public Node()
		{
			_restApi = new Neo4jRestApi(DefaultDbUrl);
		}

		public Node(string connectionString)
		{
			_restApi = new Neo4jRestApi(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString.TrimEnd('/'));
		}

		public Node(Type relationshipType)
		{
			if(relationshipType != typeof(IRelationship))
			{
				throw new ArgumentException("Create Node with invalid Relationship type");
			}

			_restApi = new Neo4jRestApi(DefaultDbUrl);
		}

		#endregion

		#region GetRootNode

		public INode GetRootNode()
		{
			string response;
			var status = _restApi.GetRoot(out response);
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

		public INode GetNode(EncryptId nodeId)
		{
			string response;
			var status = _restApi.GetNode((long)nodeId, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Node not found (node id:{0})", (long)nodeId));
			}

			return InitializeFromNodeJson(response);
		}

		public IEnumerable<INode> GetNode(string indexName, string key, object value)
		{
			string response;
			var status = _restApi.GetNode(indexName, key, value, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}

		public IEnumerable<INode> GetNode(Enum indexName, string key, object value)
		{
			return GetNode(indexName.ToString(), key, value);
		}

		public IEnumerable<INode> GetNode(string indexName, Enum key, object value)
		{
			return GetNode(indexName, key.ToString(), value);
		}

		public IEnumerable<INode> GetNode(Enum indexName, Enum key, object value)
		{
			return GetNode(indexName.ToString(), key.ToString(), value);
		}

		public IEnumerable<INode> GetNode(string connectionName, Enum indexName, string key, object value)
		{
			return GetNode(indexName.ToString(), key, value);
		}

		public IEnumerable<INode> GetNode(string connectionName, string indexName, Enum key, object value)
		{
			return GetNode(indexName, key.ToString(), value);
		}

		public IEnumerable<INode> GetNode(string connectionName, Enum indexName, Enum key, object value)
		{
			return GetNode(indexName.ToString(), key.ToString(), value);
		}

		public IEnumerable<INode> GetNode(string indexName, string searchQuery)
		{
			string response;
			var status = _restApi.GetNode(indexName, searchQuery, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}

		public IEnumerable<INode> GetNode(Enum indexName, string searchQuery)
		{
			return GetNode(indexName.ToString(), searchQuery);
		}

		public IEnumerable<INode> GetNode(string connectionName, Enum indexName, string searchQuery)
		{
			return GetNode(connectionName, indexName.ToString(), searchQuery);
		}

		#endregion

		#region CreateNode

		public INode CreateNode(string nodeType)
		{
			var properties = new Properties();
			properties.SetProperty(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(properties.ToString());
		}

		public INode CreateNode(Enum nodeType)
		{
			return CreateNode(nodeType.ToString());
		}

		public INode CreateNode(string nodeType, Properties properties)
		{
			properties.SetProperty(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(properties.ToString());
		}

		public INode CreateNode(Enum nodeType, Properties properties)
		{
			return CreateNode(nodeType.ToString(), properties);
		}

		public INode CreateNode(string nodeType, IDictionary<string, object> properties)
		{
			properties.Add(NodeProperty.NodeType.ToString(), nodeType);
			return CreateNodeFromJson(JObject.FromObject(properties).ToString(Formatting.None));
		}

		public INode CreateNode(Enum nodeType, IDictionary<string, object> properties)
		{
			return CreateNode(nodeType.ToString(), properties);
		}

		private INode CreateNodeFromJson(string jsonProperties)
		{
			string response;
			var status = _restApi.CreateNode(jsonProperties, out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating node (http response:{0})", status));
			}

			return InitializeFromNodeJson(response);
		}

		#endregion

        #region Delete

        public HttpStatusCode DeleteNode()
        {

			var status = new Neo4jRestApi(_selfDbUrl).DeleteNode(Id);
            if (status != HttpStatusCode.NoContent)
            {
                throw new Exception(string.Format("Error deleting node (node id:{0} http response:{1})", Id, status));
            }

            return status;
        }

        #endregion

		#region Initializers

		public INode InitializeFromNodeJson(string nodeJson)
		{
			JObject jo;

			try
			{
				jo = JObject.Parse(nodeJson);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid json node", e);
			}

			return InitializeFromNodeJson(jo);
		}

		public INode InitializeFromNodeJson(JObject nodeJson)
		{
			JToken self;
			if (!nodeJson.TryGetValue("self", out self))
			{
				throw new Exception("Invalid json node");
			}

			JToken properties;
			if (!nodeJson.TryGetValue("data", out properties))
			{
				throw new Exception("Invalid json node");
			}

			var node = new Node
			               {
			                   Self = self.Value<string>(),
			                   _properties = Properties.ParseJson(properties.ToString(Formatting.None)),
							   OriginalJsonNode = nodeJson.ToString(Formatting.None)
			               };

		    return node;
		}

		public INode InitializeFromSelf(string self)
		{
			var node = new Node {Self = self, _properties = null};
		    return node;
		}

		private bool IsSelfANode(string self)
		{
			var selfArray = self.Split('/');
			return (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node");
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

				_selfDbUrl = self.Substring(0, self.LastIndexOf("/node"));
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
			var status = new Neo4jRestApi(_selfDbUrl).GetPropertiesOnNode((long)EncryptedId, out response);
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

			var status = new Neo4jRestApi(_selfDbUrl).SetPropertiesOnNode((long)EncryptedId, properties.ToString());
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error setting properties on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			LoadProperties(true);
		}

		#endregion

		#region Relationships

		public IEnumerable<IRelationship> GetRelationships()
		{
			return GetRelationships(RelationshipDirection.All, (IEnumerable<string>)null);
		}

		public IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction)
		{
			return GetRelationships(direction, (IEnumerable<string>)null);
		}

		public IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, IRelationship relationshipType)
		{
			return GetRelationships(direction, new List<string> { relationshipType.ToString() });
		}

		public IEnumerable<IRelationship> GetRelationships(Enum name)
		{
			return GetRelationships(RelationshipDirection.All, new List<string> { name.ToString() });
		}

		public IEnumerable<IRelationship> GetRelationships(string name)
		{
			return GetRelationships(RelationshipDirection.All, new List<string> { name });
		}

		public IEnumerable<IRelationship> GetRelationships(IEnumerable<Enum> names)
		{
			return GetRelationships(RelationshipDirection.All, names.Select(n => n.ToString()).ToList());
		}

		public IEnumerable<IRelationship> GetRelationships(IEnumerable<string> names)
		{
			return GetRelationships(RelationshipDirection.All, names);
		}

		public IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, string name)
		{
			return GetRelationships(direction, new List<string> { name });
		}

		public IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, Enum name)
		{
			return GetRelationships(direction, new List<string> { name.ToString() });
		}

		public IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> names)
		{
			string response;
			var status = _restApi.GetRelationshipsOnNode((long)EncryptedId, direction, names, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving relationships on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			return  new Relationship().ParseJson(response);
		}

		public IRelationship CreateRelationshipTo(INode toNode, string relationshipType)
		{
			return CreateRelationshipTo(toNode, relationshipType, null);
		}

		public IRelationship CreateRelationshipTo(INode toNode, Enum relationshipType)
		{
			return CreateRelationshipTo(toNode, relationshipType.ToString(), null);
		}

		public IRelationship CreateRelationshipTo(INode toNode, Enum relationshipType, Properties relationshipProperties)
		{
			return CreateRelationshipTo(toNode, relationshipType.ToString(), relationshipProperties);
		}

		public IRelationship CreateRelationshipTo(INode toNode, string relationshipType, Properties relationshipProperties)
		{
			string response;
			var status = _restApi.CreateRelationship( 
												(long)EncryptedId, 
												toNode.Self, 
												relationshipType, 
												relationshipProperties == null ? null : relationshipProperties.ToString(), 
												out response);

			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creationg relationship on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			return new Relationship().ParseJson(response).First();
		}

		#endregion

		#region Traverse

		public IEnumerable<IGraphObject> Traverse(Order order, Uniqueness uniqueness, IEnumerable<TraverseRelationship> relationships, PruneEvaluator pruneEvaluator, ReturnFilter returnFilter, int? maxDepth, ReturnType returnType)
		{
			string response;
			var status = _restApi.Traverse((long)EncryptedId, order, uniqueness, relationships, pruneEvaluator, returnFilter, maxDepth, returnType, out response);
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
				return new Relationship().ParseJson(response);
		    }

		    if (returnType.ToString() == ReturnType.Path.ToString() || returnType.ToString() == ReturnType.FullPath.ToString())
		    {
		        return new Path().ParseJson(response);
		    }

		    throw new Exception(string.Format("Return type not implemented (type:{0})", returnType));
		}

		#endregion

		#region Index

		public INode AddNodeToIndex(long nodeId, string indexName, string key, object value)
		{
			string response;
			var status = _restApi.AddNodeToIndex(nodeId, indexName, key, value, out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating index for node (http response:{0})", status));
			}

			return InitializeFromNodeJson(response);
		}

		public INode AddNodeToIndex(long nodeId, Enum indexName, string key, object value)
		{
			return AddNodeToIndex(nodeId, indexName.ToString(), key, value);
		}

		public INode AddNodeToIndex(long nodeId, string indexName, Enum key, object value)
		{
			return AddNodeToIndex(nodeId, indexName, key.ToString(), value);
		}

		public INode AddNodeToIndex(long nodeId, Enum indexName, Enum key, object value)
		{
			return AddNodeToIndex(nodeId, indexName.ToString(), key.ToString(), value);
		}

        public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName)
		{
			var status = _restApi.RemoveNodeFromIndex(nodeId, indexName);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", nodeId, indexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName)
		{
			return RemoveNodeFromIndex(nodeId, indexName.ToString());
		}

        public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key)
		{
			var status = _restApi.RemoveNodeFromIndex(nodeId, indexName, key);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", nodeId, indexName, status));
			}

			return status;
		}
		
		public HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, string key)
		{
			return RemoveNodeFromIndex(nodeId, indexName.ToString(), key);
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, Enum key)
		{
			return RemoveNodeFromIndex(nodeId, indexName, key.ToString());
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, Enum key)
		{
			return RemoveNodeFromIndex(nodeId, indexName.ToString(), key.ToString());
		}
		
		public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key, object value)
		{
			var status = _restApi.RemoveNodeFromIndex(nodeId, indexName, key, value);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", nodeId, indexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, string key, object value)
		{
			return RemoveNodeFromIndex(nodeId, indexName.ToString(), key, value);
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, Enum key, object value)
		{
			return RemoveNodeFromIndex(nodeId, indexName, key.ToString(), value);
		}

		public HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, Enum key, object value)
		{
			return RemoveNodeFromIndex(nodeId, indexName.ToString(), key.ToString(), value);
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

		public IEnumerable<INode> ParseJson(string jsonNodes)
		{
			if (String.IsNullOrEmpty(jsonNodes))
			{
			    return null;
			}

			var nodes = new List<INode>();

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
		            throw new Exception("Invalid json node");
		    }

		    return nodes;
		}

		#endregion

		#region IEquatable<INode> Members

		public bool Equals(Node other)
		{
			if (ReferenceEquals(other, null))
				return false;

		    return ReferenceEquals(this, other) || Id.Equals(other.Id);
		}

		public override bool Equals(Object obj)
		{
			return Equals(obj as Node);
		}

		public override int GetHashCode()
		{
			return (int)Id;
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
