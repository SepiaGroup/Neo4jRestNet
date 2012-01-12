using System;
using System.Configuration;
using System.Linq.Expressions;
using System.Data;

namespace Neo4jRestNet.CypherPlugin
{
	public class Cypher : ICypher
	{
		private static readonly string DefaultDbUrl = ConfigurationManager.ConnectionStrings["neo4j"].ConnectionString.TrimEnd('/');
		private static readonly string DefaultCypherExtensionPath = ConfigurationManager.ConnectionStrings["neo4jCypherExtension"].ConnectionString.TrimEnd('/');
		private static readonly string CypherProvider = ConfigurationManager.ConnectionStrings["neo4jCypherExtension"].ProviderName;
		private readonly ICypher _cypher;
		
		public Cypher()
		{
			switch (CypherProvider)
			{
				case "":
				case "neo4j" :
					_cypher = new CypherNeo4j();
					break;

				case "inmemory":
					throw new NotImplementedException();

				default:
					throw new NotImplementedException();
			}
		}

		public DataTable Post()
		{
			return _cypher.Post(string.Concat(DefaultDbUrl, DefaultCypherExtensionPath));
		}

		public DataTable Post(string cypherUrl)
		{
			return _cypher.Post(cypherUrl);
		}

		public void Start(Func<CypherStart, object> start)
		{
			_cypher.Start(start);
		}

		public void Match(Func<CypherMatch, object> match)
		{
			_cypher.Match(match);
		}

		public void Where(Expression<Func<CypherWhere, object>> where)
		{
			_cypher.Where(where);
		}

		public void Return(Func<CypherReturn, object> cypherReturn)
		{
			_cypher.Return(cypherReturn);
		}

		public void OrderBy(Func<CypherOrderBy, object> cypherOrderBy)
		{
			_cypher.OrderBy(cypherOrderBy);
		}

		public void Skip(int skip)
		{
			_cypher.Skip(skip);
		}
		
		public void Limit(int limit)
		{
			_cypher.Limit(limit);
		}

		public string Query
		{
			get { return _cypher.Query; }
		}
	}
}
