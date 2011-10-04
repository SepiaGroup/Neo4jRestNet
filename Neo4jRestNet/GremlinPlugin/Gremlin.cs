using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Neo4jRestNet.Core;
using System.Net;
using System.Data;

namespace Neo4jRestNet.GremlinPlugin
{
	public class Gremlin
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private static readonly string DefaultGremlinExtensionPath = ConfigurationManager.ConnectionStrings["neo4jGremlinExtension"].ConnectionString.TrimEnd('/');


		public static IEnumerable<T> Post<T>(GEID StartNodeId, string script) where T : IGraphObject
		{
			return Post<T>(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), string.Format("g.v({0}).{1}", (long)StartNodeId, script)); 
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

			Type typeParameterType = typeof(T);

			JObject jo = new JObject();
			jo.Add("script", script);

			string Response;
			HttpStatusCode status = Rest.HttpRest.Post(gremlinUrl, jo.ToString(Formatting.None), out Response);

			if (typeParameterType == typeof(Node))
			{
				return (IEnumerable<T>)Node.ParseJson(Response);
			}
			else if (typeParameterType == typeof(Relationship))
			{
				return (IEnumerable<T>)Relationship.ParseJson(Response);
			}
			else if (typeParameterType == typeof(Path))
			{
				return (IEnumerable<T>)Path.ParseJson(Response);
			}

			throw new Exception("Return type " + typeParameterType.ToString() + " not implemented");
		}

		public static DataTable GetTable(string script)
		{
			return GetTable(string.Concat(DefaultDbUrl, DefaultGremlinExtensionPath), script);
		}

		public static DataTable GetTable(string gremlinUrl, string script) 
		{
			// Remove trailing /
			gremlinUrl = gremlinUrl.TrimEnd('/');

			JObject joScript = new JObject();
			joScript.Add("script", script);

			string Response;
			HttpStatusCode status = Rest.HttpRest.Post(gremlinUrl, joScript.ToString(Formatting.None), out Response);

			JObject joResponse = JObject.Parse(Response);
			JArray jaColumns =(JArray)joResponse["columns"];
			JArray jaData = (JArray)joResponse["data"];
			
			DataTable dt = new DataTable();

			bool InitColumns = true;
			int ColIndex = 0;
			foreach (JArray jRow in jaData)
			{
				List<object> row = new List<object>();
				foreach (JToken jCol in jRow)
				{
					switch (jCol.Type)
					{
						case JTokenType.String:
							row.Add(jCol.ToString());
							if (InitColumns)
							{
								dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(string));
								ColIndex++;
							}
							break;

						case JTokenType.Object:
							if (jCol["self"] == null)
							{
								row.Add(jCol.ToString());

								if (InitColumns)
								{
									dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(string));
									ColIndex++;
								}
							}
							else
							{
								string self = jCol["self"].ToString();
								string[] selfArray = self.Split('/');
								if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node"  )
								{
									row.Add(Node.InitializeFromNodeJson((JObject)jCol));

									if (InitColumns)
									{
										dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(Node));
										ColIndex++;
									}
								}
								else if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "relationship")
								{
									row.Add(Relationship.InitializeFromRelationshipJson((JObject)jCol));

									if (InitColumns)
									{
										dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(Relationship));
										ColIndex++;
									}
								}
								else
								{
									// Not a Node or Relationship - return as string
									row.Add(jCol.ToString());

									if (InitColumns)
									{
										dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(string));
										ColIndex++;
									}
								}
							}
							break;

						case JTokenType.Integer:
							row.Add(jCol.ToString());
							if (InitColumns)
							{
								dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(int));
								ColIndex++;
							}
							break;

						case JTokenType.Float:
							row.Add(jCol.ToString());
							if (InitColumns)
							{
								dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(float));
								ColIndex++;
							}
							break;

						case JTokenType.Date:
							row.Add(jCol.ToString());
							if (InitColumns)
							{
								dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(DateTime));
								ColIndex++;
							}
							break;

						case JTokenType.Boolean:
							row.Add(jCol.ToString());
							if (InitColumns)
							{
								dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(bool));
								ColIndex++;
							}
							break;

						default:
							row.Add(jCol.ToString());

							if (InitColumns)
							{
								dt.Columns.Add(jaColumns[ColIndex].ToString(), typeof(string));
								ColIndex++;
							}
							break;
					}
				}

				InitColumns = false;
				DataRow dtRow = dt.NewRow();
				dtRow.ItemArray = row.ToArray();
				dt.Rows.Add(dtRow);
			}

			return dt;
		}
	}
}
