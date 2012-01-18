using System.Collections.Generic;
using System.Net;
using Neo4jRestNet.Core;

namespace Neo4jRestNet.Rest
{
	public interface INeo4jRestApi
	{
		HttpStatusCode GetRoot(out string response);
		HttpStatusCode CreateNode(string jsonProperties, out string response);
		HttpStatusCode GetNode(long nodeId, out string response);
		HttpStatusCode SetPropertiesOnNode(long nodeId, string jsonProperties);
		HttpStatusCode GetPropertiesOnNode(long nodeId, out string response);
		HttpStatusCode RemovePropertiesFromNode(string dbUrl, long nodeId);
		HttpStatusCode SetPropertyOnNode(string dbUrl, long nodeId, string propertyName, object value);
		HttpStatusCode GetPropertyOnNode(string dbUrl, long nodeId, string propertyName, out string response);
		HttpStatusCode RemovePropertyFromNode(string dbUrl, long nodeId, string propertyName);
		HttpStatusCode DeleteNode(long nodeId);
		HttpStatusCode CreateRelationship(long fromNodeId, string toNodeSelf, string name, string jsonProperties, out string response);
		HttpStatusCode SetPropertiesOnRelationship(string dbUrl, long relationshipId, string jsonProperties);
		HttpStatusCode GetPropertiesOnRelationship(string dbUrl, long relationshipId, out string response);
		HttpStatusCode RemovePropertiesFromRelationship(string dbUrl, long relationshipId);
		HttpStatusCode SetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName, object value);
		HttpStatusCode GetPropertyOnRelationship(string dbUrl, long relationshipId, string propertyName, out string response);
		HttpStatusCode RemovePropertyFromRelationship(string dbUrl, long relationshipId, string propertyName);
		HttpStatusCode DeleteRelationship(string dbUrl, long relationshipId);
		HttpStatusCode GetRelationshipsOnNode(long nodeId, RelationshipDirection direction, IEnumerable<string> relationships, out string response);
		HttpStatusCode GetRelationshipTypes(string dbUrl, out string response);
		HttpStatusCode CreateNodeIndex(string dbUrl, string indexName, string jsonConfig, out string response);
		HttpStatusCode CreateRelationshipIndex(string dbUrl, string indexName, string jsonConfig, out string response);
		HttpStatusCode DeleteNodeIndex(string dbUrl, string indexName);
		HttpStatusCode DeleteRelationshipIndex(string dbUrl, string indexName);
		HttpStatusCode ListNodeIndexes(string dbUrl, out string response);
		HttpStatusCode ListRelationshipIndexes(string dbUrl, out string response);
		HttpStatusCode AddNodeToIndex(long nodeId, string indexName, string key, object value, out string response);
		HttpStatusCode AddNodeToIndex(string nodeSelf, string indexName, string key, object value, out string response);
		HttpStatusCode AddRelationshipToIndex(string dbUrl, long relationshipId, string indexName, string key, object value, out string response);
		HttpStatusCode AddRelationshipToIndex(string dbUrl, string relationshipself, string indexName, string key, object value, out string response);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key, object value);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName);
		HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName, string key, object value);
		HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName, string key);
		HttpStatusCode RemoveRelationshipFromIndex(string dbUrl, long relationshipId, string indexName);
		HttpStatusCode GetNode(string indexName, string key, object value, out string response);
		HttpStatusCode GetNode(string indexName, string searchQuery, out string response);
		HttpStatusCode GetRelationship(string dbUrl, string indexName, string key, object value, out string response);
		HttpStatusCode GetRelationship(string dbUrl, string indexName, string searchQuery, out string response);

		HttpStatusCode Traverse(long nodeId, Order order, Uniqueness uniqueness,
		                        IEnumerable<TraverseRelationship> relationships,
		                        PruneEvaluator pruneEvaluator, ReturnFilter returnFilter, int? maxDepth,
		                        ReturnType returnType, out string response);

		HttpStatusCode PathBetweenNodes(string dbUrl, long fromNodeId, long toNodeId,
		                                IEnumerable<TraverseRelationship> relationships, int maxDepth,
		                                PathAlgorithm algorithm, bool returnAllPaths, out string response);
	}
}