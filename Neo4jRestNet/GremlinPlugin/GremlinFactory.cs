using System;

namespace Neo4jRestNet.GremlinPlugin
{
	public class GremlinFactory
	{
		public static IGremlin CreateGremlin()
		{
			return new Gremlin();
		}

		public static IGremlin CreateGremlin<TGremlin>(params object[] args) where TGremlin : class, IGremlin, new()
		{
			return (TGremlin)Activator.CreateInstance(typeof(TGremlin), args);
		}
	}
}
