using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class BatchGetRelationshipsNotSupportedException : Exception
	{
		const string DefaultMessage = "GetRelationships on a batch node is not supported";

		public BatchGetRelationshipsNotSupportedException(string auxMessage = null) :
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}