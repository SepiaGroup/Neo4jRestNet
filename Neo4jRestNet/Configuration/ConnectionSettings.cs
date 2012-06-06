using System.Configuration;

namespace Neo4jRestNet.Configuration
{
	public class ConnectionSettings : ConfigurationSection
	{
		[ConfigurationProperty("databases", IsDefaultCollection = true)]
		public ConnectionCollection Connections
		{
			get { return (ConnectionCollection)this["databases"]; }
			set { this["databases"] = value; }
		}
	}
}