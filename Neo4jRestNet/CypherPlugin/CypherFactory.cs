using System;
using Neo4jRestNet.GremlinPlugin;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherFactory
	{
		public static ICypher CreateCypher()
		{
			return new Cypher();
		}

		public static ICypher CreateCypher<TCypher>(params object[] args) where TCypher : class, ICypher, new()
		{
			return (TCypher)Activator.CreateInstance(typeof(TCypher), args);
		}
	}
}
