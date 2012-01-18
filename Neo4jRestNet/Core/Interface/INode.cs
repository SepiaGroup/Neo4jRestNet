using System.Collections.Generic;
using System.Net;
using System;
using Newtonsoft.Json.Linq;
using Neo4jRestNet.Rest;

namespace Neo4jRestNet.Core.Interface
{
	public interface INode : IGraphObject
	{
		long Id { get; }
		EncryptId EncryptedId { get; }
		string OriginalJsonNode { get; }
		Properties Properties { get; }
		string NodeType { get; }

		INode GetRootNode();
		
		INode GetNode(EncryptId nodeId);
		IEnumerable<INode> GetNode(string indexName, string key, object value);
		IEnumerable<INode> GetNode(Enum indexName, string key, object value);
		IEnumerable<INode> GetNode(string indexName, Enum key, object value);
		IEnumerable<INode> GetNode(Enum indexName, Enum key, object value);
		IEnumerable<INode> GetNode(string indexName, string searchQuery);
		IEnumerable<INode> GetNode(Enum indexName, string searchQuery);
		
		INode CreateNode(string nodeType);
		INode CreateNode(Enum nodeType);
		INode CreateNode(string nodeType, Properties properties);
		INode CreateNode(Enum nodeType, Properties properties);
		INode CreateNode(string nodeType, IDictionary<string, object> properties);
		INode CreateNode(Enum nodeType, IDictionary<string, object> properties);
 
		HttpStatusCode DeleteNode();

		INode InitializeFromNodeJson(string nodeJson);
		INode InitializeFromNodeJson(JObject nodeJson);

		INode InitializeFromSelf(string self);

		void SaveProperties();
		void SaveProperties(Properties properties);

		IEnumerable<IRelationship> GetRelationships();
		IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction);
		IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, IRelationship relationshipType);
		IEnumerable<IRelationship> GetRelationships(Enum name);
		IEnumerable<IRelationship> GetRelationships(string name);
		IEnumerable<IRelationship> GetRelationships(IEnumerable<Enum> names);
		IEnumerable<IRelationship> GetRelationships(IEnumerable<string> names);
		IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, string name);
		IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, Enum name);
		IEnumerable<IRelationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> names);
		
		IRelationship CreateRelationshipTo(INode toNode, string relationshipType);
		IRelationship CreateRelationshipTo(INode toNode, Enum relationshipType);
		IRelationship CreateRelationshipTo(INode toNode, Enum relationshipType, Properties relationshipProperties);
		IRelationship CreateRelationshipTo(INode toNode, string relationshipType, Properties relationshipProperties);
		
		IEnumerable<IGraphObject> Traverse(Order order, Uniqueness uniqueness, IEnumerable<TraverseRelationship> relationships, PruneEvaluator pruneEvaluator, ReturnFilter returnFilter, int? maxDepth, ReturnType returnType);
		
		INode AddNodeToIndex(long nodeId, string indexName, string key, object value);
		INode AddNodeToIndex(long nodeId, Enum indexName, string key, object value);
		INode AddNodeToIndex(long nodeId, string indexName, Enum key, object value);
		INode AddNodeToIndex(long nodeId, Enum indexName, Enum key, object value);
		
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, string key);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, Enum key);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, Enum key);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, string key, object value);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, string key, object value);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, string indexName, Enum key, object value);
		HttpStatusCode RemoveNodeFromIndex(long nodeId, Enum indexName, Enum key, object value);

		IEnumerable<INode> ParseJson(string jsonNodes);
	}
}
