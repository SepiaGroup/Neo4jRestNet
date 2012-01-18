using System;
using System.Collections.Generic;
using System.Linq;
using Neo4jRestNet.Core.Interface;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Neo4jRestNet.Core.Implementation
{
	public class Path : IPath
	{
		public string Self { get; set; }
		public INode StartNode { get; private set; }
		public INode EndNode { get; private set; }
		public List<INode> Nodes { get; private set; }
		public List<IRelationship> Relationships { get; private set; }
		public string OriginalPathJson { get; private set; }
		
		public Path ()
		{
			
		}

		public Path(JObject path)
		{
			JToken startNode;
			if (!path.TryGetValue("start", out startNode))
			{
				throw new Exception("Invalid path json");
			}

			switch (startNode.Type)
			{
				case JTokenType.String:
					StartNode = new Node().InitializeFromSelf(startNode.Value<string>());
					break;

				case JTokenType.Object:
					StartNode = new Node().InitializeFromNodeJson((JObject)startNode);
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
					EndNode = new Node().InitializeFromSelf(endNode.Value<string>());
					break;

				case JTokenType.Object:
					EndNode = new Node().InitializeFromNodeJson((JObject)endNode);
					break;

				default:
					throw new Exception("Invalid path json");
			}

			Nodes = new List<INode>();
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
						Nodes.Add(new Node().InitializeFromSelf(node.Value<string>()));
						break;

					case JTokenType.Object:
						Nodes.Add(new Node().InitializeFromNodeJson((JObject)node));
						break;

					default:
						throw new Exception("Invalid path json");
				}
			}

			Relationships = new List<IRelationship>();
			JToken relationships;
			if (!path.TryGetValue("relationships", out relationships) || relationships.Type != JTokenType.Array)
			{
				throw new Exception("Invalid path json");
			}

			foreach (var relationship in relationships)
			{
				switch (relationship.Type)
				{
					case JTokenType.String:
						Relationships.Add(new Relationship().InitializeFromSelf(relationship.Value<string>()));
						break;

					case JTokenType.Object:
						Relationships.Add(new Relationship().InitializeFromRelationshipJson((JObject)relationship));
						break;

					default:
						throw new Exception("Invalid path json");
				}
			}

			OriginalPathJson = path.ToString(Formatting.None);
		}

		public List<IPath> ParseJson(string jsonPaths)
		{
			if (String.IsNullOrEmpty(jsonPaths))
			{
				return null;
			}
			
			var jaPaths = JArray.Parse(jsonPaths);
			return ParseJson(jaPaths);
		}

		public List<IPath> ParseJson(JArray jsonPaths)
		{
			if (jsonPaths == null)
			{
				return null;
			}

			var jaPaths = jsonPaths;

			return (from JObject joPath in jaPaths select (IPath) new Path(joPath)).ToList();
		}
	}
}
