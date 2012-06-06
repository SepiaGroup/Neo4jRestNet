using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class BatchRemoveFromIndexNotSupportedException : Exception
	{
		const string DefaultMessage = "Removing a Node/Relationship created within a batch from an index is not supported";

		public BatchRemoveFromIndexNotSupportedException(string auxMessage = null) :
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}