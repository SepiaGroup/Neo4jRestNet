using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.CypherQuery;
using Neo4jRestNet.Core.Exceptions;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Test
{
	[TestClass]
	public class CypherReturnTest
	{

		[TestMethod]
		public void ReturnAllElements()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var rel1 = node1.CreateRelationshipTo(node2, "like");


			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("rel1", "like").Node("node2"));
			cypher.Return(r => r.AllElements());

			var row = cypher.Execute().AsEnumerable().FirstOrDefault();

			Assert.IsNotNull(row);

			var objNode1 = row.Field<object>("node1");
			var objNode2 = row.Field<object>("node2");
			var objRel1 = row.Field<object>("rel1");

			var rowNode1 = RestNodeStore.CreateNodeFromJson((JObject)objNode1);
			var rowNode2 = RestNodeStore.CreateNodeFromJson((JObject)objNode2);
			var rowRel1 = RestRelationshipStore.CreateRelationshipFromJson((JObject)objRel1);

			Assert.IsTrue(node1.Id == rowNode1.Id);
			Assert.IsTrue(node2.Id == rowNode2.Id);
			Assert.IsTrue(rel1.Id == rowRel1.Id);
		}
	}
}
