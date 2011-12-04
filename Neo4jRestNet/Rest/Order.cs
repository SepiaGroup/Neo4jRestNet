using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class Order
    {
		public static readonly Order BreadthFirst = new Order("breadth_first");
		public static readonly Order DepthFirst = new Order("depth_first");

		private readonly string _order;

		private Order(string order)
		{
			_order = order;
		}

        public JProperty ToJson()
        {
            return new JProperty("order", _order);
        }
    }
}