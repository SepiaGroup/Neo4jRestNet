using System.Net;
using System.Data;
using System.Collections.Generic;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.Interface;


namespace Neo4jRestNet.GremlinPlugin
{
	public interface IGremlin
	{
		HttpStatusCode Post(GremlinScript script);

		IEnumerable<INode> GetNodes(GremlinScript script);
		IEnumerable<INode> GetNodes<T>(GremlinScript script) where T : class, INode, new();
		IEnumerable<IRelationship> GetRelationships(GremlinScript script);
		IEnumerable<IRelationship> GetRelationships<T>(GremlinScript script) where T : class, IRelationship, new();
		IEnumerable<IPath> GetPaths(GremlinScript script);
		IEnumerable<IPath> GetPaths<T>(GremlinScript script) where T : class, IPath, new();
	
		DataTable GetTable(GremlinScript script);
	}
}
