using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Neo4jRestNet.Configuration;
using Neo4jRestNet.Core.Exceptions;
using Neo4jRestNet.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Core
{

	class BatchJob
	{
		public HttpRest.Method Method { get; set; }
		public string To { get; set; }
		public JObject Body { get; set; }
		public object GraphObject { get; set; }
		public long Id { get; set; }
		public bool ReloadProperties { get; set; }
	}

	public class BatchStore : INodeStore, IRelationshipStore
	{

		private List<BatchJob> _jobs = new List<BatchJob>();
		public bool Open { get; private set; }

		private string _jsonResponse;
		private bool _parsed;

		public BatchStore()
		{
			Open = true;
		}

		private bool IsBatchObject(IGraphObject obj)
		{
			return obj != null && obj.DbUrl == "batch";
		}

		public bool Execute()
		{
			return Execute(ConnectionManager.Connection());
		}

		public bool Execute(ConnectionElement connection)
		{
			Open = false;  // Close the Batch

			var jsonJobs = new JArray();

			var index = 0;
			foreach (var job in _jobs)
			{
				var jsonJobDescription = new JObject
				              	{
				              		{"method", JToken.FromObject(job.Method.ToString())}, 
									{"to", JToken.FromObject(job.To)},
									{"id", JToken.FromObject(index)}
				              	};

				if (job.Body != null)
				{
					jsonJobDescription.Add("body", job.Body);
				}

				jsonJobs.Add(jsonJobDescription);

				index++;
			}

			string response;
			var status = HttpRest.Post(string.Concat(connection.DbUrl, "/batch"), jsonJobs.ToString(Formatting.None, new IsoDateTimeConverter()), out response);

			if (status != HttpStatusCode.OK)
			{
				return false;
			}

			_jsonResponse = response;

			return true;
		}

		private IGraphObject ParseJsonGraphObject(JObject jsonGraphObject, bool reloadProperties)
		{
			var selfArray = jsonGraphObject["self"].Value<string>().Split('/');
			if (selfArray.Length < 2)
			{
				throw new Exception("Invalid response from batch");
			}

			switch (selfArray[selfArray.Length - 2])
			{
				case "node":
					var node = RestNodeStore.CreateNodeFromJson(jsonGraphObject);
					if (reloadProperties)
					{
						node.Properties = null;
					}

					return node;

				case "relationship":
					var relationship = RestRelationshipStore.CreateRelationshipFromJson(jsonGraphObject);
					if (reloadProperties)
					{
						relationship.Properties = null;
					}

					return relationship;

				default:
					throw new Exception("Invalid response from batch");
			}
		}

		private void ParseJsonResponse()
		{
			foreach (var jobResponse in JArray.Parse(_jsonResponse))
			{
				var id = jobResponse["id"].Value<int>();
				//var from = jobResponse["from"].Value<string>();
				//var location = jobResponse["location"].Value<string>();

				if (jobResponse["body"] == null)
				{
					return;
				}

				switch (jobResponse["body"].Type)
				{
					case JTokenType.Object:
						_jobs[id].GraphObject = ParseJsonGraphObject(jobResponse["body"].Value<JObject>(), _jobs[id].ReloadProperties);
						break;

					case JTokenType.Array:
						_jobs[id].GraphObject = jobResponse["body"].Value<JArray>().Select(objBody => ParseJsonGraphObject((JObject)objBody, _jobs[id].ReloadProperties)).ToList();
						break;

					default:
						throw new Exception("Invalid response from batch");
				}
			}
		}

		public T GetGraphObject<T>(T graphObject) where T : IGraphObject
		{
			if (Open)
			{
				throw new Exception("Batch store must be closed before a calling GetGraphObject");
			}

			if (!_parsed)
			{
				ParseJsonResponse();
				_parsed = true;
			}

			return (T)_jobs[(int)graphObject.Id].GraphObject;
		}

		public IEnumerable<T> GetGraphObject<T>(IEnumerable<T> graphObjects) where T : IGraphObject
		{
			if (Open)
			{
				throw new Exception("Batch store must be closed before a calling GetGraphObject");
			}

			if (!_parsed)
			{
				ParseJsonResponse();
				_parsed = true;
			}

			return (from graphObj in (IEnumerable<IGraphObject>) _jobs[(int) graphObjects.First().Id].GraphObject select (T) graphObj).ToList();
		}

		#region INodeStore Implementation

		public Node GetRootNode(ConnectionElement connection)
		{
			throw new NotImplementedException();
		}

		public Node GetNode(ConnectionElement connection, long nodeId)
		{
			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Get,
				To = string.Concat("/node/", nodeId),
				Id = index
			});

			_jobs[index].GraphObject = new BatchNodeStore(this).Initilize(null, index, null);

			return (Node)_jobs[index].GraphObject;
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
			var index = _jobs.Count();
			_jobs.Add(new BatchJob
						{
							Method = HttpRest.Method.Post,
							To = "/node",
							Body = properties == null ? null : properties.ToJObject(),
							Id = index
						});

			_jobs[index].GraphObject = new BatchNodeStore(this).Initilize(null, index, properties);

			return (Node)_jobs[index].GraphObject;
		}

		public Node CreateUniqueNode(ConnectionElement connection, Properties properties, string indexName, string key, object value, IndexUniqueness uniqueness)
		{
			var body = new JObject
			           	{
			           		{"value", JToken.FromObject(value)},
							{"properties", properties.ToJObject()},
							{"key", key}
			           	};

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Post,
				To = string.Format("/index/node/{0}?uniqueness={1}", indexName, uniqueness),
				Id = index,
				Body = body
			});

			_jobs[index].GraphObject = new BatchNodeStore(this).Initilize(null, index, null);

			return (Node)_jobs[index].GraphObject;
		}

		public Node Initilize(ConnectionElement connection, long id, Properties properties)
		{
			throw new NotImplementedException();
		}

		public Node Initilize(string selfUri, Properties properties)
		{
			throw new NotImplementedException();
		}

		public void Delete(Node node)
		{
			if (IsBatchObject(node))
			{
				throw new BatchDeleteNotSupportedException();
			}

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Delete,
				To = string.Format("/node/{0}", node.Id),
				Id = index,
			});

			_jobs[index].GraphObject = null;
		}

		public void Delete(Relationship relationship)
		{

			if (!IsBatchObject(relationship))
			{
				throw new BatchDeleteNotSupportedException();
			}

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Delete,
				To = string.Format("/relationship/{0}", relationship.Id),
				Id = index,
			});

			_jobs[index].GraphObject = null;
		}

		public HttpStatusCode DeleteNode()
		{
			throw new NotImplementedException();
		}

		public Properties GetProperties()
		{
			throw new NotImplementedException();
		}

		public void SaveProperties(Properties properties)
		{
			throw new NotImplementedException();
		}

		public Relationship CreateRelationship(Node startNode, Node endNode, string relationshipType, Properties properties)
		{

			var startFormat = IsBatchObject(startNode) ? "{{{0}}}/relationships" : "/node/{0}/relationships";
			var endFormat = IsBatchObject(endNode) ? "{{{0}}}" : "/node/{0}";

			var body = new JObject
			           	{
			           		{"to", string.Format(endFormat, endNode.Id)},
							{"type", relationshipType}
			           	};

			if (properties != null)
			{
				body.Add("data", properties.ToJObject());
			}

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
						{
							Method = HttpRest.Method.Post,
							To = string.Format(startFormat, startNode.Id),
							Id = index,
							Body = body
						});

			_jobs[index].GraphObject = new BatchRelationshipStore(this).Initilize(null, index, properties);

			return (Relationship)_jobs[index].GraphObject;
		}

		public IEnumerable<Relationship> GetRelationships(RelationshipDirection direction, IEnumerable<string> relationshipTypes)
		{
			throw new NotImplementedException();
		}

		public Node AddToIndex(ConnectionElement connection, Node node, string indexName, string key, object value, bool unique = false)
		{
			var uriFormat = IsBatchObject(node) ? "{{{0}}}" : "/node/{0}";

			var body = new JObject
			           	{
			           		{"value", JToken.FromObject(value)},
							{"uri", string.Format(uriFormat, node.Id)},
							{"key", key}
			           	};

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Post,
				To = string.Format("/index/node/{0}{1}", indexName, unique ? "?unique" : string.Empty),
				Id = index,
				Body = body
			});

			_jobs[index].GraphObject = new BatchNodeStore(this).Initilize(null, index, null);

			return (Node)_jobs[index].GraphObject;
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName)
		{
			return RemoveFromIndex(connection, node, indexName, null, null);
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key)
		{
			return RemoveFromIndex(connection, node, indexName, key, null);
		}

		public bool RemoveFromIndex(ConnectionElement connection, Node node, string indexName, string key, object value)
		{
			if (IsBatchObject(node))
			{
				throw new BatchRemoveFromIndexNotSupportedException();
			}

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Delete,
				To = string.IsNullOrEmpty(key) ? string.Format("/index/node/{0}/{1}", indexName, node.Id) : 
							value == null ? string.Format("/index/node/{0}/{1}/{2}", indexName, key, node.Id) :
							string.Format("/index/node/{0}/{1}/{2}/{3}", indexName, key, 
													value is string ? Uri.EscapeDataString(value.ToString()) : value.ToString(), 
													node.Id),
				Id = index
			});

			_jobs[index].GraphObject = null;

			return true;
		}

		public long Id
		{
			get { throw new NotImplementedException(); }
		}

		public string DbUrl
		{
			get { throw new NotImplementedException(); }
		}

		public string Self
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IRelationshipStore Implementation

		public Relationship GetRelationship(ConnectionElement connection, long relationshipId)
		{
			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Get,
				To = string.Concat("/relationship/", relationshipId),
				Id = index
			});

			_jobs[index].GraphObject = new BatchRelationshipStore(this).Initilize(null, index, null);

			return (Relationship)_jobs[index].GraphObject;
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
			var startFormat = IsBatchObject(startNode) ? "{{{0}}}/relationships" : "/node/{0}/relationships";
			var endFormat = IsBatchObject(endNode) ? "{{{0}}}" : "/node/{0}";

			var body = new JObject
			           	{
			           		{"to", string.Format(endFormat, endNode.Id)},
							{"type", name}
			           	};

			if (properties != null)
			{
				body.Add("data", properties.ToJObject());
			}

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Post,
				To = string.Format(startFormat, startNode.Id),
				Id = index,
				Body = body
			});

			_jobs[index].GraphObject = new BatchRelationshipStore(this).Initilize(null, index, properties);

			return (Relationship)_jobs[index].GraphObject;
		}

		public Relationship CreateUniqueRelationship(ConnectionElement connection, Node startNode, Node endNode, string name, Properties properties, string indexName, string key, object value, IndexUniqueness uniqueness)
		{
			var startFormat = IsBatchObject(endNode) ? "{{{0}}}" : "/node/{0}";
			var endFormat = IsBatchObject(startNode) ? "{{{0}}}" : "/node/{0}";

			var body = new JObject
			           	{
			           		{"value", JToken.FromObject(value)},
							{"properties", properties.ToJObject()},
							{"key", key},
							{"start", string.Format(startFormat, startNode.Id)},
							{"end", string.Format(endFormat, endNode.Id)},
							{"type", name}
			           	};

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Post,
				To = string.Format("/index/relationship/{0}?uniqueness={1}", indexName, uniqueness),
				Id = index,
				Body = body
			});

			_jobs[index].GraphObject = new BatchRelationshipStore(this).Initilize(null, index, null);

			return (Relationship)_jobs[index].GraphObject;
		}

		Relationship IRelationshipStore.Initilize(ConnectionElement connection, long id, Properties properties)
		{
			throw new NotImplementedException();
		}

		Relationship IRelationshipStore.Initilize(string selfUri, Properties properties)
		{
			throw new NotImplementedException();
		}

		public HttpStatusCode DeleteRelationship(ConnectionElement connection)
		{
			throw new NotImplementedException();
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

		public Relationship AddToIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value, bool unique = false)
		{
			var uriFormat = IsBatchObject(relationship) ? "{{{0}}}" : "/relationship/{0}";

			var body = new JObject
			           	{
			           		{"value", JToken.FromObject(value)},
							{"uri", string.Format(uriFormat, relationship.Id)},
							{"key", key}
			           	};

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Post,
				To = string.Format("/index/relationship/{0}{1}", indexName, unique ? "?unique" : string.Empty),
				Id = index,
				Body = body
			});

			_jobs[index].GraphObject = new BatchRelationshipStore(this).Initilize(null, index, null);

			return (Relationship)_jobs[index].GraphObject;
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName)
		{
			return RemoveFromIndex(connection, relationship, indexName, null, null);
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key)
		{
			return RemoveFromIndex(connection, relationship, indexName, key, null);
		}

		public bool RemoveFromIndex(ConnectionElement connection, Relationship relationship, string indexName, string key, object value)
		{
			if (IsBatchObject(relationship))
			{
				throw new BatchRemoveFromIndexNotSupportedException();
			}

			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Delete,
				To = string.IsNullOrEmpty(key) ? string.Format("/index/relationship/{0}/{1}", indexName, relationship.Id) :
							value == null ? string.Format("/index/relationship/{0}/{1}/{2}", indexName, key, relationship.Id) :
							string.Format("/index/relationship/{0}/{1}/{2}/{3}", indexName, key,
														value is string ? Uri.EscapeDataString(value.ToString()) : value.ToString(),
														relationship.Id),
				Id = index
			});

			_jobs[index].GraphObject = null;

			return true;
		}

		#endregion

		#region Node Batch Methods

		public void SaveProperties(BatchNodeStore node, Properties properties)
		{
			var index = _jobs.Count();

			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Put,
				To = string.Format("{{{0}}}/properties", node.Id),
				Body = properties.ToJObject(),
				Id = index
			});

			_jobs[(int)node.Id].ReloadProperties = true;

			_jobs[index].GraphObject = null;
		}

		#endregion

		#region Relationship Batch Methods

		public void GetProperties(BatchRelationshipStore relationship, string propertyName = null)
		{
			var index = _jobs.Count();

			var prop = string.Empty;
			if (!string.IsNullOrEmpty(propertyName))
			{
				prop = string.Concat("/", propertyName);
			}

			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Get,
				To = string.Concat("/relationship/", relationship.Id, prop),
				Id = index
			});

			_jobs[index].GraphObject = null;
		}

		public void SaveProperties(BatchRelationshipStore relationship, Properties properties)
		{
			var index = _jobs.Count();

			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Put,
				To = string.Format("{{{0}}}/properties", relationship.Id),
				Body = properties.ToJObject(),
				Id = index
			});

			_jobs[(int)relationship.Id].ReloadProperties = true;

			_jobs[index].GraphObject = null;
		}

		public Relationship GetBatchRelationship(long relationshipId)
		{
			var index = _jobs.Count();
			_jobs.Add(new BatchJob
			{
				Method = HttpRest.Method.Get,
				To = string.Concat("/relationship/{", relationshipId, "}"),
				Id = index
			});

			_jobs[index].GraphObject = new BatchRelationshipStore(this).Initilize(null, index, null);

			return (Relationship)_jobs[index].GraphObject;
		}

		public IEnumerable<Relationship> GetRelationships(Node node, RelationshipDirection direction, IEnumerable<string> relationshipTypes = null)
		{
			if(IsBatchObject(node))
			{
				throw new BatchGetRelationshipsNotSupportedException();
			}
				
			var index = _jobs.Count();

			if (relationshipTypes == null || !relationshipTypes.Any())
			{
				_jobs.Add(new BatchJob
				          	{
				          		Method = HttpRest.Method.Get,
				          		To = string.Format("/node/{0}/relationships/{1}", node.Id, direction),
				          		Id = index
				          	});
			}
			else
			{
				_jobs.Add(new BatchJob
				{
					Method = HttpRest.Method.Get,
					To = string.Format("/node/{0}/relationships/{1}/{2}", node.Id, direction, string.Join("&", relationshipTypes)),
					Id = index
				});

			}
			_jobs[index].GraphObject = new BatchRelationshipStore(this).Initilize(null, index, null);

			return new List<Relationship> { (Relationship)_jobs[index].GraphObject };
		}

		#endregion

	}
}
