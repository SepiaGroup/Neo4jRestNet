namespace Neo4jRestNet.Cypher
{
	public interface ICypherObject
	{
		ICypherObject Append(string value);

		ICypherObject Append(string format, params object[] args);
	
	}
}
