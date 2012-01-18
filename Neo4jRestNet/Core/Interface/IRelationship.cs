using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Core.Interface
{
	public interface IRelationship : IGraphObject
	{
		INode StartNode { get; }
		INode EndNode { get; }
		string Name { get; }
		long Id { get; }
		EncryptId EncryptedId { get; }
		string OriginalJsonRelationship { get; }
		Properties Properties { get; }

		IEnumerable<IRelationship> GetRelationship(string indexName, string key, object value);
		IEnumerable<IRelationship> GetRelationship(Enum indexName, string key, object value);
		IEnumerable<IRelationship> GetRelationship(string indexName, Enum key, object value);
		IEnumerable<IRelationship> GetRelationship(Enum indexName, Enum key, object value);
		IEnumerable<IRelationship> GetRelationship(string indexName, string searchQuery);
		IEnumerable<IRelationship> GetRelationship(Enum indexName, string searchQuery);

		IRelationship InitializeFromRelationshipJson(string relationshipJson);
		IRelationship InitializeFromRelationshipJson(JObject relationshipJson);
		IRelationship InitializeFromSelf(string self);
		bool IsSelfARelationship(string self);

		void SaveProperties();
		void SaveProperties(Properties properties);

		IRelationship AddRelationshipToIndex(long relationshipId, string indexName, string key, object value);
		IRelationship AddRelationshipToIndex(long relationshipId, Enum indexName, string key, object value);
		IRelationship AddRelationshipToIndex(long relationshipId, string indexName, Enum key, object value);
		IRelationship AddRelationshipToIndex(long relationshipId, Enum indexName, Enum key, object value);
		
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, string key);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, string key);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, Enum key);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, Enum key);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, string key, object value);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, string key, object value);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, string indexName, Enum key, object value);
		HttpStatusCode RemoveRelationshipFromIndex(long relationshipId, Enum indexName, Enum key, object value);
		
		HttpStatusCode DeleteRelationship();
		
		IEnumerable<IRelationship> ParseJson(string jsonRelationships);
	}
}
