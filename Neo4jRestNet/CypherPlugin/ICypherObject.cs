using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public interface ICypherObject
	{
		ICypherObject Append(string value);

		ICypherObject Append(string Format, params object[] args);
	
	}
}
