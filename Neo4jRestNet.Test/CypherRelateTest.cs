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
	public class CypherRelateTest
	{

		[TestMethod]
		public void CreateRelationshipIfMissing()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id).Node("node2", node2.Id, node3.Id));
			cypher.Relate(l => l.Node("node1").To("r", "likes").Node("node2"));

			cypher.Return(r => r.Relationship("r"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 2);
			var r1 = result.Rows[0].Field<Relationship>("r");
			var r2 = result.Rows[1].Field<Relationship>("r");

			Assert.IsTrue(r1.StartNode == node1);
			Assert.IsTrue(r1.EndNode == node2);
			Assert.IsTrue(r2.StartNode == node1);
			Assert.IsTrue(r2.EndNode == node3);
		}

		[TestMethod]
		public void CreateNodeIfMissing()
		{
			var node1 = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Relate(l => l.Node("node1").To("likes").Node("node2"));

			cypher.Return(r => r.Node("node2"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(result.Rows[0].Field<Node>("node2") != null);
		}

		[TestMethod]
		public void CreateNodesWithProperties()
		{
			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);

			var node1 = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Relate(l => l.Node("node1").To("likes").Node("node2", p));

			cypher.Return(r => r.Node("node2"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(result.Rows[0].Field<Node>("node2") != null);
			var n2 = result.Rows[0].Field<Node>("node2");
			Assert.IsTrue(n2.GetProperty<string>("name") == "jack");
			Assert.IsTrue(n2.GetProperty<int>("age") == 12);
		}

		[TestMethod]
		public void CreateRelationshipWithProperties()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			
			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);

			var cypher = new Cypher();
			cypher.Start(s => s.Node("n1", node1.Id).Node("n2", node2.Id));

			cypher.Relate(l => l.Node("n1").To("r", "like", p).Node("n2"));

			cypher.Return(r => r.Relationship("r"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(result.Rows[0].Field<Relationship>("r") != null);
			var rel = result.Rows[0].Field<Relationship>("r");
			Assert.IsTrue(rel.StartNode == node1);
			Assert.IsTrue(rel.EndNode == node2);
			Assert.IsTrue(rel.GetProperty<string>("name") == "jack");
			Assert.IsTrue(rel.GetProperty<int>("age") == 12);
		}
	}
}
