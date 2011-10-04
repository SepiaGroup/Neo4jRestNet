using System.Collections.Generic;
using System.Net;
using System.Linq;
using Neo4jRestNet.Rest;
using System;
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

		private Path(JObject Path)
		{
			JToken startNode;
			if (!Path.TryGetValue("start", out startNode))
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
			if (!Path.TryGetValue("end", out endNode))
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
			if (!Path.TryGetValue("nodes", out nodes) || nodes.Type != JTokenType.Array)
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
			if (!Path.TryGetValue("relationships", out relationships) || relationships.Type != JTokenType.Array)
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

			OriginalPathJson = Path.ToString(Formatting.None);
		}

		public static List<Path> ParseJson(string JsonPaths)
		{
			if (String.IsNullOrEmpty(JsonPaths))
				return null;
			else
			{
				List<Path> Paths = new List<Path>();

				JArray jsonPaths = JArray.Parse(JsonPaths);

				foreach (JObject jsonPath in jsonPaths)
				{
					Paths.Add(new Path(jsonPath));
				}

				return Paths;
			}
		}
	}
}
