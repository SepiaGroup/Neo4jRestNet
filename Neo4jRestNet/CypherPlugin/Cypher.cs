using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Configuration;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using Neo4jRestNet.Core;

namespace Neo4jRestNet.CypherPlugin
{
	public class Cypher
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private static readonly string DefaultCypherExtensionPath = ConfigurationManager.ConnectionStrings["neo4jCypherExtension"].ConnectionString.TrimEnd('/');

		public static DataTable Post(CypherQuery query)
		{
			return Post(string.Concat(DefaultDbUrl, DefaultCypherExtensionPath), query.ToString());
		}
		
		public static DataTable Post(string query)
		{
			return Post(string.Concat(DefaultDbUrl, DefaultCypherExtensionPath), query);
		}

		public static DataTable Post(string gremlinUrl, string query)
		{
			// Remove trailing /
			gremlinUrl = gremlinUrl.TrimEnd('/');

			JObject joScript = new JObject();
			joScript.Add("query", query);

			string Response;
			HttpStatusCode status = Rest.HttpRest.Post(gremlinUrl, joScript.ToString(Formatting.None), out Response);

			JObject joResponse = JObject.Parse(Response);
			JArray jaColumns = (JArray)joResponse["columns"];
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
								if (selfArray.Length > 2 && selfArray[selfArray.Length - 2] == "node")
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
