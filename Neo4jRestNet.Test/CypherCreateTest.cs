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
	public class CypherCreateTest
	{
		[TestMethod]
		public void CreateNode()
		{
			var cypher = new Cypher();
			cypher.Create(c => c.Node("node"));

			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(result.Rows[0].Field<Node>("node") != null);
		}

		[TestMethod]
		public void CreateNodeWithProperties()
		{
			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);

			var cypher = new Cypher();
	
			cypher.Create(c => c.Node("node", p));

			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(result.Rows[0].Field<Node>("node") != null);
			var n = result.Rows[0].Field<Node>("node");
			Assert.IsTrue(n.GetProperty<string>("name") == "jack");
			Assert.IsTrue(n.GetProperty<int>("age") == 12);
		}

		[TestMethod]
		public void CreateRelationship()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("n1", node1.Id).Node("n2", node2.Id));

			cypher.Create(c => c.Node("n1").To("r", "like").Node("n2"));

			cypher.Return(r => r.Relationship("r"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(result.Rows[0].Field<Relationship>("r") != null);
			var rel = result.Rows[0].Field<Relationship>("r");
			Assert.IsTrue(rel.StartNode == node1);
			Assert.IsTrue(rel.EndNode == node2);
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

			cypher.Create(c => c.Node("n1").To("r", "like", p).Node("n2"));

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
