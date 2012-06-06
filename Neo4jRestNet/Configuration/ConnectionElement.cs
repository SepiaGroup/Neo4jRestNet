using System.Configuration;

namespace Neo4jRestNet.Configuration
{
	public class ConnectionElement : ConfigurationElement
	{
		public string DbUrl
		{
			get
			{
				return string.Format("{0}://{1}:{2}{3}", Https ? "https" : "http", Domain, Port, DataPath);
			}
		}

		public string CypherUrl
		{
			get
			{
				return string.Concat(DbUrl, "/cypher");
			}
		}

		public string GremlinUrl
		{
			get
			{
				return string.Concat(DbUrl, GremlinPath);
			}
		}

		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("https", IsRequired = false, DefaultValue = false)]
		public bool Https
		{
			get { return (bool)this["https"]; }
			set { this["https"] = value; }
		}

		[ConfigurationProperty("default", IsRequired = false, DefaultValue = false)]
		public bool Default
		{
			get { return (bool)this["default"]; }
			set { this["default"] = value; }
		}

		[ConfigurationProperty("domain", IsRequired = true)]
		public string Domain
		{
			get { return (string)this["domain"]; }
			set { this["domain"] = value; }
		}

		[ConfigurationProperty("port", IsRequired = false, DefaultValue = "7474")]
		public string Port
		{
			get { return (string)this["port"]; }
			set { this["port"] = value; }
		}

		[ConfigurationProperty("dataPath", IsRequired = false, DefaultValue = "/db/data")]
		public string DataPath
		{
			get { return (string)this["dataPath"]; }
			set { this["dataPath"] = string.Concat('/', value.TrimStart('/').TrimEnd('/')); }
		}

		[ConfigurationProperty("gremlinPath", IsRequired = false, DefaultValue = "/ext/GremlinPlugin/graphdb/execute_script")]
		public string GremlinPath
		{
			get { return (string)this["gremlinPath"]; }
			set { this["gremlinPath"] = string.Concat('/', value.TrimStart('/').TrimEnd('/')); }
		}

	}
}