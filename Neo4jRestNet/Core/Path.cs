using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Neo4jRestNet.Core
{
	public class Path : IGraphObject
	{
		public string Self { get; set; }
		public Node StartNode { get; private set; }
		public Node EndNode { get; private set; }
		public List<Node> Nodes { get; private set; }
		public List<Relationship> Relationships { get; private set; }
		public string OriginalPathJson { get; private set; }

		private Path(JObject path)
		{
			JToken startNode;
			if (!path.TryGetValue("start", out startNode))
			{
				throw new Exception("Invalid path json");
			}

			switch (startNode.Type)
			{
				case JTokenType.String:
					StartNode = Node.InitializeFromSelf(startNode.Value<string>());
					break;

				case JTokenType.Object:
					StartNode = Node.InitializeFromNodeJson((JObject)startNode);
					break;

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
					EndNode = Node.InitializeFromSelf(endNode.Value<string>());
					break;

				case JTokenType.Object:
					EndNode = Node.InitializeFromNodeJson((JObject)endNode);
					break;

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
						Nodes.Add(Node.InitializeFromSelf(node.Value<string>()));
						break;

					case JTokenType.Object:
						Nodes.Add(Node.InitializeFromNodeJson((JObject)node));
						break;

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
						Relationships.Add(Relationship.InitializeFromSelf(relationship.Value<string>()));
						break;

					case JTokenType.Object:
						Relationships.Add(Relationship.InitializeFromRelationshipJson((JObject)relationship));
						break;

					default:
						throw new Exception("Invalid path json");
				}
			}

			OriginalPathJson = path.ToString(Formatting.None);
		}

		public static List<Path> ParseJson(string jsonPaths)
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
			if (jsonPaths == null)
			{
				return null;
			}

			var jaPaths = jsonPaths;

			return (from JObject joPath in jaPaths select new Path(joPath)).ToList();
		}
	}
}
