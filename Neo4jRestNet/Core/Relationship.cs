using System.Collections.Generic;
using System.Net;
using System.Linq;
using Neo4jRestNet.Rest;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace Neo4jRestNet.Core
{
	public class Relationship : IGraphObject
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');

		private string _dbUrl;
		private string _Self;
		public Node StartNode { get; private set; }
		public Node EndNode { get; private set; }
		public string Name { get; private set; }
		private Properties _Properties;
		public long Id { get; private set; }
		public EncryptId EncryptedId { get; private set; }
		public string OriginalRelationshipJson { get; private set; }

		private Relationship() { }

		#region GetRelationship

		public static IEnumerable<Relationship> GetRelationship(string IndexName, string Key, object Value)
		{
			return GetRelationship(DefaultDbUrl, IndexName, Key, Value);
		}

		public static IEnumerable<Relationship> GetRelationship(string dbUrl, string IndexName, string Key, object Value)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.GetRelationship(dbUrl, IndexName, Key, Value, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", IndexName));
			}

			return Relationship.ParseJson(Response);
		}

		public static IEnumerable<Relationship> GetRelationship(string IndexName, string SearchQuery)
		{
			return GetRelationship(DefaultDbUrl, IndexName, SearchQuery);
		}

		public static IEnumerable<Relationship> GetRelationship(string dbUrl, string IndexName, string SearchQuery)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.GetRelationship(dbUrl, IndexName, SearchQuery, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", IndexName));
			}

			return Relationship.ParseJson(Response);
		}

		#endregion

		#region Initializers 

		public static Relationship InitializeFromRelationshipJson(string RelationshipJson)
		{
			JObject jo;

			try
			{
				jo = JObject.Parse(RelationshipJson);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid relationship json", e);
			}

			return InitializeFromRelationshipJson(jo);
		}

		public static Relationship InitializeFromRelationshipJson(JObject RelationshipJson)
		{
			Relationship relationship = new Relationship();
			JToken self;
			if (!RelationshipJson.TryGetValue("self", out self) || self.Type != JTokenType.String)
			{
				throw new Exception("Invalid relationship json");
			}
			
			relationship.Self = self.Value<string>();

			JToken properties;
			if (!RelationshipJson.TryGetValue("data", out properties) || properties.Type != JTokenType.Object)
			{
				throw new Exception("Invalid relationship json");
			}

			JToken startNode;
			if (!RelationshipJson.TryGetValue("start", out startNode))
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
			if (!RelationshipJson.TryGetValue("end", out endNode))
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
			if (!RelationshipJson.TryGetValue("type", out name) || name.Type != JTokenType.String)
			{
				throw new Exception("Invalid relationship json");
			}

			relationship.Name = name.Value<string>();

			relationship._Properties = Properties.ParseJson(properties.ToString(Formatting.None));

			relationship.OriginalRelationshipJson = RelationshipJson.ToString(Formatting.None);

			return relationship;
		}

		public static Relationship InitializeFromSelf(string Self)
		{
			Relationship relationship = new Relationship();

			relationship.Self = Self;
			relationship.StartNode = null;
			relationship.EndNode = null;
			relationship.Name = null;
			relationship._Properties = null;
			relationship.OriginalRelationshipJson = null;

			return relationship;
		}

		public static bool IsSelfARelationship(string Self)
		{
			string[] selfArray = Self.Split('/');
			return (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "relationship") ? true : false;
		}
		#endregion

		#region Self

		public string Self
		{
			get
			{
				return _Self;
			}

			private set
			{
				if (!Relationship.IsSelfARelationship(value))
				{
					throw new Exception(string.Format("Self is not a Relationship ({0})", Self));
				}

				// Make sure there is no trailing /
				string self = value.TrimEnd('/');

				string[] SelfArray = self.Split('/');

				long RelationshipId;
				if (!long.TryParse(SelfArray.Last(), out RelationshipId))
				{
					throw new Exception(string.Format("Invalid Self id ({0})", value));
				}

				_dbUrl = self.Substring(0, self.LastIndexOf("/relationship"));
				_Self = self;
				this.Id = RelationshipId;
			}
		}

		#endregion

		#region Properties

		private void LoadProperties(bool Refresh)
		{
			if (Refresh)
			{
				this._Properties = null;
			}

			if (this._Properties != null)
			{
				return;
			}

			string Response;
			HttpStatusCode status = Neo4jRestApi.GetPropertiesOnRelationship(_dbUrl, Id, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving properties on relationship (relationship id:{0} http response:{1})", Id, status));
			}

			this._Properties = Properties.ParseJson(Response);
		}

		public Properties Properties
		{
			get
			{
				LoadProperties(false);
				return _Properties;
			}
		}

		public void SaveProperties()
		{
			SaveProperties(Properties);
		}

		public void SaveProperties(Properties properties)
		{
			HttpStatusCode status = Neo4jRestApi.SetPropertiesOnRelationship(_dbUrl, Id, properties.ToString());
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error setting properties on relationship (relationship id:{0} http response:{1})", Id, status));
			}

			LoadProperties(true);
		}

		#endregion

		#region Index

		public static Relationship AddToIndex(long RelationshipId, string IndexName, string Key, object Value)
		{
			return AddToIndex(DefaultDbUrl, RelationshipId, IndexName, Key, Value);
		}

		public static Relationship AddToIndex(string dbUrl, long RelationshipId, string IndexName, string Key, object Value)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.AddRelationshipToIndex(dbUrl, RelationshipId, IndexName, Key, Value, out Response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating index for relationship (http response:{0})", status));
			}

			return Relationship.InitializeFromRelationshipJson(Response);
		}

		public HttpStatusCode RemoveFromIndex(long RelationshipId, string IndexName)
		{
			return RemoveFromIndex(DefaultDbUrl, RelationshipId, IndexName);
		}

		public HttpStatusCode RemoveFromIndex(string dbUrl, long RelationshipId, string IndexName)
		{
			HttpStatusCode status = Neo4jRestApi.RemoveRelationshipFromIndex(dbUrl, RelationshipId, IndexName);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", RelationshipId, IndexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveFromIndex(long RelationshipId, string IndexName, string Key)
		{
			return RemoveFromIndex(DefaultDbUrl, RelationshipId, IndexName, Key);
		}

		public HttpStatusCode RemoveFromIndex(string dbUrl, long RelationshipId, string IndexName, string Key)
		{
			HttpStatusCode status = Neo4jRestApi.RemoveRelationshipFromIndex(dbUrl, RelationshipId, IndexName, Key);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", RelationshipId, IndexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveFromIndex(long RelationshipId, string IndexName, string Key, object Value)
		{
			return RemoveFromIndex(DefaultDbUrl, RelationshipId, IndexName, Key, Value);
		}

		public HttpStatusCode RemoveFromIndex(string dbUrl, long RelationshipId, string IndexName, string Key, object Value)
		{
			HttpStatusCode status = Neo4jRestApi.RemoveRelationshipFromIndex(dbUrl, RelationshipId, IndexName, Key, Value);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove relationship from index (relationship id:{0} index name:{1} http response:{2})", RelationshipId, IndexName, status));
			}

			return status;
		}
		#endregion

		#region Delete

		public HttpStatusCode Delete()
		{
			HttpStatusCode status = Neo4jRestApi.DeleteRelationship(this._dbUrl, this.Id);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error deleteing relationship (relationship id:{0} http response:{1})", Id, status));
			}

			return status;
		}

		#endregion

		#region ParseJson

		public static IEnumerable<Relationship> ParseJson(string JsonRelationships)
		{
			if (String.IsNullOrEmpty(JsonRelationships))
				return null;
			else
			{
				List<Relationship> Relationships = new List<Relationship>();

				// The Json passed in can be a JObject or JArray - this is to test for that.
				JObject jo = JObject.Parse(string.Concat("{\"root\":", JsonRelationships, "}"));

				switch (jo["root"].Type)
				{
					case JTokenType.Object:
						Relationships.Add(Relationship.InitializeFromRelationshipJson(jo["root"].ToString(Formatting.None)));
						break;

					case JTokenType.Array:
						foreach (JObject jsonRelationship in jo["root"])
						{
							Relationships.Add(Relationship.InitializeFromRelationshipJson(jsonRelationship));
						}
						break;

					default:
						throw new Exception("Invalid relationship json");
				}

				return Relationships;
			}
		}

		#endregion
	}
}
