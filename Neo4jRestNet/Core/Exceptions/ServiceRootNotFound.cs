using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class ServiceRootNotFound : Exception
	{
		const string DefaultMessage = "Service root not found";

		public ServiceRootNotFound(string auxMessage = null) : 
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}