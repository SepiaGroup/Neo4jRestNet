using System.Collections.Generic;
using Neo4jRestNet.Core;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class Order
    {
		public static readonly Order BreadthFirst = new Order("breadth_first");
		public static readonly Order DepthFirst = new Order("depth_first");

		private string _Order;

		private Order(string Order)
		{
			_Order = Order;
		}

        public JProperty ToJson()
        {
            return new JProperty("order", _Order);
        }
    }
}