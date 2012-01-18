using System;
using System.Net;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using Neo4jRestNet.Core.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.Implementation;


namespace Neo4jRestNet.GremlinPlugin
{
	public class Gremlin : IGremlin
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private static readonly string DefaultGremlinExtensionPath = ConfigurationManager.ConnectionStrings["neo4jGremlinExtension"].ConnectionString.TrimEnd('/');

		private readonly string _gremlinUrl;

		public Gremlin()
		{
			_gremlinUrl = string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath).TrimEnd('/'); 
		}

		public Gremlin(string connectionString)
		{
			_gremlinUrl = string.Concat(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString.TrimEnd('/'), DefaultGremlinExtensionPath).TrimEnd('/'); 
		}

		public HttpStatusCode Post(GremlinScript script)
		{
			var jo = new JObject { { "script", script.GetScript() } };

			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, jo.ToString(Formatting.None), out response);

			return status;
		}

		public IEnumerable<INode>GetNodes(GremlinScript script)
		{
			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, script.GetScript(), out response);
			return GraphFactory.CreateNode().ParseJson(response);
		}
		
		public IEnumerable<INode> GetNodes<T>(GremlinScript script) where T : class, INode, new()
		{
			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, script.GetScript(), out response);
			return GraphFactory.CreateNode<T>().ParseJson(response);
		}

		public IEnumerable<IRelationship> GetRelationships(GremlinScript script)
		{
			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, script.GetScript(), out response);
			return GraphFactory.CreateRelationship().ParseJson(response);
		}

		public IEnumerable<IRelationship> GetRelationships<T>(GremlinScript script) where T : class, IRelationship, new()
		{
			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, script.GetScript(), out response);
			return GraphFactory.CreateRelationship<T>().ParseJson(response);
		}

		public IEnumerable<IPath> GetPaths(GremlinScript script)
		{
			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, script.GetScript(), out response);
			return GraphFactory.CreatePath().ParseJson(response);
		}

		public IEnumerable<IPath> GetPaths<T>(GremlinScript script) where T : class, IPath, new()
		{
			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, script.GetScript(), out response);
			return GraphFactory.CreatePath<T>().ParseJson(response);
		}

		public DataTable GetTable(GremlinScript script)
		{
			

			string response;
			var status = Rest.HttpRest.Post(_gremlinUrl, script.GetScript(), out response);

			var joResponse = JObject.Parse(response);
			var jaColumns =(JArray)joResponse["columns"];
			var jaData = (JArray)joResponse["data"];
			
			var dt = new DataTable();

			var initColumns = true;
			var colIndex = 0;
			foreach (JArray jRow in jaData)
			{
				var row = new List<object>();
				foreach (var jCol in jRow)
				{
					switch (jCol.Type)
					{
						case JTokenType.String:
							row.Add(jCol.ToString());
							if (initColumns)
							{
								dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(string));
								colIndex++;
							}
							break;

						case JTokenType.Object:
							if (jCol["self"] == null)
							{
								row.Add(jCol.ToString());

								if (initColumns)
								{
									dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(string));
									colIndex++;
								}
							}
							else
							{
								var self = jCol["self"].ToString();
								var selfArray = self.Split('/');
								if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node"  )
								{
									row.Add(new Node().InitializeFromNodeJson((JObject)jCol));

									if (initColumns)
									{
										dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(INode));
										colIndex++;
									}
								}
								else if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "relationship")
								{
									row.Add(new Relationship().InitializeFromRelationshipJson((JObject)jCol));

									if (initColumns)
									{
										dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(Relationship));
										colIndex++;
									}
								}
								else
								{
									// Not a Node or Relationship - return as string
									row.Add(jCol.ToString());

									if (initColumns)
									{
										dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(string));
										colIndex++;
									}
								}
							}
							break;

						case JTokenType.Integer:
							row.Add(jCol.ToString());
							if (initColumns)
							{
								dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(int));
								colIndex++;
							}
							break;

						case JTokenType.Float:
							row.Add(jCol.ToString());
							if (initColumns)
							{
								dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(float));
								colIndex++;
							}
							break;

						case JTokenType.Date:
							row.Add(jCol.ToString());
							if (initColumns)
							{
								dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(DateTime));
								colIndex++;
							}
							break;

						case JTokenType.Boolean:
							row.Add(jCol.ToString());
							if (initColumns)
							{
								dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(bool));
								colIndex++;
							}
							break;

						default:
							row.Add(jCol.ToString());

							if (initColumns)
							{
								dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(string));
								colIndex++;
							}
							break;
					}
				}

				initColumns = false;
				var dtRow = dt.NewRow();
				dtRow.ItemArray = row.ToArray();
				dt.Rows.Add(dtRow);
			}

			return dt;
		}
	}
}
