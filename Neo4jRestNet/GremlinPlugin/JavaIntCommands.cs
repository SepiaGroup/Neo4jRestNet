using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.GremlinPlugin
{
	public static class JavaIntCommands
	{
		public static JavaInt Id(this JavaObject javaObject)
		{
			return new JavaInt(javaObject).Append("id");
		}
	}
}
