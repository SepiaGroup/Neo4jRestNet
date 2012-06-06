using System.Collections.Generic;
using System.Net;
using Neo4jRestNet.Configuration;

namespace Neo4jRestNet.Core
{
	public interface INodeStore : IGraphObject
	{
		Node GetRootNode(ConnectionElement connection);

		Node GetNode(ConnectionElement connection, long nodeId);
		IEnumerable<Node> GetNode(ConnectionElement connection, string indexName, string key, object value);
		IEnumerable<Node> GetNode(ConnectionElement connection, string indexName, string searchQuery);

		Node CreateNode(ConnectionElement connection, Properties properties);
		Node CreateUniqueNode(ConnectionElement connection, Properties properties, string indexName, string key, object value);

		Node Initilize(ConnectionElement connection, long id, Properties properties);
		Node Initilize(string selfUri, Properties properties);

		HttpStatusCode DeleteNode();

		Properties GetProperties();
		void SaveProperties(Properties properties);

		Relationship CreateRelationship(Node startNode, Node endNode, string relationshipType, Properties properties);

		IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> relationshipTypes);

		Node AddToIndex(ConnectionElement connection, Node node, string indexName, string key, object value, bool unique = false);
		bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName);
		bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key);
		bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key, object value);
	}
}
