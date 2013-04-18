using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using Neo4jRestNet.Configuration;

namespace Neo4jRestNet.Core
{
	public class Node : IGraphObject, IEquatable<Node>
	{
		private static readonly ConnectionElement DefaultConnection = ConnectionManager.Connection();

		private readonly INodeStore _nodeGraphStore;
		private Properties _properties;

		public Node(Properties properties = null)
		{
			_properties = properties;
			_nodeGraphStore = new RestNodeStore();
		}

		public Node(INodeStore nodeStore, Properties properties = null)
		{
			_properties = properties;
			_nodeGraphStore = nodeStore;
		}

		public long Id
		{
			get
			{
				return _nodeGraphStore.Id;
			}
		}

		public string DbUrl
		{
			get
			{
				return _nodeGraphStore.DbUrl;
			}
		}

		public EncryptId EncryptedId
		{
			get
			{
				return new EncryptId(_nodeGraphStore.Id);
			}
		}

		public string Self
		{
			get
			{
				return _nodeGraphStore.Self;
			}
		}

		#region CreateNode

		public static Node CreateNode()
		{
			return CreateNode(null, null, null);
		}

		public static Node CreateNode(Properties properties, ConnectionElement connection = null)
		{
			return CreateNode(properties, null, connection);
		}

		public static Node CreateNode(INodeStore nodeStore, ConnectionElement connection = null)
		{
			return CreateNode(null, nodeStore, connection);
		}

		public static Node CreateNode(Properties properties, INodeStore nodeStore, ConnectionElement connection = null)
		{
			if (properties == null)
			{
				properties = new Properties();
			}
			
			if(nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if(connection == null)
			{
				connection = DefaultConnection;
			}

			return nodeStore.CreateNode(connection, properties);
		}

		public static Node CreateUniqueNode(Enum indexName, Enum key, object value, IndexUniqueness uniqueness, Properties properties = null, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			return CreateUniqueNode(indexName.ToString(), key.ToString(), value, uniqueness, properties, nodeStore, connection);
		}

		public static Node CreateUniqueNode(string indexName, string key, object value, IndexUniqueness uniqueness, Properties properties = null, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (properties == null)
			{
				properties = new Properties();
			}

			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return nodeStore.CreateUniqueNode(connection, properties, indexName, key, value, uniqueness);
		}

		#endregion

		#region Initilize

		public static Node Initilize(Properties properties = null, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (properties == null)
			{
				properties = new Properties();
			}

			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return nodeStore.Initilize(connection.DbUrl, properties);
		}

		public static Node Initilize(long id, Properties properties, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			} 
			
			return nodeStore.Initilize(connection, id, properties);
		}

		public static Node Initilize(Properties properties, string selfUri, INodeStore nodeStore = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			return nodeStore.Initilize(selfUri, properties);
		}

		#endregion

		#region GetRootNode

		public static Node GetRootNode()
		{
			return GetRootNode(new RestNodeStore(), DefaultConnection);
		}

		public static Node GetRootNode(ConnectionElement connection)
		{
			return GetRootNode(new RestNodeStore(), connection);
		}

		public static Node GetRootNode(INodeStore nodeStore)
		{
			return GetRootNode(nodeStore, DefaultConnection);
		}

		public static Node GetRootNode(INodeStore nodeStore, ConnectionElement connection)
		{
			return nodeStore.GetRootNode(connection);
		}

		#endregion

		#region GetNode

		public static Node GetNode(EncryptId nodeId, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			return GetNode((long)nodeId, nodeStore, connection);
		}

		public static Node GetNode(long nodeId, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return nodeStore.GetNode(connection, nodeId);
		}

		public static IEnumerable<Node> GetNode(Enum indexName, Enum key, object value, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			return GetNode(indexName.ToString(), key.ToString(), value, nodeStore, connection);
		}

		public static IEnumerable<Node> GetNode(string indexName, string key, object value, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return nodeStore.GetNode(connection, indexName, key, value);
		}

		public static IEnumerable<Node> GetNode(Enum indexName, string searchQuery, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			return GetNode(indexName.ToString(), searchQuery, nodeStore, connection);
		}

		public static IEnumerable<Node> GetNode(string indexName, string searchQuery, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return nodeStore.GetNode(connection, indexName, searchQuery);
		}

		#endregion

		#region Delete

		public HttpStatusCode Delete()
		{
			return _nodeGraphStore.DeleteNode();
		}

		#endregion

		#region Properties

		private void LoadProperties(bool refresh)
		{
			if (refresh)
			{
				_properties = null;
			}

			if (_properties == null)
			{
				_properties = _nodeGraphStore.GetProperties();
			}
		}

		public Properties Properties
		{
			get
			{
				LoadProperties(false);
				return _properties;
			}

			internal set
			{
				_properties = value;
			}
		}

		public void SaveProperties()
		{
			SaveProperties(Properties);
		}

		public void SaveProperties(Properties properties)
		{
			_nodeGraphStore.SaveProperties(properties);
		}

		#endregion

		#region Relationships

		public IEnumerable<Relationship> GetRelationships()
		{
			return GetRelationships(RelationshipDirection.All, (IEnumerable<string>)null);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction)
		{
			return GetRelationships(direction, (IEnumerable<string>)null);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, Relationship relationshipType)
		{
			return GetRelationships(direction, new List<string> { relationshipType.ToString() });
		}

		public IEnumerable<Relationship> GetRelationships(Enum name)
		{
			return GetRelationships(RelationshipDirection.All, new List<string> { name.ToString() });
		}

		public IEnumerable<Relationship> GetRelationships(string name)
		{
			return GetRelationships(RelationshipDirection.All, new List<string> { name });
		}

