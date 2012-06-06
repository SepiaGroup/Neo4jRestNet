using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class NodeNotFoundException : Exception
	{
		const string DefaultMessage = "Node not found";

		public NodeNotFoundException(string auxMessage = null) : 
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}