using System;
using System.Configuration;
using System.Net;

namespace Neo4jRestNet.CleanDbPlugin
{
    public class CleanDbPlugin
    {
        private static readonly string _defaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
        private static readonly string _cleanDbPluginPath = ConfigurationManager.ConnectionStrings["cleandb"].ConnectionString.TrimEnd('/');

        public static HttpStatusCode CleanDb()
        {
			var uri = new Uri(_defaultDbUrl);
			
            return Rest.HttpRest.Delete(string.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, _cleanDbPluginPath));
        }
    }
}
