using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Neo4jRestNet.Configuration
{
	public class DatabaseEndpoint
	{
		[JsonProperty("management")]
		public string Management { get; set; }

		[JsonProperty("data")]
		public string Data { get; set; }
	}
}
