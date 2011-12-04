using System;
using System.Net;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Neo4jRestNet.Core;


namespace Neo4jRestNet.GremlinPlugin
{
	public class Gremlin
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private static readonly string DefaultGremlinExtensionPath = ConfigurationManager.ConnectionStrings["neo4jGremlinExtension"].ConnectionString.TrimEnd('/');

		public static HttpStatusCode Post(GremlinScript script)
		{
			return Post(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), script.ToString());
		}
	
		public static HttpStatusCode Post(string script)
		{
			 return Post(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), script);
		}

		public static HttpStatusCode Post(string gremlinUrl, GremlinScript script)
		{
			return Post(gremlinUrl, script.ToString());
		}

		public static HttpStatusCode Post(string gremlinUrl, string script)
		{
			// Remove trailing /
			gremlinUrl = gremlinUrl.TrimEnd('/');

			var jo = new JObject {{"script", script}};

			string response;
			HttpStatusCode status = Rest.HttpRest.Post(gremlinUrl, jo.ToString(Formatting.None), out response);

			return status;
		}

		public static IEnumerable<T> Post<T>(EncryptId startNodeId, string script) where T : IGraphObject
		{
			return Post<T>(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), string.Format("g.v({0}).{1}", (long)startNodeId, script)); 
		}
		
		public static IEnumerable<T> Post<T>(string script) where T : IGraphObject
		{
			return Post<T>(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), script);
		}

		public static IEnumerable<T> Post<T>(GremlinScript script) where T : IGraphObject
		{
			return Post<T>(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), script.ToString());
		}

		public static IEnumerable<T> Post<T>(string gremlinUrl, GremlinScript script) where T : IGraphObject
		{
			return Post<T>(gremlinUrl, script.ToString());
		}

		public static IEnumerable<T> Post<T>(string gremlinUrl, string script) where T : IGraphObject
		{
			// Remove trailing /
			gremlinUrl = gremlinUrl.TrimEnd('/');

			var typeParameterType = typeof(T);

			var jo = new JObject {{"script", script}};

			string response;
			HttpStatusCode status = Rest.HttpRest.Post(gremlinUrl, jo.ToString(Formatting.None), out response);

			if (typeParameterType == typeof(Node))
			{
				return (IEnumerable<T>)Node.ParseJson(response);
			}
			
			if (typeParameterType == typeof(Relationship))
			{
				return (IEnumerable<T>)Relationship.ParseJson(response);
			}
			
			if (typeParameterType == typeof(Path))
			{
				return (IEnumerable<T>)Path.ParseJson(response);
			}

			throw new Exception("Return type " + typeParameterType.ToString() + " not implemented");
		}

		public static DataTable GetTable(string script)
		{
			return GetTable(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), script);
		}

		public static DataTable GetTable(GremlinScript script)
		{
			return GetTable(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), script.ToString());
		}

		public static DataTable GetTable(string gremlinUrl, GremlinScript script)
		{
			return GetTable(gremlinUrl, script.ToString());
		}

		public static DataTable GetTable(string gremlinUrl, string script) 
		{
			// Remove trailing /
			gremlinUrl = gremlinUrl.TrimEnd('/');

			var joScript = new JObject {{"script", script}};

			string response;
			HttpStatusCode status = Rest.HttpRest.Post(gremlinUrl, joScript.ToString(Formatting.None), out response);

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
								string self = jCol["self"].ToString();
								string[] selfArray = self.Split('/');
								if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node"  )
								{
									row.Add(Node.InitializeFromNodeJson((JObject)jCol));

									if (initColumns)
									{
										dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(Node));
										colIndex++;
									}
								}
								else if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "relationship")
								{
									row.Add(Relationship.InitializeFromRelationshipJson((JObject)jCol));

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
				DataRow dtRow = dt.NewRow();
				dtRow.ItemArray = row.ToArray();
				dt.Rows.Add(dtRow);
			}

			return dt;
		}
	}
}
