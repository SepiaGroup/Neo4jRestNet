using System.Collections.Generic;
using System.Net;
using Neo4jRestNet.Configuration;

namespace Neo4jRestNet.Core
{
	public interface IRelationshipStore : IGraphObject
	{
		Relationship GetRelationship(ConnectionElement connection, long relationshipId);

		IEnumerable<Relationship> GetRelationship(ConnectionElement connection, string indexName, string key, object value);
		IEnumerable<Relationship> GetRelationship(ConnectionElement connection, string indexName, string searchQuery);

		Relationship CreateRelationship(ConnectionElement connection, Node startNode, Node endNode, string name, Properties properties);
		Relationship CreateUniqueRelationship(ConnectionElement connection, Node startNode, Node endNode, string name, Properties properties, string indexName, string key, object value, IndexUniqueness uniqueness);

		Relationship Initilize(ConnectionElement connection, long id, Properties properties);
		Relationship Initilize(string selfUri, Properties properties);

		HttpStatusCode DeleteRelationship(ConnectionElement connection);

		Properties GetProperties();
		void SaveProperties(Properties properties);

		Node StartNode { get; }
		Node EndNode { get; }
		string Type { get; }

		Relationship AddToIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value);
		bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName);
		bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key);
		bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value);

	}
}
