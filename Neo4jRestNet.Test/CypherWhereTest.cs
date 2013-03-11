using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.CypherQuery;

namespace Neo4jRestNet.Test
{
	[TestClass]
	public class CypherWhereTest
	{
		[TestMethod]
		public void OperatorInTest()
		{
			var p1 = new Properties();
			p1.SetProperty("name", "jack");
			p1.SetProperty("count", 3);

			var p2 = new Properties();
			p2.SetProperty("name", "jill");
			p2.SetProperty("count", 3);

			var node1 = Node.CreateNode(p1);
			var node2 = Node.CreateNode(p1);
			var node3 = Node.CreateNode(p2);

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node", node1.Id, node2.Id, node3.Id));
			cypher.Where(w => w.Node("node").Property("name").In("jack"));
			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 2);
			Assert.IsTrue(result.First().Field<Node>("node") == node1);
			Assert.IsTrue(result.ElementAt(1).Field<Node>("node") == node2);
		}


		[TestMethod]
		public void OperatorHasTest1()
		{
			var p1 = new Properties();
			p1.SetProperty("name", "jack");

			var p2 = new Properties();
			p2.SetProperty("name", "jill");
			p2.SetProperty("count", 3);

			var node1 = Node.CreateNode(p1);
			var node2 = Node.CreateNode(p1);
			var node3 = Node.CreateNode(p2);

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node", node1.Id, node2.Id, node3.Id));
			cypher.Where(w => w.NodeHas("node", "count"));
			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("node") == node3);
		}

		[TestMethod]
		public void OperatorHasTest2()
		{
			var p1 = new Properties();
			p1.SetProperty("name", "jack");

			var p2 = new Properties();
			p2.SetProperty("name", "jill");
			p2.SetProperty("count", 3);

			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "like", p1);
			var rel2 = node1.CreateRelationshipTo(node3, "like", p2);

			var cypher = new Cypher();
			cypher.Start(s => s.Relationship("rel", rel1.Id	, rel2.Id));
			cypher.Where(w => w.RelationshipHas("rel", "count"));
			cypher.Return(r => r.Relationship("rel"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Relationship>("rel") == rel2);
		}
	}
}
