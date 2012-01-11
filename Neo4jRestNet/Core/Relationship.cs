using System;
using System.Net;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using Neo4jRestNet.Rest;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace Neo4jRestNet.Core
{
	public class Relationship : IGraphObject
	{
		private const string DefaultConnectionName = "neo4j";
		private string _selfDbUrl;
		private string _self;
		public Node StartNode { get; private set; }
		public Node EndNode { get; private set; }
		public string Name { get; private set; }
		private Properties _properties;
		public long Id { get; private set; }
		public EncryptId EncryptedId { get; private set; }
		public string OriginalRelationshipJson { get; private set; }

		private Relationship() { }

		#region GetRelationship

		public static IEnumerable<Relationship> GetRelationship(string indexName, string key, object value)
		{
			return GetRelationship(DefaultConnectionName, indexName, key, value);
		}
		
		public static IEnumerable<Relationship> GetRelationship(Enum indexName, string key, object value)
		{
			return GetRelationship(DefaultConnectionName, indexName.ToString(), key, value);
		}

		public static IEnumerable<Relationship> GetRelationship(string indexName, Enum key, object value)
		{
			return GetRelationship(DefaultConnectionName, indexName, key.ToString(), value);
		}
		
		public static IEnumerable<Relationship> GetRelationship(Enum indexName, Enum key, object value)
		{
			return GetRelationship(DefaultConnectionName, indexName.ToString(), key.ToString(), value);
		}

		public static IEnumerable<Relationship> GetRelationship(string connectionName, Enum indexName, Enum key, object value)
		{
			return GetRelationship(connectionName, indexName.ToString(), key.ToString(), value);
		}

		public static IEnumerable<Relationship> GetRelationship(string connectionName, string indexName, string key, object value)
		{
			string response;
			HttpStatusCode status = new StoreFactory().CreateNeo4jRestApi(connectionName).GetRelationship(connectionName, indexName, key, value, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}

		public static IEnumerable<Relationship> GetRelationship(string indexName, string searchQuery)
		{
			return GetRelationship(DefaultConnectionName, indexName, searchQuery);
		}

		public static IEnumerable<Relationship> GetRelationship(Enum indexName, string searchQuery)
		{
			return GetRelationship(DefaultConnectionName, indexName.ToString(), searchQuery);
		}

		public static IEnumerable<Relationship> GetRelationship(string connectionName, Enum indexName, string searchQuery)
		{
			return GetRelationship(connectionName, indexName.ToString(), searchQuery);
		}

		public static IEnumerable<Relationship> GetRelationship(string connectionName, string indexName, string searchQuery)
		{
			string response;
			HttpStatusCode status = new StoreFactory().CreateNeo4jRestApi(connectionName).GetRelationship(connectionName, indexName, searchQuery, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}

		#endregion

		#region Initializers 

		public static Relationship InitializeFromRelationshipJson(string relationshipJson)
		{
			JObject jo;

			try
			{
				jo = JObject.Parse(relationshipJson);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid relationship json", e);
			}

			return InitializeFromRelationshipJson(jo);
		}

		public static Relationship InitializeFromRelationshipJson(JObject relationshipJson)
		{
			var relationship = new Relationship();
			JToken self;
			if (!relationshipJson.TryGetValue("self", out self) || self.Type != JTokenType.String)
			{
				throw new Exception("Invalid relationship json");
			}
			
			relationship.Self = self.Value<string>();

			JToken properties;
			if (!relationshipJson.TryGetValue("data", out properties) || properties.Type != JTokenType.Object)
			{
				throw new Exception("Invalid relationship json");
			}

			JToken startNode;
			if (!relationshipJson.TryGetValue("start", out startNode))
			{
				throw new Exception("Invalid relationship json");
			}

			switch (startNode.Type)
			{
				case JTokenType.String:
					relationship.StartNode = Node.InitializeFromSelf(startNode.Value<string>());
					break;

				case JTokenType.Object:
					relationship.StartNode = Node.InitializeFromNodeJson((JObject)startNode);
					break;

				default:
					throw new Exception("Invalid relationship json");
			}

			JToken endNode;
			if (!relationshipJson.TryGetValue("end", out endNode))
			{
				throw new Exception("Invalid relationship json");
			}

			switch (endNode.Type)
			{
				case JTokenType.String:
					relationship.EndNode = Node.InitializeFromSelf(endNode.Value<string>());
					break;

				case JTokenType.Object:
					relationship.EndNode = Node.InitializeFromNodeJson((JObject)endNode);
					break;

				default:
					throw new Exception("Invalid relationship json");
			}

			JToken name;
			if (!relationshipJson.TryGetValue("type", out name) || name.Type != JTokenType.String)
			{
				throw new Exception("Invalid relationship json");
			}

			relationship.Name = name.Value<string>();

			relationship._properties = Properties.ParseJson(properties.ToString(Formatting.None));

			relationship.OriginalRelationshipJson = relationshipJson.ToString(Formatting.None);

			return relationship;
		}

		public static Relationship InitializeFromSelf(string self)
		{
			var relationship = new Relationship
			                   	{
			                   		Self = self,
			                   		StartNode = null,
			                   		EndNode = null,
			                   		Name = null,
			                   		_properties = null,
			                   		OriginalRelationshipJson = null
			                   	};


			return relationship;
		}

		public static bool IsSelfARelationship(string self)
		{
			var selfArray = self.Split('/');
			return (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "relationship");
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
				if (!IsSelfARelationship(value))
				{
					throw new Exception(string.Format("Self is not a Relationship ({0})", Self));
				}

				// Make sure there is no trailing /
				string self = value.TrimEnd('/');

				var selfArray = self.Split('/');

				long relationshipId;
				if (!long.TryParse(selfArray.Last(), out relationshipId))
				{
					throw new Exception(string.Format("Invalid Self id ({0})", value));
				}

				_selfDbUrl = self.Substring(0, self.LastIndexOf("/relationship"));
				_self = self;
				Id = relationshipId;
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
			var status = new StoreFactory().CreateNeo4jRestApi(_selfDbUrl).GetPropertiesOnRelationship(_selfDbUrl, Id, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving properties on relationship (relationship id:{0} http response:{1})", Id, status));
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
			var status = new StoreFactory().CreateNeo4jRestApi(_selfDbUrl).SetPropertiesOnRelationship(_selfDbUrl, Id, properties.ToString());
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error setting properties on relationship (relationship id:{0} http response:{1})", Id, status));
			}

			LoadProperties(true);
		}

		#endregion

		#region Index

		public static Relationship AddRelationshipToIndex(long relationshipId, string indexName, string key, object value)
		{
			return AddRelationshipToIndex(DefaultConnectionName, relationshipId, indexName, key, value);
		}

		public static Relationship AddRelationshipToIndex(long relationshipId, Enum indexName, string key, object value)
		{
			return AddRelationshipToIndex(DefaultConnectionName, relationshipId, indexName.ToString(), key, value);
		}

		public static Relationship AddRelationshipToIndex(long relationshipId, string indexName, Enum key, object value)
		{
			return AddRelationshipToIndex(DefaultConnectionName, relationshipId, indexName, key.ToString(), value);
		}

		public static Relationship AddRelationshipToIndex(long relationshipId, Enum indexName, Enum key, object value)
		{
			return AddRelationshipToIndex(DefaultConnectionName, relationshipId, indexName.ToString(), key.ToString(), value);
		}

		public static Relationship AddRelationshipToIndex(string connectionName, long relationshipId, Enum indexName, Enum key, object value)
		{
			return AddRelationshipToIndex(connectionName, relationshipId, indexName.ToString(), key.ToString(), value);
		}

		public static Relationship AddRelationshipToIndex(string connectionName, long relationshipId, string indexName, string key, object value)
		{
			string response;
			var status = new StoreFactory().CreateNeo4jRestApi(connectionName).AddRelationshipToIndex(connectionName, relationshipId, indexName, key, value, out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating index for relationship (http response:{0})", status));
			}

			return InitializeFromRelationshipJson(response);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string connectionName, long relationshipId, Enum indexName)
		{
			return RemoveRelationshipFromIndex(connectionName, relationshipId, indexName.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string connectionName, long relationshipId, string indexName)
		{
			var status = new StoreFactory().CreateNeo4jRestApi(connectionName).RemoveRelationshipFromIndex(connectionName, relationshipId, indexName);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", relationshipId, indexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, string key)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName, key);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, string key)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName.ToString(), key);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, Enum key)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName, key.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, Enum key)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName.ToString(), key.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string connectionName, long relationshipId, Enum indexName, Enum key)
		{
			return RemoveRelationshipFromIndex(connectionName, relationshipId, indexName.ToString(), key.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string connectionName, long relationshipId, string indexName, string key)
		{
			var status = new StoreFactory().CreateNeo4jRestApi(connectionName).RemoveRelationshipFromIndex(connectionName, relationshipId, indexName, key);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", relationshipId, indexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, string key, object value)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName, key, value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, string key, object value)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName.ToString(), key, value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, Enum key, object value)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName, key.ToString(), value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, Enum key, object value)
		{
			return RemoveRelationshipFromIndex(DefaultConnectionName, relationshipId, indexName.ToString(), key.ToString(), value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string connectionName, long relationshipId, Enum indexName, Enum key, object value)
		{
			return RemoveRelationshipFromIndex(connectionName, relationshipId, indexName.ToString(), key.ToString(), value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(string connectionName, long relationshipId, string indexName, string key, object value)
		{
			var status = new StoreFactory().CreateNeo4jRestApi(connectionName).RemoveRelationshipFromIndex(connectionName, relationshipId, indexName, key, value);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", relationshipId, indexName, status));
			}

			return status;
		}
		#endregion

		#region Delete

		public HttpStatusCode DeleteRelationship()
		{
			var status = new StoreFactory().CreateNeo4jRestApi(_selfDbUrl).DeleteRelationship(_selfDbUrl, Id);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error deleteing relationship (relationship id:{0} http response:{1})", Id, status));
			}

			return status;
		}

		#endregion

		#region ParseJson

		public static IEnumerable<Relationship> ParseJson(string jsonRelationships)
		{
			if (String.IsNullOrEmpty(jsonRelationships))
				return null;
			
			var relationships = new List<Relationship>();

			// The Json passed in can be a JObject or JArray - this is to test for that.
			JObject jo = JObject.Parse(string.Concat("{\"root\":", jsonRelationships, "}"));

			switch (jo["root"].Type)
			{
				case JTokenType.Object:
					relationships.Add(InitializeFromRelationshipJson(jo["root"].ToString(Formatting.None)));
					break;

				case JTokenType.Array:
					relationships.AddRange(from JObject jsonRelationship in jo["root"] select InitializeFromRelationshipJson(jsonRelationship));
					break;

				default:
					throw new Exception("Invalid relationship json");
			}

			return relationships;
		}

		#endregion
	}
}
