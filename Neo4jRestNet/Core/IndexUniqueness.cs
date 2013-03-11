using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4jRestNet.Core
{
	public struct IndexUniqueness
	{
		public static readonly IndexUniqueness GetOrCreate = new IndexUniqueness("get_or_create");
		public static readonly IndexUniqueness CreateOrFail = new IndexUniqueness("create_or_fail");

		private readonly string _uniqueness;

		private IndexUniqueness(string uniqueness) 
		{
			_uniqueness = uniqueness;
		}

		public static implicit operator string(IndexUniqueness uniqueness)
		{
			return uniqueness._uniqueness;
		}

		public static implicit operator IndexUniqueness(string uniqueness)
		{
			var unique = uniqueness.ToLower();
			if (unique != GetOrCreate || unique != CreateOrFail)
			{
				throw new ArgumentOutOfRangeException("uniqueness", uniqueness, string.Format("The value {0} is not valid for Uniqueness", uniqueness));
			}

			return new IndexUniqueness(unique);
		}

		public override string ToString()
		{
			return _uniqueness;
		}
	}
}
