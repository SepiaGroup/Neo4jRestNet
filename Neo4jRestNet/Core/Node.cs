using System.Collections.Generic;
using System.Linq;
using System.Net;
using Neo4jRestNet.Core;
using System.Reflection;
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

		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');

		private string _dbUrl;
		public long Id { get; private set; }
		public EncryptId EncryptedId { get; private set; }
		private string _Self;
		private Properties _Properties;
		public string OriginalNodeJson { get; private set; }
		
		protected Node() { }

		#region GetRootNode

		public static Node GetRootNode()
		{
			return GetRootNode(DefaultDbUrl);
		}

		public static Node GetRootNode(string dbUrl)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.GetRoot(dbUrl, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error getting root node (http response:{0})", status));
			}

			JObject jo;
			try
			{
				jo = JObject.Parse(Response);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid json", e);
			}

			JToken ReferenceNode;
			if (!jo.TryGetValue("reference_node", out ReferenceNode))
			{
				throw new Exception("Invalid json");
			}

			Node node = new Node();

			node.Self = ReferenceNode.Value<string>();
			node._Properties = null;

			return node;
		}

		#endregion

		#region GetNode

		public static Node GetNode(EncryptId NodeId)
		{
			return GetNode(DefaultDbUrl, NodeId);
		}

		public static Node GetNode(string dbUrl, EncryptId NodeId)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.GetNode(dbUrl, (long)NodeId, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Node not found (node id:{0})", (long)NodeId));
			}

			return Node.InitializeFromNodeJson(Response);
		}

		public static IEnumerable<Node> GetNode(string IndexName, string Key, object Value)
		{
			return GetNode(DefaultDbUrl, IndexName, Key, Value);
		}

		public static IEnumerable<Node> GetNode(string dbUrl, string IndexName, string Key, object Value)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.GetNode(dbUrl, IndexName, Key, Value, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", IndexName));
			}

			return Node.ParseJson(Response);
		}

		public static IEnumerable<Node> GetNode(string IndexName, string SearchQuery)
		{
			return GetNode(DefaultDbUrl, IndexName, SearchQuery);
		}

		public static IEnumerable<Node> GetNode(string dbUrl, string IndexName, string SearchQuery)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.GetNode(dbUrl, IndexName, SearchQuery, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Index not found in (index:{0})", IndexName));
			}

			return Node.ParseJson(Response);
		}

		#endregion

		#region CreateNode

		public static Node CreateNode(string NodeType)
		{
			Properties properties = new Properties();
			properties.SetProperty(NodeProperty.NodeType.ToString(), NodeType.ToString());

			return CreateNodeFromJson(DefaultDbUrl, properties.ToString());
		}

		public static Node CreateNode(string dbUrl, string NodeType)
		{
			Properties properties = new Properties();
			properties.SetProperty(NodeProperty.NodeType.ToString(), NodeType.ToString());

			return CreateNodeFromJson(dbUrl, properties.ToString());
		}

		public static Node CreateNode(string NodeType, Properties properties)
		{
			properties.SetProperty(NodeProperty.NodeType.ToString(), NodeType.ToString());

			return CreateNodeFromJson(DefaultDbUrl, properties.ToString());
		}

		public static Node CreateNode(string dbUrl, string NodeType, Properties properties)
		{
			properties.SetProperty(NodeProperty.NodeType.ToString(), NodeType.ToString());

			return CreateNodeFromJson(dbUrl, properties.ToString());
		}

		public static Node CreateNode(string NodeType, IDictionary<string, object> properties)
		{
			properties.Add(NodeProperty.NodeType.ToString(), NodeType.ToString());

			return CreateNodeFromJson(DefaultDbUrl, JObject.FromObject(properties).ToString(Formatting.None));
		}

		public static Node CreateNode(string dbUrl, string NodeType, IDictionary<string, object> properties)
		{
			properties.Add(NodeProperty.NodeType.ToString(), NodeType.ToString());

			return CreateNodeFromJson(dbUrl, JObject.FromObject(properties).ToString(Formatting.None));
		}

		private static Node CreateNodeFromJson(string dbUrl, string jsonProperties)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.CreateNode(dbUrl, jsonProperties, out Response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating node (http response:{0})", status));
			}

			return Node.InitializeFromNodeJson(Response);
		}

		#endregion

		#region Initializers

		public static Node InitializeFromNodeJson(string NodeJson)
		{
			JObject jo;

			try
			{
				jo = JObject.Parse(NodeJson);
			}
			catch (Exception e)
			{
				throw new Exception("Invalid node json", e);
			}

			return InitializeFromNodeJson(jo);
		}

		public static Node InitializeFromNodeJson(JObject NodeJson)
		{
			JToken self;
			if (!NodeJson.TryGetValue("self", out self))
			{
				throw new Exception("Invalid node json");
			}

			JToken properties;
			if (!NodeJson.TryGetValue("data", out properties))
			{
				throw new Exception("Invalid node json");
			}

			Node node = new Node();

			node.Self = self.Value<string>();
			node._Properties = Properties.ParseJson(properties.ToString(Formatting.None));
			node.OriginalNodeJson = NodeJson.ToString(Formatting.None);

			return node;
		}

		public static Node InitializeFromSelf(string Self)
		{
			Node node = new Node();

			node.Self = Self;
			node._Properties = null;

			return node;
		}

		public static bool IsSelfANode(string Self)
		{
			string[] selfArray = Self.Split('/');
			return (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node") ? true : false;
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
				if (!Node.IsSelfANode(value))
				{
					throw new Exception(string.Format("Self is not a Node ({0})", Self));
				}

				// Make sure there is no trailing /
				string self = value.TrimEnd('/');

				string[] SelfArray = self.Split('/');

				long NodeId;
				if (!long.TryParse(SelfArray.Last(), out NodeId))
				{
					throw new Exception(string.Format("Invalid Self id ({0})", value));
				}

				_dbUrl = self.Substring(0, self.LastIndexOf("/node"));
				_Self = self;

				// Set Id & NodeId values
				this.Id = NodeId;
				this.EncryptedId = NodeId;
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
			HttpStatusCode status = Neo4jRestApi.GetPropertiesOnNode(_dbUrl, (long)EncryptedId, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving properties on node (node id:{0} http response:{1})", (long)EncryptedId, status));
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
			LoadProperties(false);
			if (Properties.HasProperty(NodeProperty.NodeType.ToString()))
			{
				properties.SetProperty(NodeProperty.NodeType.ToString(), Properties.GetProperty<string>(NodeProperty.NodeType.ToString()));
			}

			HttpStatusCode status = Neo4jRestApi.SetPropertiesOnNode(_dbUrl, (long)EncryptedId, properties.ToString());
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
			IEnumerable<string> Names = null;
			return GetRelationships(RelationshipDirection.All, Names);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection Direction)
		{
			IEnumerable<string> Names = null;
			return GetRelationships(Direction, Names);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection Direction, Relationship RelationshipType)
		{
			return GetRelationships(Direction, new List<string>() { RelationshipType.ToString() });
		}

		public IEnumerable<Relationship> GetRelationships(string Name)
		{
			return GetRelationships(RelationshipDirection.All, new List<string>() { Name });
		}

		public IEnumerable<Relationship> GetRelationships(IEnumerable<string> Names)
		{
			return GetRelationships(RelationshipDirection.All, Names);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection Direction, string Name)
		{
			return GetRelationships(Direction, new List<string>() { Name });
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection Direction, IEnumerable<string> Names)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.GetRelationshipsOnNode(_dbUrl, (long)EncryptedId, Direction, Names, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error retrieving relationships on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			return Relationship.ParseJson(Response);
		}

		public Relationship CreateRelationshipTo(Node ToNode, string RelationshipType)
		{
			return CreateRelationshipTo(ToNode, RelationshipType, null);
		}

		public Relationship CreateRelationshipTo(Node ToNode, string RelationshipType, Properties RelationshipProperties)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.CreateRelationship(_dbUrl, 
																	(long)EncryptedId, 
																	ToNode.Self, 
																	RelationshipType, 
																	RelationshipProperties == null ? null : RelationshipProperties.ToString(), 
																	out Response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creationg relationship on node (node id:{0} http response:{1})", (long)EncryptedId, status));
			}

			return Relationship.ParseJson(Response).First();
		}

		#endregion

		#region Traverse

		public IEnumerable<IGraphObject> Traverse(Order order, Uniqueness uniqueness, IEnumerable<TraverseRelationship> relationships, PruneEvaluator pruneEvaluator, ReturnFilter returnFilter, int? MaxDepth, ReturnType returnType)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.Traverse(_dbUrl, (long)EncryptedId, order, uniqueness, relationships, pruneEvaluator, returnFilter, MaxDepth, returnType, out Response);
			if (status != HttpStatusCode.OK)
			{
				throw new Exception(string.Format("Error traversing nodes (node id:{0} status code:{1})", (long)EncryptedId, status));
			}

			if (returnType.ToString() == ReturnType.Node.ToString())
			{
				return Node.ParseJson(Response);
			}
			else if (returnType.ToString() == ReturnType.Relationship.ToString())
			{
				return Relationship.ParseJson(Response);
			}
			else if (returnType.ToString() == ReturnType.Path.ToString() || returnType.ToString() == ReturnType.FullPath.ToString())
			{
				return Path.ParseJson(Response);
			}

			throw new Exception(string.Format("Return type not implemented (type:{0})", returnType.ToString()));
		}

		#endregion

		#region Index

		public static Node AddToIndex(long NodeId, string IndexName, string Key, object Value)
		{
			return AddToIndex(DefaultDbUrl, NodeId, IndexName, Key, Value);
		}

		public static Node AddToIndex(string dbUrl, long NodeId, string IndexName, string Key, object Value)
		{
			string Response;
			HttpStatusCode status = Neo4jRestApi.AddNodeToIndex(dbUrl, NodeId, IndexName, Key, Value, out Response);
			if (status != HttpStatusCode.Created)
			{
				throw new Exception(string.Format("Error creating index for node (http response:{0})", status));
			}

			return Node.InitializeFromNodeJson(Response);
		}

		public HttpStatusCode RemoveFromIndex(long NodeId, string IndexName)
		{
			return RemoveFromIndex(DefaultDbUrl, NodeId, IndexName);
		}

		public HttpStatusCode RemoveFromIndex(string dbUrl, long NodeId, string IndexName)
		{
			HttpStatusCode status = Neo4jRestApi.RemoveNodeFromIndex(dbUrl, NodeId, IndexName);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", NodeId, IndexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveFromIndex(long NodeId, string IndexName, string Key)
		{
			return RemoveFromIndex(DefaultDbUrl, NodeId, IndexName, Key);
		}

		public HttpStatusCode RemoveFromIndex(string dbUrl, long NodeId, string IndexName, string Key)
		{
			HttpStatusCode status = Neo4jRestApi.RemoveNodeFromIndex(dbUrl, NodeId, IndexName, Key);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", NodeId, IndexName, status));
			}

			return status;
		}

		public HttpStatusCode RemoveFromIndex(long NodeId, string IndexName, string Key, object Value)
		{
			return RemoveFromIndex(DefaultDbUrl, NodeId, IndexName, Key, Value);
		}

		public HttpStatusCode RemoveFromIndex(string dbUrl, long NodeId, string IndexName, string Key, object Value)
		{
			HttpStatusCode status = Neo4jRestApi.RemoveNodeFromIndex(dbUrl, NodeId, IndexName, Key, Value);
			if (status != HttpStatusCode.NoContent)
			{
				throw new Exception(string.Format("Error remove node from index (node id:{0} index name:{1} http response:{2})", NodeId, IndexName, status));
			}

			return status;
		}
		#endregion

		#region NodeType

		public string NodeType
		{
			get
			{
				if(_Properties == null)
				{
					return null;
				}

				return _Properties.GetProperty<string>(NodeProperty.NodeType.ToString());
			}
		}

		#endregion

		#region ParseJson

		public static IEnumerable<Node> ParseJson(string JsonNodes)
		{
			if (String.IsNullOrEmpty(JsonNodes))
				return null;
			else
			{
				List<Node> Nodes = new List<Node>();

				// The Json passed in can be a JObject or JArray - this is to test for that.
				JObject jo = JObject.Parse(string.Concat("{\"root\":", JsonNodes, "}"));

				switch (jo["root"].Type)
				{
					case JTokenType.Object:
						Nodes.Add(Node.InitializeFromNodeJson(jo["root"].ToString(Formatting.None)));
						break;

					case JTokenType.Array:
						foreach (JObject JsonNode in jo["root"])
						{
							Nodes.Add(Node.InitializeFromNodeJson(JsonNode));
						}
						break;

					default:
						throw new Exception("Invalid node json");
				}

				return Nodes;
			}
		}

		#endregion

		#region IEquatable<Node> Members

		public bool Equals(Node other)
		{
			if (ReferenceEquals(other, null))
				return false;
			else if (ReferenceEquals(this, other))
				return true;
			else if (this.EncryptedId == null || other.EncryptedId == null)
				return false;
			else if (this.EncryptedId.Equals(other.EncryptedId))
				return true;

			return false;
		}

		public override bool Equals(Object obj)
		{
			return Equals(obj as Node);
		}

		public override int GetHashCode()
		{
			return (int)this.EncryptedId;
		}

		public static bool operator ==(Node Value1, Node Value2)
		{
			if (object.ReferenceEquals(Value1, Value2))
			{
				return true;
			}
			if (object.ReferenceEquals(Value1, null)) // Value2=null is covered by Equals
			{
				return false;
			}
			return Value1.Equals(Value2);
		}

		public static bool operator !=(Node Value1, Node Value2)
		{
			if (object.ReferenceEquals(Value1, Value2))
			{
				return false;
			}
			if (object.ReferenceEquals(Value1, null)) // Value2=null is covered by Equals
			{
				return false;
			}
			return !Value1.Equals(Value2);
		}

		#endregion
	}
}