using System;
using System.Net;
using System.Collections.Generic;
using Neo4jRestNet.Configuration;


namespace Neo4jRestNet.Core
{
	public class Relationship : IGraphObject, IEquatable<Relationship>
	{
		private static readonly ConnectionElement DefaultConnection = ConnectionManager.Connection();

		private readonly IRelationshipStore _relationshipGraphStore;
		private Properties _properties;

		private Relationship(Properties properties = null) 
		{
			_properties = properties ?? new Properties();
			_relationshipGraphStore = new RestRelationshipStore();
		}

		public Relationship(IRelationshipStore relationshipStore, Properties properties = null)
		{

			_properties = properties ?? new Properties();
			_relationshipGraphStore = relationshipStore;
		}

		public long Id
		{
			get
			{
				return _relationshipGraphStore.Id;
			}
		}

		public string DbUrl
		{
			get
			{
				return _relationshipGraphStore.DbUrl;
			}
		}

		public EncryptId EncryptedId
		{
			get
			{
				return new EncryptId(_relationshipGraphStore.Id);
			}
		}

		public string Self
		{
			get
			{
				return _relationshipGraphStore.Self;
			}
		}

		public Node StartNode
		{
			get
			{
				return _relationshipGraphStore.StartNode;	
			}
		}

		public Node EndNode
		{
			get
			{
				return _relationshipGraphStore.EndNode;
			}
		}

		public string Type
		{
			get
			{
				return _relationshipGraphStore.Type;
			}
		}

		#region Create

		public static Relationship CreateRelationship(Node startNode, Node endNode, Enum name, Properties properties = null, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			return CreateRelationship(startNode, endNode, name.ToString(), properties, relationshipStore, connection);
		}

