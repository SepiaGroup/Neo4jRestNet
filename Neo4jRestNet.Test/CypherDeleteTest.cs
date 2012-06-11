using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.CypherQuery;
using Neo4jRestNet.Core.Exceptions;

namespace Neo4jRestNet.Test
{
	[TestClass]
	public class CypherDeleteTest
	{

		[TestMethod]
		[ExpectedException(typeof(NodeNotFoundException))]
		public void DeleteSingleNode()
		{
			var node1 = Node.CreateNode();

			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Delete(d => d.Node("node1"));

			cypher.Execute();

			Node.GetNode(node1.Id);
		}

		[TestMethod]
		[ExpectedException(typeof(RelationshipNotFoundException))]
		public void DeleteSingleRelationship()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var rel = node1.CreateRelationshipTo(node2, "like");

			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("r", "like").Node("node2"));
			cypher.Delete(d => d.Relationship("r"));

			cypher.Execute();

			Relationship.GetRelationship(rel.Id);
		}

		[TestMethod]
		public void DeleteNodeAndRelationship()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var rel = node1.CreateRelationshipTo(node2, "like");

			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Match(m => m.Node("node1").To("r", "like").Node("node2"));
			cypher.Delete(d => d.Node("node2").Relationship("r"));

			cypher.Execute();

			try
			{
				Node.GetNode(node2.Id);

				Assert.Fail();
			}
			catch (NodeNotFoundException e)
			{

			}


			try
			{
				Relationship.GetRelationship(rel.Id);
				
				Assert.Fail();
			}
			catch(RelationshipNotFoundException e)
			{
				
			}
		}

		[TestMethod]
		public void DeleteSingleNodeProperty()
		{
			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);

			var node1 = Node.CreateNode(p);

			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Delete(d => d.Node("node1", "age"));

			cypher.Execute();

			var n1 = Node.GetNode(node1.Id);

			Assert.IsTrue(n1.GetPropertyOrOther("age", -1) == -1);

			Assert.IsTrue(n1.GetProperty<string>("name") == "jack");
			Assert.IsTrue(n1.GetProperty<DateTime>("date") == now);
		}

		[TestMethod]
		public void DeleteMultipleNodeProperties1()
		{
			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);

			var node1 = Node.CreateNode(p);

			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Delete(d => d.Node("node1", "age").Node("node1", "name"));
			cypher.Return(r => r.Node("node1"));

			var results = cypher.Execute();

			var n1 = Node.GetNode(node1.Id);

			Assert.IsTrue(n1 == results.Rows[0].Field<Node>("node1"));

			Assert.IsTrue(n1.GetPropertyOrOther("age", -1) == -1);
			Assert.IsTrue(n1.GetPropertyOrOther("name", "deleted") == "deleted");

			Assert.IsTrue(n1.GetProperty<DateTime>("date") == now);
		}

		[TestMethod]
		public void DeleteMultipleNodeProperties2()
		{
			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);

			var node1 = Node.CreateNode(p);

			var cypher = new Cypher();
			
			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Delete(d => d.Node("node1", new List<string>{"age", "name"}));
			cypher.Return(r => r.Node("node1"));

			var results = cypher.Execute();

			var n1 = Node.GetNode(node1.Id);

			Assert.IsTrue(n1 == results.Rows[0].Field<Node>("node1"));

			Assert.IsTrue(n1.GetPropertyOrOther("age", -1) == -1);
			Assert.IsTrue(n1.GetPropertyOrOther("name", "deleted") == "deleted");

			Assert.IsTrue(n1.GetProperty<DateTime>("date") == now);
		}

		[TestMethod]
		public void DeleteSingleRelationshipProperty()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();


			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);

			var rel1 = Relationship.CreateRelationship(node1, node2, "like", p);

			var cypher = new Cypher();

			cypher.Start(s => s.Relationship("rel1", rel1.Id));
			cypher.Delete(d => d.Relationship("rel1", "age"));

			cypher.Execute();

			var r1 = Relationship.GetRelationship(rel1.Id);

			Assert.IsTrue(r1.GetPropertyOrOther("age", -1) == -1);

			Assert.IsTrue(r1.GetProperty<string>("name") == "jack");
			Assert.IsTrue(r1.GetProperty<DateTime>("date") == now);
		}

		[TestMethod]
		public void DeleteMultipleRelationshipProperties1()
		{

			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);

			var rel1 = Relationship.CreateRelationship(node1, node2, "like", p); 

			var cypher = new Cypher();

			cypher.Start(s => s.Relationship("rel1", rel1.Id));
			cypher.Delete(d => d.Relationship("rel1", "age").Node("rel1", "name"));
			cypher.Return(r => r.Relationship("rel1"));

			var results = cypher.Execute();

			var r1 = Relationship.GetRelationship(rel1.Id);

			Assert.IsTrue(r1 == results.Rows[0].Field<Relationship>("rel1"));

			Assert.IsTrue(r1.GetPropertyOrOther("age", -1) == -1);
			Assert.IsTrue(r1.GetPropertyOrOther("name", "deleted") == "deleted");
						  
			Assert.IsTrue(r1.GetProperty<DateTime>("date") == now);
		}

		[TestMethod]
		public void DeleteMultipleRelationshipProperties2()
		{

			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);

			var rel1 = Relationship.CreateRelationship(node1, node2, "like", p); 

			var cypher = new Cypher();

			cypher.Start(s => s.Relationship("rel1", rel1.Id));
			cypher.Delete(d => d.Relationship("rel1", new List<string>{ "age", "name"}));
			cypher.Return(r => r.Relationship("rel1"));

			var results = cypher.Execute();

			var r1 = Relationship.GetRelationship(rel1.Id);

			Assert.IsTrue(r1 == results.Rows[0].Field<Relationship>("rel1"));

			Assert.IsTrue(r1.GetPropertyOrOther("age", -1) == -1);
			Assert.IsTrue(r1.GetPropertyOrOther("name", "deleted") == "deleted");

			Assert.IsTrue(r1.GetProperty<DateTime>("date") == now);
		}

	}
}
