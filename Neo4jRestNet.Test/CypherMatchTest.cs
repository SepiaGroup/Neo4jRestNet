using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.CypherQuery;


namespace Neo4jRestNet.Test
{
	[TestClass]
	public class CypherMatchTest
	{

		private string UniqueValue()
		{
			return System.Web.Security.Membership.GeneratePassword(5, 0);
		}

		[TestMethod]
		public void RelatedNodes()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "like");
			var rel2 = node1.CreateRelationshipTo(node3, "like");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").Any().Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 2);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node2);
			Assert.IsTrue(result.ElementAt(1).Field<Node>("matchedNode") == node3);
		}

		[TestMethod]
		public void OutgoingRelationships()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "like");
			var rel2 = node3.CreateRelationshipTo(node1, "like");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To().Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node2);
		}

		[TestMethod]
		public void IncomingRelationships()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "like");
			var rel2 = node3.CreateRelationshipTo(node1, "like");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").From().Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node3);
		}

		[TestMethod]
		public void DirectedRelationshipsAndIdentifier()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("r", string.Empty).Node());
			cypher.Return(r => r.Relationship("r"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 2);
			Assert.IsTrue(result.First().Field<Relationship>("r") == rel1);
			Assert.IsTrue(result.ElementAt(1).Field<Relationship>("r") == rel2);
		}

		[TestMethod]
		public void MatchByRelationshipType1()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").Any("blocks").Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node3);
		}

		[TestMethod]
		public void MatchByRelationshipType2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("blocks").Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node3);
		}

		[TestMethod]
		public void MatchByRelationshipType3()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node2.CreateRelationshipTo(node1, "knows");
			var rel2 = node3.CreateRelationshipTo(node1, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").From("blocks").Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node3);
		}
		
		[TestMethod]
		public void MatchByMultipleRelationshipType1()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").Any(new List<string> { "knows", "blocks" }).Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 2);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node2);
			Assert.IsTrue(result.ElementAt(1).Field<Node>("matchedNode") == node3);
		}

		[TestMethod]
		public void MatchByMultipleRelationshipType2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To( new List<string>{ "knows", "blocks" } ).Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 2);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node2);
			Assert.IsTrue(result.ElementAt(1).Field<Node>("matchedNode") == node3);
		}

		[TestMethod]
		public void MatchByMultipleRelationshipType3()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node2.CreateRelationshipTo(node1, "knows");
			var rel2 = node3.CreateRelationshipTo(node1, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").From(new List<string> { "knows", "blocks" }).Node("matchedNode"));
			cypher.Return(r => r.Node("matchedNode"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 2);
			Assert.IsTrue(result.First().Field<Node>("matchedNode") == node2);
			Assert.IsTrue(result.ElementAt(1).Field<Node>("matchedNode") == node3);
		}

		[TestMethod]
		public void MatchByRelationshipTypeUseIdentifer1()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").Any("r1", "knows").Node("matchedNode"));
			cypher.Return(r => r.Relationship("r1"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Relationship>("r1") == rel1);
		}

		[TestMethod]
		public void MatchByRelationshipTypeUseIdentifer2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("r1", "knows").Node("matchedNode"));
			cypher.Return(r => r.Relationship("r1"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Relationship>("r1") == rel1);
		}

		[TestMethod]
		public void MatchByRelationshipTypeUseIdentifer3()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node2.CreateRelationshipTo(node1, "knows");
			var rel2 = node1.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").From("r1", "knows").Node("matchedNode"));
			cypher.Return(r => r.Relationship("r1"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Relationship>("r1") == rel1);
		}

		[TestMethod]
		public void MultipleRelatinship1()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node2.CreateRelationshipTo(node3, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("knows").Node("node2").To("blocks").Node("node3"));
			cypher.Return(r => r.Node("node1").Node("node2").Node("node3"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("node1") == node1);
			Assert.IsTrue(result.First().Field<Node>("node2") == node2);
			Assert.IsTrue(result.First().Field<Node>("node3") == node3);
		}

		[TestMethod]
		public void MultipleRelatinship2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node3.CreateRelationshipTo(node2, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("knows").Node("node2").From("blocks").Node("node3"));
			cypher.Return(r => r.Node("node1").Node("node2").Node("node3"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("node1") == node1);
			Assert.IsTrue(result.First().Field<Node>("node2") == node2);
			Assert.IsTrue(result.First().Field<Node>("node3") == node3);
		}

		[TestMethod]
		public void MultipleRelatinship3()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();

			var rel1 = node1.CreateRelationshipTo(node2, "knows");
			var rel2 = node3.CreateRelationshipTo(node2, "blocks");

			var cypher = new Cypher();
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("knows").Node("node2").Any("blocks").Node("node3"));
			cypher.Return(r => r.Node("node1").Node("node2").Node("node3"));

			var result = cypher.Execute();

			Assert.IsTrue(result.Count() == 1);
			Assert.IsTrue(result.First().Field<Node>("node1") == node1);
			Assert.IsTrue(result.First().Field<Node>("node2") == node2);
			Assert.IsTrue(result.First().Field<Node>("node3") == node3);
		}
	}
}
