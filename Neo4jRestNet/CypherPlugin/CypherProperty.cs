using System;

namespace Neo4jRestNet.CypherPlugin
{
	public static class CypherProperty
	{
		public static CypherWhere Property(this CypherWhere cypherWhere, string name)
		{
			return (CypherWhere)cypherWhere.Append(".{0}", name);
		}

		public static CypherWhere Property(this CypherWhere cypherWhere, Enum name)
		{
			return Property(cypherWhere, name.ToString());
		}

		public static CypherWhere Property(this CypherWhere cypherWhere, string name, bool optional) 
		{
			return (CypherWhere)cypherWhere.Append(".{0}{1}", name, optional ? "?" : string.Empty);
		}

		public static CypherWhere Property(this CypherWhere cypherWhere, Enum name, bool optional) 
		{
			return Property(cypherWhere, name.ToString(), optional);
		}

		public static CypherReturn Property<T>(this CypherReturn cypherReturn, string name) 
		{
			return Property<T>(cypherReturn, name, false);
		}

		public static CypherReturn Property<T>(this CypherReturn cypherReturn, Enum name)
		{
			return Property<T>(cypherReturn, name.ToString(), false);
		}

		public static CypherReturn Property<T>(this CypherReturn cypherReturn, Enum name, bool optional)
		{
			return Property<T>(cypherReturn, name.ToString(), optional);
		}
		
		public static CypherReturn Property<T>(this CypherReturn cypherReturn, string name, bool optional)
		{
			// Remove the Previous type of Node or Relationship
			cypherReturn.ReturnTypes.RemoveAt(cypherReturn.ReturnTypes.Count - 1);

			// Add the correct Property Return Type
			cypherReturn.ReturnTypes.Add(typeof(T));

			return (CypherReturn)cypherReturn.Append(".{0}{1}", name, optional ? "?" : string.Empty);
		}
	}
}
