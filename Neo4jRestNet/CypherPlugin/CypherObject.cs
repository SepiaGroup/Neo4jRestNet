using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public static class CypherObject 
	{
		public static T Property<T>(this T cypherReturn, string Name) where T : ICypherObject
		{
			return (T)cypherReturn.Append(".{0}", Name);
		}
	}
}
