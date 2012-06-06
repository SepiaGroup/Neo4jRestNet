using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Neo4jRestNet.Configuration;
using Neo4jRestNet.Core.Exceptions;

namespace Neo4jRestNet.Core
{
	public class BatchNodeStore : INodeStore
	{
		private BatchStore _batchStore;

		public BatchNodeStore(BatchStore batchStore)
		{
			_batchStore = batchStore;	
		}

		public Node GetRootNode(ConnectionElement connection)
		{
			throw new NotImplementedException();
		}

		public Node GetNode(ConnectionElement connection, long nodeId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Node> GetNode(ConnectionElement connection, string indexName, string key, object value)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Node> GetNode(ConnectionElement connection, string indexName, string searchQuery)
		{
			throw new NotImplementedException();
		}

		public Node CreateNode(ConnectionElement connection, Properties properties)
		{
			throw new NotImplementedException();
		}

		public Node CreateUniqueNode(ConnectionElement connection, Properties properties, string indexName, string key, object value)
		{
			return _batchStore.CreateUniqueNode(connection, properties, indexName, key, value);
		}

		public Node Initilize(ConnectionElement connection, long id, Properties properties)
		{
			Id = id;
			return new Node(this, properties);
		}

		public Node Initilize(string selfUri, Properties properties)
		{
			throw new NotImplementedException();
		}

		public HttpStatusCode DeleteNode()
		{
			throw new BatchDeleteNotSupportedException();
		}

		public Properties GetProperties()
		{
			throw new NotImplementedException();
		}

		public void SaveProperties(Properties properties)
		{
			_batchStore.SaveProperties(this, properties);
		}

		public Relationship CreateRelationship(Node startNode, Node endNode, string relationshipType, Properties properties)
		{
			return _batchStore.CreateRelationship(startNode, endNode, relationshipType, properties);
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> relationshipTypes)
		{
			throw new BatchGetRelationshipsNotSupportedException();
		}

		public Node AddToIndex(ConnectionElement connection, Node node, string indexName, string key, object value, bool unique = false)
		{
			return _batchStore.AddToIndex(connection, node, indexName, key, value, unique);
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName)
		{
			throw new BatchRemoveFromIndexNotSupportedException();
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key)
		{
			throw new BatchRemoveFromIndexNotSupportedException();
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key, object value)
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
				return string.Concat("batch/node/", Id);
			}
		}
	}
}
