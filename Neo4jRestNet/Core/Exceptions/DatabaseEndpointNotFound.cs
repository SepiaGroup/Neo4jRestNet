using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class DatabaseEndpointNotFound : Exception
	{
		const string DefaultMessage = "Database endpoint not found";

		public DatabaseEndpointNotFound(string auxMessage = null) : 
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}