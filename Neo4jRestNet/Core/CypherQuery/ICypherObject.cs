namespace Neo4jRestNet.Core.CypherQuery
{
	public interface ICypherObject
	{
		ICypherObject Append(string value);

		ICypherObject Append(string format, params object[] args);
	
	}
}
