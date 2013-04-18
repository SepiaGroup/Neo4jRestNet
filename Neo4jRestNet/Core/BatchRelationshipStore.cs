using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Configuration;
using Neo4jRestNet.Core.Exceptions;

namespace Neo4jRestNet.Core
{
	public class BatchRelationshipStore : IRelationshipStore
	{
		private BatchStore _batchStore;

		public BatchRelationshipStore(BatchStore batchStore)
		{
			_batchStore = batchStore;	
		}

		public Relationship GetRelationship(ConnectionElement connection, long relationshipId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Relationship> GetRelationship(ConnectionElement connection, string indexName, string key, object value)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Relationship> GetRelationship(ConnectionElement connection, string indexName, string searchQuery)
		{
			throw new NotImplementedException();
		}

		public Relationship CreateRelationship(ConnectionElement connection, Node startNode, Node endNode, string name, Properties properties)
		{
			throw new NotImplementedException();
		}

		public Relationship CreateUniqueRelationship(ConnectionElement connection, Node startNode, Node endNode, string name, Properties properties, string indexName, string key, object value, IndexUniqueness uniqueness)
		{
			return _batchStore.CreateUniqueRelationship(connection, startNode, endNode, name, properties, indexName, key, value, uniqueness);
		}

		public Relationship Initilize(ConnectionElement connection, long id, Properties properties)
		{
			Id = id;
			return new Relationship(this, properties);
		}

		public Relationship Initilize(string selfUri, Properties properties)
		{
			throw new NotImplementedException();
		}

		public System.Net.HttpStatusCode DeleteRelationship(ConnectionElement connection)
		{
			throw new NotImplementedException();
		}

		public Properties GetProperties()
		{
			_batchStore.GetProperties(this);

			return null;
		}

		public void SaveProperties(Properties properties)
		{
			_batchStore.SaveProperties(this, properties);
		}

		public Node StartNode
		{
			get { throw new NotImplementedException(); }
		}

		public Node EndNode
		{
			get { throw new NotImplementedException(); }
		}

		public string Type
		{
			get { throw new NotImplementedException(); }
		}

		public Relationship AddToIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value)
		{
			return _batchStore.AddToIndex(connection, relationship, indexName, key, value);
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName)
		{
			throw new BatchRemoveFromIndexNotSupportedException();
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key)
		{
			throw new BatchRemoveFromIndexNotSupportedException();
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value)
		{
			throw new BatchRemoveFromIndexNotSupportedException();
		}

		public long Id { get; private set; }

		public string DbUrl
		{
			get { return "batch"; }
		}

		public string Self
		{
			get
			{
				return string.Concat("batch/relationship/", Id);
			}
		}
	}
}
