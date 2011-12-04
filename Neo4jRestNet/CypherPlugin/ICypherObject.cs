namespace Neo4jRestNet.CypherPlugin
{
	public interface ICypherObject
	{
		ICypherObject Append(string value);

		ICypherObject Append(string format, params object[] args);
	
	}
}
