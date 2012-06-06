using System;
using System.Net;
using System.Data;
using System.Collections.Generic;
using Neo4jRestNet.Configuration;
using Newtonsoft.Json.Linq;
using Neo4jRestNet.Core;


namespace Neo4jRestNet.GremlinPlugin
{
	public static class Gremlin
	{
		public static HttpStatusCode Post(GremlinScript script)
		{
			string response;
			var status = Rest.HttpRest.Post(ConnectionManager.Connection().GremlinUrl, script.GetScript(), out response);

			return status;
		}

		public static IEnumerable<T> Post<T>(GremlinScript script) where T : IGraphObject
		{
			return Post<T>(ConnectionManager.Connection(), script);
		}

		public static IEnumerable<T> Post<T>(ConnectionElement connection, GremlinScript script) where T : IGraphObject
		{
			var typeParameterType = typeof(T);

			string response;
			var status = Rest.HttpRest.Post(connection.GremlinUrl, script.GetScript(), out response);

			if (typeParameterType == typeof(Node))
			{
				return (IEnumerable<T>)RestNodeStore.ParseNodeJson(response);
			}

			if (typeParameterType == typeof(Relationship))
			{
				return (IEnumerable<T>)RestRelationshipStore.ParseRelationshipJson(response);
			}

			if (typeParameterType == typeof(Path))
			{
				return (IEnumerable<T>)Path.ParseJson(response);
			}

			throw new Exception("Return type " + typeParameterType + " not implemented");


		}

		public static DataTable GetTable(GremlinScript script)
		{
			return GetTable(ConnectionManager.Connection(), script);
		}

		public static DataTable GetTable(ConnectionElement connection, GremlinScript script)
		{
			string response;
			var status = Rest.HttpRest.Post(connection.GremlinUrl, script.GetScript(), out response);

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
									row.Add(RestNodeStore.CreateNodeFromJson((JObject)jCol));

									if (initColumns)
									{
										dt.Columns.Add(jaColumns[colIndex].ToString(), typeof(Node));
										colIndex++;
									}
								}
								else if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "relationship")
								{
									row.Add(RestRelationshipStore.CreateRelationshipFromJson((JObject)jCol));

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
