using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Configuration
{
	public class ServiceRoot
	{
		[JsonProperty("node")]
		public string Node { get; set; }

		private string _relationship;
		public string Relationship
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_relationship))
				{
					_relationship = Node.Replace("node", "relationship");
				}

				return _relationship;
			}
			
			set
			{
				_relationship = value;
			}
		}

		[JsonProperty("reference_node")]
		public string ReferenceNode { get; set; }

		[JsonProperty("node_index")]
		public string NodeIndex { get; set; }

		[JsonProperty("relationship_index")]
		public string RelationshipIndex { get; set; }

		[JsonProperty("extensions_info")]
		public string ExtensionsInfo { get; set; }

		[JsonProperty("relationship_types")]
		public string RelationshipTypes { get; set; }

		[JsonProperty("batch")]
		public string Batch { get; set; }

		[JsonProperty("cypher")]
		public string Cypher { get; set; }

		[JsonProperty("neo4j_version")]
		public string Neo4jVersion { get; set; }

		private JObject _extensions;

		[JsonProperty("extensions")]
		public JObject Extensions
		{
			get { return _extensions; }
			
			set
			{
				_extensions = value;
				var plugin = value["GremlinPlugin"];
				if (plugin == null)
				{
					Gremlin = string.Empty;
					return;
				}

				var gremlin = plugin["execute_script"];
				if (gremlin == null)
				{
					Gremlin = string.Empty;
					return;
				}

				Gremlin = gremlin.ToString();
			}
		}

		public string Gremlin { get; set; }
	}
}
