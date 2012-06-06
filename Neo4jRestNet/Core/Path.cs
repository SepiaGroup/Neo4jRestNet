using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Neo4jRestNet.Core
{
	public class Path 
	{

		public long Id { get; private set; }

		public string Self { get; set; }
		public Node StartNode { get; private set; }
		public Node EndNode { get; private set; }
		public List<Node> Nodes { get; private set; }
		public List<Relationship> Relationships { get; private set; }
		public string OriginalPathJson { get; private set; }

		private Path(JObject path, INodeStore nodeGraphStore, IRelationshipStore relationshipGraphStore)
		{
			JToken startNode;
			if (!path.TryGetValue("start", out startNode))
			{
				throw new Exception("Invalid path json");
			}

			switch (startNode.Type)
			{
				case JTokenType.String:
					StartNode = nodeGraphStore.Initilize(startNode.Value<string>(), null);
					break;

				//case JTokenType.Object:
				//    StartNode = nodeGraphStore.CreateNodeFromJson((JObject)startNode);
				//    break;

				default:
					throw new Exception("Invalid path json");
			}

			JToken endNode;
			if (!path.TryGetValue("end", out endNode))
			{
				throw new Exception("Invalid path json");
			}

			switch (endNode.Type)
			{
				case JTokenType.String:
					EndNode = nodeGraphStore.Initilize(endNode.Value<string>(), null);
					break;

				//case JTokenType.Object:
				//    EndNode = nodeGraphStore.CreateNodeFromJson((JObject)endNode);
				//    break;

				default:
					throw new Exception("Invalid path json");
			}
			
			Nodes = new List<Node>();
			JToken nodes;
			if (!path.TryGetValue("nodes", out nodes) || nodes.Type != JTokenType.Array)
			{
				throw new Exception("Invalid path json");
			}

			foreach (JToken node in nodes)
			{
				switch (node.Type)
				{
					case JTokenType.String:
						Nodes.Add(nodeGraphStore.Initilize(node.Value<string>(), null));
						break;

					//case JTokenType.Object:
					//    Nodes.Add(nodeGraphStore.CreateNodeFromJson((JObject)node));
					//    break;

					default:
						throw new Exception("Invalid path json");
				}
			}

			Relationships = new List<Relationship>();
			JToken relationships;
			if (!path.TryGetValue("relationships", out relationships) || relationships.Type != JTokenType.Array)
			{
				throw new Exception("Invalid path json");
			}

			foreach (JToken relationship in relationships)
			{
				switch (relationship.Type)
				{
					case JTokenType.String:
						Relationships.Add(relationshipGraphStore.Initilize(relationship.Value<string>(), null));
						break;

					//case JTokenType.Object:
					//    Relationships.Add(relationshipGraphStore.CreateRelationshipFromJson((JObject)relationship));
					//    break;

					default:
						throw new Exception("Invalid path json");
				}
			}

			OriginalPathJson = path.ToString(Formatting.None);
		}

		#region ParseJson

		public static List<Path> ParseJson(string jsonPaths)
		{
			return ParseJson(jsonPaths, new RestNodeStore(), new RestRelationshipStore());
		}

		public static List<Path> ParseJson(string jsonPaths, INodeStore nodeGraphStore)
		{
			return ParseJson(jsonPaths, nodeGraphStore, new RestRelationshipStore());
		}

		public static List<Path> ParseJson(string jsonPaths, IRelationshipStore relationshipGraphStore)
		{
			return ParseJson(jsonPaths, new RestNodeStore(), relationshipGraphStore);
		}

		public static List<Path> ParseJson(string jsonPaths, INodeStore nodeGraphStore, IRelationshipStore relationshipGraphStore)
		{
			if (String.IsNullOrEmpty(jsonPaths))
			{
				return null;
			}

			var jaPaths = JArray.Parse(jsonPaths);
			return ParseJson(jaPaths);
		}

		public static List<Path> ParseJson(JArray jsonPaths)
		{
			return ParseJson(jsonPaths, new RestNodeStore(), new RestRelationshipStore());
		}

		public static List<Path> ParseJson(JArray jsonPaths, INodeStore nodeGraphStore)
		{
			return ParseJson(jsonPaths, nodeGraphStore, new RestRelationshipStore());
		}

		public static List<Path> ParseJson(JArray jsonPaths, IRelationshipStore relationshipGraphStore)
		{
			return ParseJson(jsonPaths, new RestNodeStore(), relationshipGraphStore);
		}

		public static List<Path> ParseJson(JArray jsonPaths, INodeStore nodeGraphStore, IRelationshipStore relationshipGraphStore)
		{
			if (jsonPaths == null)
			{
				return null;
			}

			var jaPaths = jsonPaths;

			return (from JObject joPath in jaPaths select new Path(joPath, nodeGraphStore, relationshipGraphStore)).ToList();
		}

		#endregion
	}
}
