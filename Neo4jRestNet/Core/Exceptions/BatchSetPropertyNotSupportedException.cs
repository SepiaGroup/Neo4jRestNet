using System;

namespace Neo4jRestNet.Core.Exceptions
{
	public class BatchSetPropertyNotSupportedException : Exception
	{
		const string DefaultMessage = "Settomg of a Node/Relationship property within a batch is not supported";

		public BatchSetPropertyNotSupportedException(string auxMessage = null) :
			base(string.Format("{0}{1}", DefaultMessage, string.IsNullOrEmpty(auxMessage) ? string.Empty : string.Format(" - {0}", auxMessage)))
		{
		}
	}
}