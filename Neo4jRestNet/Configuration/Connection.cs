using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Neo4jRestNet.Core.Exceptions;
using Neo4jRestNet.Rest;
using Newtonsoft.Json;

namespace Neo4jRestNet.Configuration
{
	class Connection
	{
		static readonly ConcurrentDictionary<string, DatabaseEndpoint> DatabaseEndpoints = new ConcurrentDictionary<string, DatabaseEndpoint>();
		private static readonly ConcurrentDictionary<string, ServiceRoot> ServiceRoots = new ConcurrentDictionary<string, ServiceRoot>();

		public static DatabaseEndpoint GetDatabaseEndpoint(string dbUrl)
		{
			DatabaseEndpoint endpoint;

			if (!DatabaseEndpoints.TryGetValue(dbUrl, out endpoint))
			{
				string response;

				if (Neo4jRestApi.GetRoot(dbUrl, out response) == HttpStatusCode.OK)
				{
					endpoint = JsonConvert.DeserializeObject<DatabaseEndpoint>(response);
					DatabaseEndpoints.TryAdd(dbUrl, endpoint);
				}
				else
				{
					throw new DatabaseEndpointNotFound();
				}
			}

			return endpoint;
		}

		public static ServiceRoot GetServiceRoot(string dbUrl)
		{
			ServiceRoot serviceRoot;

			if (!ServiceRoots.TryGetValue(dbUrl, out serviceRoot))
			{
				string response;

				if (Neo4jRestApi.GetRoot(GetDatabaseEndpoint(dbUrl).Data, out response) == HttpStatusCode.OK)
				{
					serviceRoot = JsonConvert.DeserializeObject<ServiceRoot>(response);
					ServiceRoots.TryAdd(dbUrl, serviceRoot);
				}
				else
				{
					throw new ServiceRootNotFound();
				}
			}

			return serviceRoot;
		}
	}
}
