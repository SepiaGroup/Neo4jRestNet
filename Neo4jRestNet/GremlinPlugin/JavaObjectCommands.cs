namespace Neo4jRestNet.GremlinPlugin
{
	public static class JavaObjectCommands
	{
		public static JavaObject GetProperty(this JavaObject javaObject, string name)
		{
			return javaObject.Append("getProperty('{0}')", name);
		}

		public static JavaObject GetColumn(this JavaObject javaObject, string name)
		{
			return javaObject.Append("getColumn('{0}')", name);
		}

		public static JavaString Name(this JavaObject javaObject)
		{
			return new JavaString(javaObject).Append("name");
		}

	}
}
