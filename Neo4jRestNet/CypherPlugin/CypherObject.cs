namespace Neo4jRestNet.CypherPlugin
{
	public static class CypherObject 
	{
		public static T Property<T>(this T cypherReturn, string name) where T : ICypherObject
		{
			return (T)cypherReturn.Append(".{0}", name);
		}

		public static T Property<T>(this T cypherReturn, string name, bool optional) where T : ICypherObject
		{
			return (T)cypherReturn.Append(".{0}{1}", name, optional ? "?" : string.Empty);
		}
	}
}
