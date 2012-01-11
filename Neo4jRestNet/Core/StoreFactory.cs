using System;
using System.Configuration;
using Neo4jRestNet.Rest;

namespace Neo4jRestNet.Core
{

	public class StoreFactory
	{
		public INeo4jRestApi CreateNeo4jRestApi(string connectionName)
		{
			var dbUrl = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString.TrimEnd('/');
			var provider = ConfigurationManager.ConnectionStrings[connectionName].ProviderName.ToLower();

			switch (provider)
			{
				case "":
				case "neo4j" :
					return new Neo4jRestApi(dbUrl);

				case "inmemory":
					throw new NotImplementedException();

				default:
					throw new NotImplementedException();
			}
		}
	}
}
