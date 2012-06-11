using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using Neo4jRestNet.Configuration;

namespace Neo4jRestNet.Core.CypherQuery
{
	public class Cypher
	{
		readonly List<Func<CypherStart, object>> _start = new List<Func<CypherStart, object>>();
		readonly List<Func<CypherCreate, object>> _create = new List<Func<CypherCreate, object>>();
		readonly List<Func<CypherRelate, object>> _relate = new List<Func<CypherRelate, object>>();
		readonly List<Func<CypherMatch, object>> _match = new List<Func<CypherMatch, object>>();
		readonly List<Tuple<string, Expression<Func<CypherWhere, object>>>> _where = new List<Tuple<string, Expression<Func<CypherWhere, object>>>>();
		readonly List<Func<CypherReturn, object>> _return = new List<Func<CypherReturn, object>>();
		readonly List<Func<CypherOrderBy, object>> _orderBy = new List<Func<CypherOrderBy, object>>();
		String _skip = string.Empty;
		String _limit = string.Empty;

		public DataTable Execute()
		{
			return Execute(ConnectionManager.Connection());
		}

		public DataTable Execute(ConnectionElement connection)
		{
			
			var joScript = new JObject { { "query", Query } };

			string response;
			Rest.HttpRest.Post(connection.CypherUrl, joScript.ToString(Formatting.None), out response);

			var joResponse = JObject.Parse(response);
			var jaColumns = (JArray)joResponse["columns"];
			var jaData = (JArray)joResponse["data"];
			var returnTypes = GetReturnTypes;

			var dt = new DataTable();

			if(jaData == null)
			{
				return dt;
			}

			var initColumns = true;

			foreach (JArray jRow in jaData)
			{
				var colIndex = 0;
				var row = new List<object>();

				foreach (var jCol in jRow)
				{
					if (initColumns)
					{
						// NOTE: DataTable does NOT support nullable data types
						dt.Columns.Add(jaColumns[colIndex].ToString(), returnTypes[colIndex]);
					}

					if (returnTypes[colIndex] == typeof(Node))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : RestNodeStore.CreateNodeFromJson((JObject)jCol));
					}
					else if (returnTypes[colIndex] == typeof(Relationship))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : RestRelationshipStore.CreateRelationshipFromJson((JObject)jCol));
					}
					else if (returnTypes[colIndex] == typeof(Path))
					{
						row.Add(jCol.Type == JTokenType.Null ? null : Path.ParseJson((JArray)jCol));
					}
	
					// Handle the Null case for all Types
					else if (jCol.Type == JTokenType.Null)
					{
						row.Add(DBNull.Value);
					}

					// Explicitly cast to correct data type
					else if (returnTypes[colIndex] == typeof(DateTime))
					{
						row.Add(jCol.Value<DateTime>());
					}			
					else if (returnTypes[colIndex] == typeof(double))
					{
						row.Add(jCol.Value<double>());
					}
					else if (returnTypes[colIndex] == typeof(int))
					{
						row.Add(jCol.Value<int>());
					}
					else if (returnTypes[colIndex] == typeof(long))
					{
						row.Add(jCol.Value<long>());
					}
					else if (returnTypes[colIndex] == typeof(string))
					{
						row.Add(jCol.Value<string>());
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

		public void Create(Func<CypherCreate, object> create)
		{
			_create.Add(create);
		}

		public void Relate(Func<CypherRelate, object> relate)
		{
			_relate.Add(relate);
		}

		public void Match(Func<CypherMatch, object> match)
		{
			_match.Add(match);
		}

		public void Where(Expression<Func<CypherWhere, object>> where)
		{
			_where.Add(new Tuple<string, Expression<Func<CypherWhere, object>>>(" and ", where));
		}

		public void OrWhere(Expression<Func<CypherWhere, object>> where) 
		{
			_where.Add(new Tuple<string, Expression<Func<CypherWhere, object>>>(" or ", where));
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
				var leftParen = string.Empty;
				var rightParen = string.Empty;

				string label;

				if (_create != null)
				{
					label = "START";
					foreach (var s in _start)
					{
						sbToString.AppendFormat("{1}{0}", s.Invoke(new CypherStart()), label);
						label = ",";
					}
				}

				if (_create != null)
				{
					label = "CREATE";
					foreach (var c in _create)
					{
						sbToString.AppendFormat("{1}{0}", c.Invoke(new CypherCreate()), label);
						label = ",";
					}
				}

				if (_relate != null)
				{
					label = "RELATE";
					foreach (var r in _relate)
					{
						sbToString.AppendFormat("{1}{0}", r.Invoke(new CypherRelate()), label);
						label = ",";
					}
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
					leftParen = string.Empty;
					rightParen = string.Empty;

					foreach (var w in _where)
					{
						sbToString.AppendFormat(" {1} {2}{0}{3}", new ParseWhereLambda().Parse(w.Item2), label == string.Empty ?  w.Item1 : label, leftParen, rightParen);
						label = string.Empty;
						leftParen = "(";
						rightParen = ")";
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
