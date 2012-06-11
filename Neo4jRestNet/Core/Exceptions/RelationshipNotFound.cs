using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class RelationshipNotFoundException : Exception
	{
		const string DefaultMessage = "Relationship not found";

		public RelationshipNotFoundException(string auxMessage = null) : 
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}