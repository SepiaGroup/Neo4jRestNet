using System;

namespace Neo4jRestNet.CypherPlugin
{
	public static class CypherObject 
	{
		public static T Property<T>(this T cypherReturn, string name) where T : ICypherObject
		{
			return (T)cypherReturn.Append(".{0}", name);
		}
		
		public static T Property<T>(this T cypherReturn, Enum name) where T : ICypherObject
		{
			return Property(cypherReturn, name.ToString());
		}

		public static T Property<T>(this T cypherReturn, string name, bool optional) where T : ICypherObject
		{
			return (T)cypherReturn.Append(".{0}{1}", name, optional ? "?" : string.Empty);
		}

		public static T Property<T>(this T cypherReturn, Enum name, bool optional) where T : ICypherObject
		{
			return Property(cypherReturn, name.ToString(), optional);
		}

	}
}
