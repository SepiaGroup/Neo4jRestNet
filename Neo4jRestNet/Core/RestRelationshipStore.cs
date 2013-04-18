using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Neo4jRestNet.Configuration;
using Neo4jRestNet.Core.Exceptions;
using Neo4jRestNet.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Core
{
	public class RestRelationshipStore : IRelationshipStore
	{
		public long Id { get; private set; }
		public string DbUrl { get; private set; }
		public string Self { get; private set; }
		public string OriginalRelationshipJson {get; private set; }
		private Node _startNode;
		private Node _endNode;
		private string _type;

		public Node StartNode
		{
			get
			{
				if(_startNode != null)
				{
					return _startNode;
				}

				if(string.IsNullOrWhiteSpace(OriginalRelationshipJson))
				{
					return null;
				}

				var joRelationship = JObject.Parse(OriginalRelationshipJson);

				JToken startNode;
				if (!joRelationship.TryGetValue("start", out startNode))
				{
					throw new Exception("Invalid relationship json");
				}

				if( startNode.Type != JTokenType.String)
				{
					throw new Exception("Invalid relationship json");
				}

				_startNode = Node.Initilize(null, startNode.ToString());

				return _startNode;
			}
		}

		public Node EndNode
		{
			get
			{
				if (_endNode != null)
				{
					return _endNode;
				}

				if (string.IsNullOrWhiteSpace(OriginalRelationshipJson))
				{
					return null;
				}

				var joRelationship = JObject.Parse(OriginalRelationshipJson);

				JToken endNode;
				if (!joRelationship.TryGetValue("end", out endNode))
				{
					throw new Exception("Invalid relationship json");
				}

				if (endNode.Type != JTokenType.String)
				{
					throw new Exception("Invalid relationship json");
				}

				_endNode = Node.Initilize(null, endNode.ToString());

				return _endNode;
			}
		}

		public string Type
		{
			get
			{
				if (_type != null)
				{
					return _type;
				}

				if (string.IsNullOrWhiteSpace(OriginalRelationshipJson))
				{
					return null;
				}

				var joRelationship = JObject.Parse(OriginalRelationshipJson);

				JToken type;
				if (!joRelationship.TryGetValue("type", out type))
				{
					throw new Exception("Invalid relationship json");
				}

				if (type.Type != JTokenType.String)
				{
					throw new Exception("Invalid relationship json");
				}

				_type = type.ToString();

				return _type;
			}
		}

		#region Creators

		public RestRelationshipStore()
		{
			Id = int.MinValue;
			DbUrl = null;
			Self = null;
			OriginalRelationshipJson = null;
		}

		private RestRelationshipStore(ConnectionElement connection, long id, string relationshipJson = null)
		{
			DbUrl = connection.DbUrl;
			Id = id;
			Self = string.Concat(Connection.GetServiceRoot(DbUrl).Relationship, "/", Id);
			OriginalRelationshipJson = relationshipJson;
		}

		private RestRelationshipStore(string self, string relationshipJson = null)
		{
			ParseSelf(self);
			OriginalRelationshipJson = relationshipJson;
		}

		#endregion

		#region CreateRelationship

		public Relationship CreateRelationship(ConnectionElement connection, Node startNode, Node endNode, string name, Properties properties)
		{
			string response;
			var status = Neo4jRestApi.CreateRelationship(connection.DbUrl, startNode.Id, endNode.Id, name, properties.ToString(), out response);

			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating relationship (http response:{0})", status));
			}

			return ParseRelationshipJson(response).First();
		}

		public Relationship CreateUniqueRelationship(ConnectionElement connection, Node startNode, Node endNode, string name, Properties properties, string indexName, string key, object value, IndexUniqueness uniqueness)
		{
			string response;
			var status = Neo4jRestApi.CreateUniqueRelationship(connection.DbUrl, startNode.Id, endNode.Id, name, properties.ToString(), indexName, key, value, uniqueness, out response);

			if (status == HttpStatusCode.Created)
			{
				return ParseRelationshipJson(response).First();
			}

			// Create unique relationship but index mapping already exists
			if (status == HttpStatusCode.OK)
			{
				return null;
			}

			throw new Exception(string.Format("Error creating relationship (http response:{0})", status));
		}

		#endregion

		#region GetRelationship

		public Relationship GetRelationship(ConnectionElement connection, long relationshipId)
		{
			string response;
			var status = Neo4jRestApi.GetRelationship(connection.DbUrl, relationshipId, out response);
			if (status == HttpStatusCode.NotFound)
			{
				throw new RelationshipNotFoundException(string.Format("Relationship({0})", relationshipId));
			}

			return CreateRelationshipFromJson(response);
		}


		public IEnumerable<Relationship> GetRelationship(ConnectionElement connection, string indexName, string key, object value)
		{
			string response;
			var status = Neo4jRestApi.GetRelationship(connection.DbUrl, indexName, key, value, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseRelationshipJson(response);
		}

		public IEnumerable<Relationship> GetRelationship(ConnectionElement connection, string indexName, string searchQuery)
		{
			string response;
			var status = Neo4jRestApi.GetRelationship(connection.DbUrl, indexName, searchQuery, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", indexName));
			}

			return ParseRelationshipJson(response);
		}

		#endregion

		#region Initilize

		public Relationship Initilize(ConnectionElement connection, long id, Properties properties)
		{
			return new Relationship(new RestRelationshipStore(connection, id), properties);
		}

		public Relationship Initilize(string selfUri, Properties properties)
		{
			return new Relationship(new RestRelationshipStore(selfUri), properties);
		}

		#endregion

		#region DeleteRelationship

		public HttpStatusCode DeleteRelationship(ConnectionElement connection)
		{
			var status = Neo4jRestApi.DeleteRelationship(connection.DbUrl, Id);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error deleting relationship (relationship id:{0} http response:{1})", Id, status));
			}

			return status;
		}

		#endregion

		#region Index

		public Relationship AddToIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value)
		{
			string response;
			var status = Neo4jRestApi.AddRelationshipToIndex(connection.DbUrl, relationship.Id, indexName, key, value, out response);

			if (status == HttpStatusCode.Created || status == HttpStatusCode.OK)
			{
				return ParseRelationshipJson(response).First();
			}

			//// Add a relationship to an index but mapping already exists
			//if (unique && status == HttpStatusCode.OK)
			//{
			//	return null;
			//}

			throw new Exception(string.Format("Error adding relationship to index (http response:{0})", status));
			
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName)
		{
			var status = Neo4jRestApi.RemoveRelationshipFromIndex(connection.DbUrl, relationship.Id, indexName);

			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", relationship.Id, indexName, status));
			}

			return true;
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key)
		{
			var status = Neo4jRestApi.RemoveRelationshipFromIndex(connection.DbUrl, relationship.Id, indexName, key);

			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} key:{2} http response:{3})", relationship.Id, indexName, key, status));
			}

			return true;
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value)
		{
			var status = Neo4jRestApi.RemoveRelationshipFromIndex(connection.DbUrl, relationship.Id, indexName, key, value);

			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} key:{2} http response:{3})", relationship.Id, indexName, key, status));
			}

			return true;
		}
		#endregion

		#region Properties

		public Properties GetProperties()
		{
			string response;
			var status = Neo4jRestApi.GetPropertiesOnRelationship(DbUrl, Id, out response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving properties on relationship (relationship id:{0} http response:{1})", Id, status));
			}

			return Properties.ParseJson(response);
		}

		public void SaveProperties(Properties properties)
		{
			var status = Neo4jRestApi.SetPropertiesOnRelationship(DbUrl, Id, properties.ToString());
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error setting properties on relationship (relationship id:{0} http response:{1})", Id, status));
			}

		}

		#endregion

		#region ParseRelationshipJson

		public static IEnumerable<Relationship> ParseRelationshipJson(string jsonRelationships)
		{
			if (String.IsNullOrEmpty(jsonRelationships))
				return null;

			var relationships = new List<Relationship>();

			// The Json passed in can be a JObject or JArray - this is to test for that.
			var jo = JObject.Parse(string.Concat("{\"root\":", jsonRelationships, "}"));

			switch (jo["root"].Type)
			{
				case JTokenType.Object:
					relationships.Add(CreateRelationshipFromJson(jo["root"].ToString(Formatting.None)));
					break;

				case JTokenType.Array:
					relationships.AddRange(from JObject jsonRelationship in jo["root"] select CreateRelationshipFromJson(jsonRelationship));
					break;

				default:
					throw new Exception("Invalid relationship json");
			}

			return relationships;
		}

		#endregion

		#region ParseSelf

		private void ParseSelf(string self)
		{
			if (!IsSelfARelationship(self))
			{
				throw new Exception(string.Format("Self is not a Relationship ({0})", self));
			}

			// Make sure there is no trailing /
			self = self.TrimEnd('/');

			var selfArray = self.Split('/');

			long relationshipId;
			if (!long.TryParse(selfArray.Last(), out relationshipId))
			{
				throw new Exception(string.Format("Invalid Self id ({0})", self));
			}

			var url = new Uri(self);

			DbUrl = string.Format("{0}://{1}", url.Scheme, url.Authority);

			// Set Id & RelationshipId values
			Id = relationshipId;

			Self = self;
		}

		private static bool IsSelfARelationship(string self)
		{
			var selfArray = self.Split('/');
			return (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "relationship");
		}

		#endregion

		#region CreateRelationshipFromJson

		private static Relationship CreateRelationshipFromJson(string relationshipJson)
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

			return CreateRelationshipFromJson(jo);
		}

		public static Relationship CreateRelationshipFromJson(JObject relationshipJson)
		{
			JToken self;
			if (!relationshipJson.TryGetValue("self", out self))
			{
				throw new Exception("Invalid relationship json");
			}

			JToken properties;
			if (!relationshipJson.TryGetValue("data", out properties))
			{
				throw new Exception("Invalid relationship json");
			}

			return new Relationship(new RestRelationshipStore(self.Value<string>(), relationshipJson.ToString(Formatting.None, new IsoDateTimeConverter())), Properties.ParseJson(properties.ToString(Formatting.None, new IsoDateTimeConverter())));
		}

		public static Relationship CreateRelationshipFromSelf(string self)
		{
			return new Relationship(new RestRelationshipStore(self));
		}

		#endregion

	}
}
