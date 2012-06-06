using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Neo4jRestNet.Configuration
{
	public static class ConnectionManager
	{
		private static readonly ConnectionSettings ConnectionConfiguration = ConfigurationManager.GetSection("neo4jRestNet") as ConnectionSettings;
        private static readonly IEnumerable<ConnectionElement> Connections = ConnectionConfiguration.Connections.Cast<ConnectionElement>().ToList();

		public static ConnectionElement Connection()
		{
			return Connections.First(f => f.Default);
		}

		public static ConnectionElement Connection(string name)
		{
			return Connections.First(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)); 
		}
	}
}
