using System;
using System.Configuration;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Neo4jRestNet.Core.Interface;
using Neo4jRestNet.Rest;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Neo4jRestNet.Core.Implementation
{
	public class Relationship : IRelationship
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private readonly Neo4jRestApi _restApi;
		private string _selfDbUrl;
		private string _self;
		public INode StartNode { get; private set; }
		public INode EndNode { get; private set; }
		public string Name { get; private set; }
		private Properties _properties;
		public long Id { get; private set; }
		public EncryptId EncryptedId { get; private set; }
		public string OriginalJsonRelationship { get; private set; }

		#region Constructor
		
		public  Relationship()
		{
			_restApi = new Neo4jRestApi(DefaultDbUrl);
		}

		public Relationship(string connectionString)
		{
			_restApi = new Neo4jRestApi(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString.TrimEnd('/'));
		}

		public Relationship(Type nodeType)
		{
			if (nodeType != typeof(INode))
			{
				throw new ArgumentException("Create Relationship with invalid Node type");
			}

			_restApi = new Neo4jRestApi(DefaultDbUrl);
		}
		
		#endregion

		#region GetRelationship

		public IEnumerable<IRelationship> GetRelationship(string indexName, string key, object value)
		{
			string response;
			var status = _restApi.GetRelationship(indexName, key, value, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}
		
		public IEnumerable<IRelationship> GetRelationship(Enum indexName, string key, object value)
		{
			return GetRelationship(indexName.ToString(), key, value);
		}

		public IEnumerable<IRelationship> GetRelationship(string indexName, Enum key, object value)
		{
			return GetRelationship(indexName, key.ToString(), value);
		}

		public IEnumerable<IRelationship> GetRelationship(Enum indexName, Enum key, object value)
		{
			return GetRelationship(indexName.ToString(), key.ToString(), value);
		}

		public IEnumerable<IRelationship> GetRelationship(string indexName, string searchQuery)
		{
			string response;
			var status = _restApi.GetRelationship(indexName, searchQuery, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseJson(response);
		}

		public IEnumerable<IRelationship> GetRelationship(Enum indexName, string searchQuery)
		{
			return GetRelationship(indexName.ToString(), searchQuery);
		}

		#endregion

		#region Initializers 

		public IRelationship InitializeFromRelationshipJson(string relationshipJson)
		{
			JObject jo;

			try
			{
				jo = JObject.Parse(relationshipJson);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid json relationship", e);
			}

			return InitializeFromRelationshipJson(jo);
		}

		public IRelationship InitializeFromRelationshipJson(JObject relationshipJson)
		{
			var relationship = new Relationship();
			JToken self;
			if (!relationshipJson.TryGetValue("self", out self) || self.Type != JTokenType.String)
			{
				throw new Exception("Invalid json relationship");
			}
			
			relationship.Self = self.Value<string>();

			JToken properties;
			if (!relationshipJson.TryGetValue("data", out properties) || properties.Type != JTokenType.Object)
			{
				throw new Exception("Invalid json relationship");
			}

			JToken startNode;
			if (!relationshipJson.TryGetValue("start", out startNode))
			{
				throw new Exception("Invalid json relationship");
			}

			switch (startNode.Type)
			{
				case JTokenType.String:
					relationship.StartNode =  new Node().InitializeFromSelf(startNode.Value<string>());
					break;

				case JTokenType.Object:
					relationship.StartNode = new Node().InitializeFromNodeJson((JObject)startNode);
					break;

				default:
					throw new Exception("Invalid json relationship");
			}

			JToken endNode;
			if (!relationshipJson.TryGetValue("end", out endNode))
			{
				throw new Exception("Invalid json relationship");
			}

			switch (endNode.Type)
			{
				case JTokenType.String:
					relationship.EndNode = new Node().InitializeFromSelf(endNode.Value<string>());
					break;

				case JTokenType.Object:
					relationship.EndNode = new Node().InitializeFromNodeJson((JObject)endNode);
					break;

				default:
					throw new Exception("Invalid json relationship");
			}

			JToken name;
			if (!relationshipJson.TryGetValue("type", out name) || name.Type != JTokenType.String)
			{
				throw new Exception("Invalid json relationship");
			}

			relationship.Name = name.Value<string>();

			relationship._properties = Properties.ParseJson(properties.ToString(Formatting.None));

			relationship.OriginalJsonRelationship = relationshipJson.ToString(Formatting.None);

			return relationship;
		}

		public IRelationship InitializeFromSelf(string self)
		{
			var relationship = new Relationship
			                   	{
			                   		Self = self,
			                   		StartNode = null,
			                   		EndNode = null,
			                   		Name = null,
			                   		_properties = null,
			                   		OriginalJsonRelationship = null
			                   	};


			return relationship;
		}

		public bool IsSelfARelationship(string self)
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
			var status = new Neo4jRestApi(_selfDbUrl).GetPropertiesOnRelationship(_selfDbUrl, Id, out response);
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
			var status = new Neo4jRestApi(_selfDbUrl).SetPropertiesOnRelationship(_selfDbUrl, Id, properties.ToString());
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error setting properties on relationship (relationship id:{0} http response:{1})", Id, status));
			}

			LoadProperties(true);
		}

		#endregion

		#region Index

		public IRelationship AddRelationshipToIndex(long relationshipId, string indexName, string key, object value)
		{
			string response;
			var status = _restApi.AddRelationshipToIndex(relationshipId, indexName, key, value, out response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating index for relationship (http response:{0})", status));
			}

			return InitializeFromRelationshipJson(response);
		}

		public IRelationship AddRelationshipToIndex(long relationshipId, Enum indexName, string key, object value)
		{
			return AddRelationshipToIndex(relationshipId, indexName.ToString(), key, value);
		}

		public IRelationship AddRelationshipToIndex(long relationshipId, string indexName, Enum key, object value)
		{
			return AddRelationshipToIndex(relationshipId, indexName, key.ToString(), value);
		}

		public IRelationship AddRelationshipToIndex(long relationshipId, Enum indexName, Enum key, object value)
		{
			return AddRelationshipToIndex(relationshipId, indexName.ToString(), key.ToString(), value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName)
		{
			var status = _restApi.RemoveRelationshipFromIndex(relationshipId, indexName);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", relationshipId, indexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName)
		{
			return RemoveRelationshipFromIndex(relationshipId, indexName.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, string key)
		{
			var status = _restApi.RemoveRelationshipFromIndex(relationshipId, indexName, key);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", relationshipId, indexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, string key)
		{
			return RemoveRelationshipFromIndex(relationshipId, indexName.ToString(), key);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, Enum key)
		{
			return RemoveRelationshipFromIndex(relationshipId, indexName, key.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, Enum key)
		{
			return RemoveRelationshipFromIndex(relationshipId, indexName.ToString(), key.ToString());
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, string key, object value)
		{
			var status = _restApi.RemoveRelationshipFromIndex(relationshipId, indexName, key, value);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", relationshipId, indexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, string key, object value)
		{
			return RemoveRelationshipFromIndex(relationshipId, indexName.ToString(), key, value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, Enum key, object value)
		{
			return RemoveRelationshipFromIndex(relationshipId, indexName, key.ToString(), value);
		}

		public HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, Enum key, object value)
		{
			return RemoveRelationshipFromIndex(relationshipId, indexName.ToString(), key.ToString(), value);
		}

		#endregion

		#region Delete

		public HttpStatusCode DeleteRelationship()
		{
			var status = new Neo4jRestApi(_selfDbUrl).DeleteRelationship(Id);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error deleteing relationship (relationship id:{0} http response:{1})", Id, status));
			}

			return status;
		}

		#endregion

		#region ParseJson

		public IEnumerable<IRelationship> ParseJson(string jsonRelationships)
		{
			if (String.IsNullOrEmpty(jsonRelationships))
				return null;
			
			var relationships = new List<IRelationship>();

			// The Json passed in can be a JObject or JArray - this is to test for that.
			var jo = JObject.Parse(string.Concat("{\"root\":", jsonRelationships, "}"));

			switch (jo["root"].Type)
			{
				case JTokenType.Object:
					relationships.Add(InitializeFromRelationshipJson(jo["root"].ToString(Formatting.None)));
					break;

				case JTokenType.Array:
					relationships.AddRange(from JObject jsonRelationship in jo["root"] select InitializeFromRelationshipJson(jsonRelationship));
					break;

				default:
					throw new Exception("Invalid json relationship");
			}

			return relationships;
		}

		#endregion
	}
}