		public static Relationship CreateRelationship(Node startNode, Node endNode, string name, Properties properties = null, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			if (properties == null)
			{
				properties = new Properties();
			}

			if (relationshipStore == null)
			{
				relationshipStore = new RestRelationshipStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return relationshipStore.CreateRelationship(connection, startNode, endNode, name, properties);
		}

		public static Relationship CreateUniqueRelationship(Node startNode, Node endNode, Enum name, Enum indexName, Enum key, object value, IndexUniqueness uniqueness, Properties properties = null, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			return CreateUniqueRelationship(startNode, endNode, name.ToString(), indexName.ToString(), key.ToString(), value, uniqueness, properties, relationshipStore, connection);
		}

		public static Relationship CreateUniqueRelationship(Node startNode, Node endNode, string name, string indexName, string key, object value, IndexUniqueness uniqueness, Properties properties = null, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			if (properties == null)
			{
				properties = new Properties();
			}

			if (relationshipStore == null)
			{
				relationshipStore = new RestRelationshipStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return relationshipStore.CreateUniqueRelationship(connection, startNode, endNode, name, properties, indexName, key, value, uniqueness);
		}

		#endregion

		#region GetRelationship

		public static Relationship GetRelationship(EncryptId relationshipId, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			return GetRelationship((long)relationshipId, relationshipStore, connection);
		}

		public static Relationship GetRelationship(long relationshipId, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			if (relationshipStore == null)
			{
				relationshipStore = new RestRelationshipStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return relationshipStore.GetRelationship(connection, relationshipId);
		}

		public static IEnumerable<Relationship> GetRelationship(Enum indexName, Enum key, object value, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			return GetRelationship(indexName.ToString(), key.ToString(), value, relationshipStore, connection);
		}

		public static IEnumerable<Relationship> GetRelationship(string indexName, string key, object value, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			if (relationshipStore == null)
			{
				relationshipStore = new RestRelationshipStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return relationshipStore.GetRelationship(connection, indexName, key, value);
		}

		public static IEnumerable<Relationship> GetRelationship(Enum indexName, string searchQuery, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			return GetRelationship(indexName.ToString(), searchQuery, relationshipStore, connection);
		}
		
		public static IEnumerable<Relationship> GetRelationship(string indexName, string searchQuery, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			if (relationshipStore == null)
			{
				relationshipStore = new RestRelationshipStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return relationshipStore.GetRelationship(connection, indexName, searchQuery);
		}

		#endregion

		#region Delete

		public HttpStatusCode Delete()
		{
			return _relationshipGraphStore.DeleteRelationship(DefaultConnection);
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
				_properties = _relationshipGraphStore.GetProperties();
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
			_relationshipGraphStore.SaveProperties(properties);
		}

		#endregion

		#region Index

		#region Member AddToIndex

		public Relationship AddToIndex(Enum indexName, Enum key, Enum propertyName, bool unique = false)
		{
			return AddToIndex(indexName.ToString(), key.ToString(), Properties.GetProperty(propertyName), unique);
		}

		public  Relationship AddToIndex(Enum indexName, Enum key, object value, bool unique = false)
		{
			return AddToIndex(indexName.ToString(), key.ToString(), value, unique);
		}

		public  Relationship AddToIndex(string indexName, string key, object value, bool unique = false)
		{
			return _relationshipGraphStore.AddToIndex(DefaultConnection, this, indexName, key, value, unique);
		}

		#endregion

		#region Static AddToIndex

		public static Relationship AddToIndex(Relationship relationship, Enum indexName, Enum key, object value, bool unique = false, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			return AddToIndex(relationship, indexName.ToString(), key.ToString(), value, unique, relationshipStore, connection);
		}
		
		public static Relationship AddToIndex(Relationship relationship, string indexName, string key, object value, bool unique = false, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			if (relationshipStore == null)
			{
				relationshipStore = new RestRelationshipStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return relationshipStore.AddToIndex(connection, relationship, indexName, key, value, unique);
		}

		#endregion

		#region Member RemoveFromIndex

		public bool RemoveFromIndex(Enum indexName, string key = null, object value = null)
		{
			return RemoveFromIndex(indexName.ToString(), key, value);
		}

		public bool RemoveFromIndex(string indexName, string key = null, object value = null)
		{
			return key == null ?
				_relationshipGraphStore.RemoveFromIndex(DefaultConnection, this, indexName)
				: value == null ?
					_relationshipGraphStore.RemoveFromIndex(DefaultConnection, this, indexName, key)
					: _relationshipGraphStore.RemoveFromIndex(DefaultConnection, this, indexName, key, value);
		}

		#endregion

		#region Static RemoveFromIndex

		public static bool RemoveFromIndex(Relationship relationship, Enum indexName, Enum key = null, object value = null, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			return RemoveFromIndex(relationship, indexName.ToString(), key == null ? null : key.ToString(), value, relationshipStore, connection);
		}

		public static bool RemoveFromIndex(Relationship relationship, string indexName, string key = null, object value = null, IRelationshipStore relationshipStore = null, ConnectionElement connection = null)
		{
			if (relationshipStore == null)
			{
				relationshipStore = new RestRelationshipStore();
			}

			if (connection == null)
			{
				connection = DefaultConnection;
			}

			return key == null ?
				relationshipStore.RemoveFromIndex(connection, relationship, indexName)
				: value == null ?
					relationshipStore.RemoveFromIndex(connection, relationship, indexName, key)
					: relationshipStore.RemoveFromIndex(connection, relationship, indexName, key, value);
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

		#region IEquatable<Relationship> Members

		public bool Equals(Relationship other)
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
			return Equals(obj as Relationship);
		}

		public override int GetHashCode()
		{
			return (int)Id;
		}

		public static bool operator ==(Relationship value1, Relationship value2)
		{
			if (ReferenceEquals(value1, value2))
			{
				return true;
			}

			return !ReferenceEquals(value1, null) && value1.Equals(value2);
		}

		public static bool operator !=(Relationship value1, Relationship value2)
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
