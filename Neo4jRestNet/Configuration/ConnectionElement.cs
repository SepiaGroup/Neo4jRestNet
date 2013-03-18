using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net;
using Neo4jRestNet.Core.Exceptions;
using Neo4jRestNet.Rest;
using Newtonsoft.Json;

namespace Neo4jRestNet.Configuration
{
	public class ConnectionElement : ConfigurationElement
	{
		public string DbUrl
		{
			get
			{
				return string.Format("{0}://{1}:{2}", Https ? "https" : "http", Domain, Port);
			}
		}

		public string CypherUrl
		{
			get
			{
				return Connection.GetServiceRoot(DbUrl).Cypher;
			}
		}

		public string GremlinUrl
		{
			get
			{
				return Connection.GetServiceRoot(DbUrl).Gremlin; 
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
	}
}