using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.CypherPlugin
{
	public interface ICypherWhereType
	{
	}

	public class And : ICypherWhereType
	{
	}

	public class Or : ICypherWhereType
	{
	}	
}