		public IEnumerable<Relationship> GetRelationships(IEnumerable<Enum> names)
		{
			return GetRelationships(RelationshipDirection.All, names.Select(n => n.ToString()).ToList());
		}

		public IEnumerable<Relationship> GetRelationships(IEnumerable<string> names)
		{
			return GetRelationships(RelationshipDirection.All, names);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, string name)
		{
			return GetRelationships(direction, new List<string> { name });
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, Enum name)
		{
			return GetRelationships(direction, new List<string> { name.ToString() });
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> names)
		{
			return _nodeGraphStore.GetRelationships(direction, names);
		}

		public Relationship CreateRelationshipTo(Node endNode, Enum relationshipType, Properties relationshipProperties = null)
		{
			return CreateRelationshipTo(endNode, relationshipType.ToString(), relationshipProperties);
		}

		public Relationship CreateRelationshipTo(Node endNode, string relationshipType, Properties properties = null)
		{
			return _nodeGraphStore.CreateRelationship(this, endNode, relationshipType, properties);
		}

		#endregion

		#region Index

		#region Member AddToIndex

		public Node AddToIndex(Enum indexName, Enum key, Enum propertyName)
		{
			return AddToIndex(indexName.ToString(), key.ToString(), Properties.GetProperty(propertyName));
		}

		public Node AddToIndex(Enum indexName, Enum key, object value)
		{
			return AddToIndex(indexName.ToString(), key.ToString(), value);
		}

		public Node AddToIndex(string indexName, string key, object value)
		{
			return _nodeGraphStore.AddToIndex(DefaultConnection, this, indexName, key, value);
		}

		#endregion

		#region Static AddToIndex

		public static Node AddToIndex(Node node, Enum indexName, Enum key, object value, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			return AddToIndex(node, indexName.ToString(), key.ToString(), value, nodeStore, connection);
		}

		public static Node AddToIndex(Node node, string indexName, string key, object value, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return nodeStore.AddToIndex(connection, node, indexName, key, value);
		}

		#endregion

		#region Member RemoveFromIndex

		public bool RemoveFromIndex(Enum indexName, Enum key = null, object value = null)
		{
			return RemoveFromIndex(indexName.ToString(), key == null ? null : key.ToString(), value);
		}

		public bool RemoveFromIndex(string indexName, string key = null, object value = null)
		{
			return string.IsNullOrWhiteSpace(key) ?
				_nodeGraphStore.RemoveFromIndex(DefaultConnection, this, indexName)
				: value == null ?
					_nodeGraphStore.RemoveFromIndex(DefaultConnection, this, indexName, key)
					: _nodeGraphStore.RemoveFromIndex(DefaultConnection, this, indexName, key, value);
		}
		
		#endregion

		#region Static RemoveFromIndex

		public static bool RemoveFromIndex(Node node, Enum indexName, Enum key = null, object value = null, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			return RemoveFromIndex(node, indexName.ToString(), key == null ? null : key.ToString(), value, nodeStore, connection);
		}

		public static bool RemoveFromIndex(Node node, string indexName, string key = null, object value = null, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return key == null ?
				nodeStore.RemoveFromIndex(connection, node, indexName)
				: value == null ?
					nodeStore.RemoveFromIndex(connection, node, indexName, key)
					: nodeStore.RemoveFromIndex(connection, node, indexName, key, value);
		}

		#endregion

		#endregion

		#region Property Methods

		public T GetProperty<T>(string key)
		{
			return _properties.GetProperty<T>(key);
		}

		public T GetProperty<T>(Enum key)
		{
			return _properties.GetProperty<T>(key);
		}

		public object GetProperty(string key)
		{
			return _properties.GetProperty(key);
		}

		public object GetProperty(Enum key)
		{
			return _properties.GetProperty(key);
		}

		public T GetPropertyOrDefault<T>(string key)
		{
			return _properties.GetPropertyOrDefault<T>(key);
		}

		public T GetPropertyOrDefault<T>(Enum key)
		{
			return _properties.GetPropertyOrDefault<T>(key);
		}

		public T GetPropertyOrOther<T>(string key, T otherValue)
		{
			return _properties.GetPropertyOrOther<T>(key, otherValue);
		}

		public T GetPropertyOrOther<T>(Enum key, T otherValue)
		{
			return _properties.GetPropertyOrOther<T>(key, otherValue);
		}

		public IEnumerable<string> GetPropertyKeys()
		{
			return _properties.GetPropertyKeys();
		}

		public bool HasProperty(string key)
		{
			return _properties.HasProperty(key);
		}

		public bool HasProperty(Enum key)
		{
			return _properties.HasProperty(key);
		}

		public object RemoveProperty(string key)
		{
			return _properties.RemoveProperty(key);
		}

		public object RemoveProperty(Enum key)
		{
			return _properties.RemoveProperty(key);
		}

		public void SetProperty<T>(string key, T value)
 		{
			_properties.SetProperty(key, value);
 		}
 
		public void SetProperty<T>(Enum key, T value)
 		{
			_properties.SetProperty(key, value);
 		}
 

		#endregion

		#region Static Property Methods

		public static void SetProperty<T>(Node node, string key, T value, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			if (nodeStore == null)
			{
				nodeStore = new RestNodeStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			nodeStore.SetProperty(node, key, value);
		}

		public static void SetProperty<T>(Node node, Enum key, T value, INodeStore nodeStore = null, ConnectionElement connection = null)
		{
			SetProperty(node, key.ToString(), value, nodeStore, connection);
		}

		#endregion

		#region IEquatable<Node> Members

		public bool Equals(Node other)
		{
			if (ReferenceEquals(other, null))
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (Id == int.MinValue || other.Id == int.MinValue)
				return false;

			return EncryptedId.Equals(other.EncryptedId);
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
