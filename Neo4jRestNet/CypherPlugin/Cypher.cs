﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Data;
using Neo4jRestNet.Core.Implementation;
using Neo4jRestNet.Core.Interface;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public class Cypher : ICypher
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private static readonly string DefaultCypherExtensionPath = ConfigurationManager.ConnectionStrings["neo4jCypherExtension"].ConnectionString.TrimEnd('/');

		private readonly List<Func<CypherStart, object>> _start = new List<Func<CypherStart, object>>();
		private readonly List<Func<CypherMatch, object>> _match = new List<Func<CypherMatch, object>>();
		private readonly List<Expression<Func<CypherWhere, object>>> _where = new List<Expression<Func<CypherWhere, object>>>();
		private readonly List<Func<CypherReturn, object>> _return = new List<Func<CypherReturn, object>>();
		private readonly List<Func<CypherOrderBy, object>> _orderBy = new List<Func<CypherOrderBy, object>>();
		private String _skip = string.Empty;
		private String _limit = string.Empty;

		public DataTable Post()
		{
			return Post(string.Concat(DefaultDbUrl, DefaultCypherExtensionPath));
		}

		public DataTable Post(string cypherUrl)
		{
			// Remove trailing /
			cypherUrl = cypherUrl.TrimEnd('/');

			var joScript = new JObject {{"query", Query}};

			string response;
			Rest.HttpRest.Post(cypherUrl, joScript.ToString(Formatting.None), out response);

			var joResponse = JObject.Parse(response);
			var jaColumns = (JArray)joResponse["columns"];
			var jaData = (JArray)joResponse["data"];
			var returnTypes = GetReturnTypes;

			var dt = new DataTable();

			var initColumns = true;
			
			foreach (JArray jRow in jaData)
			{
				var colIndex = 0;
				var row = new List<object>();

				foreach (var jCol in jRow)
				{
					if (initColumns)
					{
						dt.Columns.Add(jaColumns[colIndex].ToString(), returnTypes[colIndex]);
					}

					if (returnTypes[colIndex] == typeof (INode))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : new Node().InitializeFromNodeJson((JObject)jCol));
					}
					else if (returnTypes[colIndex] == typeof (IRelationship))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : new Relationship().InitializeFromRelationshipJson((JObject) jCol));
					}
					else if (returnTypes[colIndex] == typeof (IPath))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : new Path().ParseJson((JArray)jCol));
					}
					else if (returnTypes[colIndex] == typeof(string))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : (string)jCol);
					}
					else if (returnTypes[colIndex] == typeof (int))
					{
						if(jCol.Type == JTokenType.Null)
						{
							throw new ArgumentNullException(string.Format("Value for column {0} of type {1} can not be null", jaColumns[colIndex], returnTypes[colIndex].Name));
						}

						row.Add((int) jCol);
					}
					else if (returnTypes[colIndex] == typeof(int?))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : (int?)jCol);
					}
					else if (returnTypes[colIndex] == typeof(long))
					{
						if (jCol.Type == JTokenType.Null)
						{
							throw new ArgumentNullException(string.Format("Value for column {0} of type {1} can not be null", jaColumns[colIndex], returnTypes[colIndex].Name));
						}

						row.Add((long)jCol);
					}
					else if (returnTypes[colIndex] == typeof(long?))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : (long?)jCol);
					}
					else
					{
						throw new NotSupportedException(string.Format("Return Type of {0} is not supported", returnTypes[colIndex].Name));
					}

					colIndex++;
				}

				initColumns = false;
				var dtRow = dt.NewRow();
				dtRow.ItemArray = row.ToArray();
				dt.Rows.Add(dtRow);
			}

			return dt;
		}

				public void Start(Func<CypherStart, object> start)
		{
			_start.Add(start);
		}

		public void Match(Func<CypherMatch, object> match)
		{
			_match.Add(match);
		}

		public void Where(Expression<Func<CypherWhere, object>> where)
		{
			_where.Add(where);
		}

		public void Return(Func<CypherReturn, object> cypherReturn)
		{
			_return.Add(cypherReturn);
		}

		public void OrderBy(Func<CypherOrderBy, object> cypherOrderBy)
		{
			_orderBy.Add(cypherOrderBy);
		}

		public void Skip(int skip)
		{
			_skip = string.Format(" SKIP {0}", skip);
		}
		
		public void Limit(int limit)
		{
			_limit = string.Format(" LIMIT {0}", limit);
		}

		private ReadOnlyCollection<Type> GetReturnTypes
		{
			get
			{
				var returnTypes = new List<Type>();

				foreach (var r in _return)
				{
					//  call GetReturnTypes somehow
					var obj = (CypherReturn)r.Invoke(new CypherReturn());
					returnTypes.AddRange(obj.GetReturnTypes);
				}

				return returnTypes.AsReadOnly();
			}
		}

		public string Query
		{
			get
			{
				var sbToString = new StringBuilder();

				var label = "START";
				foreach (var s in _start)
				{
					sbToString.AppendFormat("{1}{0}", s.Invoke(new CypherStart()), label);
					label = ",";
				}

				if (_match != null)
				{
					label = "MATCH";
					foreach (var m in _match)
					{
						sbToString.AppendFormat(" {1}{0}", m.Invoke(new CypherMatch()), label);
						label = ",";
					}
				}

				if (_where != null)
				{
					label = "WHERE";
					foreach (var w in _where)
					{
						sbToString.AppendFormat(" {1} {0}", new ParseWhereLambda().Parse(w), label);
						label = string.Empty;
					}
				}

				if (_return != null)
				{
					label = "RETURN";
					foreach (var r in _return)
					{
						sbToString.AppendFormat(" {1}{0}", r.Invoke(new CypherReturn()), label);
						label = ",";
					}
				}

				if (_orderBy != null)
				{
					label = "ORDER BY";
					foreach (var o in _orderBy)
					{
						sbToString.AppendFormat(" {1}{0}", o.Invoke(new CypherOrderBy()), label);
						label = ",";
					}
				}

				// Append Skip
				sbToString.Append(_skip);

				// Append Limit
				sbToString.Append(_limit);
				return sbToString.ToString();
			}
		}
	}
}
