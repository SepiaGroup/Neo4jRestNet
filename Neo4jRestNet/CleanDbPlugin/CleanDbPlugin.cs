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
            return Rest.HttpRest.Delete(string.Concat(_defaultDbUrl, _cleanDbPluginPath));
        }
    }
}
