using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class BatchDeleteNotSupportedException : Exception
	{
		const string DefaultMessage = "Deleting of a Node/Relationship created within a batch is not supported";

		public BatchDeleteNotSupportedException(string auxMessage = null) :
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}