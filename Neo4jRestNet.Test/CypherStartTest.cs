using System;
using System.Linq;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.CypherQuery;

namespace Neo4jRestNet.Test
{
	[TestClass]
	public class CypherStartTest
	{
		private string UniqueValue()
		{
			return System.Web.Security.Membership.GeneratePassword(5, 0);
		}

		[TestMethod]
		public void GetNodeById()
		{
			var node = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node", node.Id));
			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);

			Assert.IsTrue(node == result.AsEnumerable().First().Field<Node>("node"));
		}

		[TestMethod]
		public void GetRelationshipById()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var rel = node1.CreateRelationshipTo(node2, "like");

			var cypher = new Cypher();
			cypher.Start(s => s.Relationship("relationship", rel.Id));
			cypher.Return(r => r.Relationship("relationship"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);

			Assert.IsTrue(rel == result.AsEnumerable().First().Field<Relationship>("relationship"));
		}

		[TestMethod]
		public void GetMultipleNodesById()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node", node1.Id, node2.Id, node3.Id));
			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 3);

			Assert.IsTrue(node1 == result.Rows[0].Field<Node>("node"));
			Assert.IsTrue(node2 == result.Rows[1].Field<Node>("node"));
			Assert.IsTrue(node3 == result.Rows[2].Field<Node>("node"));
		}

		//[TestMethod]
		//public void GetAllNodes()
		//{
		//    var node1 = Node.CreateNode();

		//    var cypher = new Cypher();
		//    cypher.Start(s => s.AllNodes("allNodes"));
		//    cypher.Return(r => r.Node("allNodes"));

		//    var result = cypher.Post();

		//    Assert.IsTrue(result.Rows.Count > 1);
		//}

		[TestMethod]
		public void GetNodeByIndexLookup()
		{
			var node1 = Node.CreateNode();

			var value1 = UniqueValue();

			node1.AddToIndex("nodes", "name", value1);

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node", "nodes", "name", value1));
			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);

			Assert.IsTrue(node1 == result.Rows[0].Field<Node>("node"));
		}

		[TestMethod]
		public void GetRelationshipByIndexLookup()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var rel = node1.CreateRelationshipTo(node2, "like");

			var value1 = UniqueValue();

			rel.AddToIndex("relationships", "name", value1);

			var cypher = new Cypher();
			cypher.Start(s => s.Relationship("rel", "relationships", "name", value1));
			cypher.Return(r => r.Relationship("rel"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);

			Assert.IsTrue(rel == result.Rows[0].Field<Relationship>("rel"));
		}

		[TestMethod]
		public void GetNodeByIndexQuery()
		{
			var node1 = Node.CreateNode();

			var value1 = DateTime.Now.Millisecond;

			node1.AddToIndex("nodes", "name", value1);

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node", "nodes", string.Format("name:{0}", value1)));
			cypher.Return(r => r.Node("node"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);

			Assert.IsTrue(node1 == result.Rows[0].Field<Node>("node"));
		}

		[TestMethod]
		public void GetRelationshipByIndexQuery()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var rel = node1.CreateRelationshipTo(node2, "like");

			var value1 = DateTime.Now.Millisecond;

			rel.AddToIndex("relationships", "name", value1);

			var cypher = new Cypher();
			cypher.Start(s => s.Relationship("rel", "relationships", string.Format("name:{0}", value1)));
			cypher.Return(r => r.Relationship("rel"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);

			Assert.IsTrue(rel == result.Rows[0].Field<Relationship>("rel"));
		}

		[TestMethod]
		public void MultipleNodeStartingPoints1()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("n1", node1.Id).Node("n2", node2.Id));
			cypher.Return(r => r.Node("n1").Node("n2"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(node1 == result.Rows[0].Field<Node>("n1"));
			Assert.IsTrue(node2 == result.Rows[0].Field<Node>("n2")); 
		}

		[TestMethod]
		public void MultipleNodeStartingPoints2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var cypher = new Cypher();
			cypher.Start(s => s.Node("n1", node1.Id));
			cypher.Start(s => s.Node("n2", node2.Id));

			cypher.Return(r => r.Node("n1"));
			cypher.Return(r => r.Node("n2"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(node1 == result.Rows[0].Field<Node>("n1"));
			Assert.IsTrue(node2 == result.Rows[0].Field<Node>("n2"));
		}

		[TestMethod]
		public void MultipleRelationshipStartingPoints1()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node3, "like");
			var rel2 = node2.CreateRelationshipTo(node3, "like");

			var cypher = new Cypher();
			cypher.Start(s => s.Relationship("r1", rel1.Id).Relationship("r2", rel2.Id));
			cypher.Return(r => r.Relationship("r1").Relationship("r2"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(rel1 == result.Rows[0].Field<Relationship>("r1"));
			Assert.IsTrue(rel2 == result.Rows[0].Field<Relationship>("r2"));
		}

		[TestMethod]
		public void MultipleRelationshipStartingPoints2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node3, "like");
			var rel2 = node2.CreateRelationshipTo(node3, "like");

			var cypher = new Cypher();
			cypher.Start(s => s.Relationship("r1", rel1.Id));
			cypher.Start(s => s.Relationship("r2", rel2.Id));

			cypher.Return(r => r.Relationship("r1"));
			cypher.Return(r => r.Relationship("r2"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Rows.Count == 1);
			Assert.IsTrue(rel1 == result.Rows[0].Field<Relationship>("r1"));
			Assert.IsTrue(rel2 == result.Rows[0].Field<Relationship>("r2"));
		}
	}
}
