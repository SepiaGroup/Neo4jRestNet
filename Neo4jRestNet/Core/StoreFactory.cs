using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Neo4jRestNet.Rest;

namespace Neo4jRestNet.Core
{

	public enum DbStore
	{
		Default,
		InMemory
	}
	public class StoreFactory
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');

		public Neo4jRestApi Create(DbStore dbStore)
		{
			switch (dbStore)
			{
				case DbStore.Default:
					return new Neo4jRestApi(DefaultDbUrl);

				case DbStore.InMemory:
					throw new NotImplementedException();

				default:
					throw new NotImplementedException();
			}
		}
	}
}
