using System;
using System.Linq.Expressions;
using System.Data;

namespace Neo4jRestNet.CypherPlugin
{
	public interface ICypher
	{
		DataTable Post();
		DataTable Post(string cypherUrl);
		void Start(Func<CypherStart, object> start);
		void Match(Func<CypherMatch, object> match);
		void Where(Expression<Func<CypherWhere, object>> where);
		void Return(Func<CypherReturn, object> cypherReturn);
		void OrderBy(Func<CypherOrderBy, object> cypherOrderBy);
		void Skip(int skip);
		void Limit(int limit);
		string Query { get; }
	}
}
