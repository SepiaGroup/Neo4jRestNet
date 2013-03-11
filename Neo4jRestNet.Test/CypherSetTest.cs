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
	public class CypherSetTest
	{

		[TestMethod]
		public void SetNodeProperty1()
		{
			var p = new Properties();
			p.SetProperty("name", "jack");
			
			var node1 = Node.CreateNode();

			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Set(s => s.Node("node1", p));
			cypher.Return(r => r.Node("node1"));

			var results = cypher.Execute();

			var n1 = results.First().Field<Node>("node1");

			Assert.IsTrue(n1.GetProperty<string>("name") == "jack");
		}

		[TestMethod]
		public void SetNodeProperty2()
		{
			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);


			var node1 = Node.CreateNode();

			var cypher = new Cypher();

			cypher.Start(s => s.Node("node1", node1.Id));
			cypher.Set(s => s.Node("node1", p));
			cypher.Return(r => r.Node("node1"));

			var results = cypher.Execute();

			var n1 = results.First().Field<Node>("node1");

			Assert.IsTrue(n1.GetProperty<string>("name") == "jack");
			Assert.IsTrue(n1.GetProperty<int>("age") == 12);
			Assert.IsTrue(n1.GetProperty<DateTime>("date") == now);
		}

		[TestMethod]
		public void SetRelationshipProperty1()
		{
			var p = new Properties();
			p.SetProperty("name", "jack");

			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var rel = Relationship.CreateRelationship(node1, node2, "like");

			var cypher = new Cypher();

			cypher.Start(s => s.Relationship("rel1", rel.Id));
			cypher.Set(s => s.Relationship("rel1", p));
			cypher.Return(r => r.Relationship("rel1"));

			var results = cypher.Execute();

			var r1 = results.First().Field<Relationship>("rel1");

			Assert.IsTrue(r1.GetProperty<string>("name") == "jack");
		}

		[TestMethod]
		public void SetRelationshipProperty2()
		{
			var now = DateTime.Now;

			var p = new Properties();
			p.SetProperty("name", "jack");
			p.SetProperty("age", 12);
			p.SetProperty("date", now);


			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var rel = Relationship.CreateRelationship(node1, node2, "like");

			var cypher = new Cypher();

			cypher.Start(s => s.Relationship("rel1", rel.Id));
			cypher.Set(s => s.Relationship("rel1", p));
			cypher.Return(r => r.Relationship("rel1"));

			var results = cypher.Execute();

			var r1 = results.First().Field<Relationship>("rel1");

			Assert.IsTrue(r1.GetProperty<string>("name") == "jack"); 
			Assert.IsTrue(r1.GetProperty<int>("age") == 12);
			Assert.IsTrue(r1.GetProperty<DateTime>("date") == now);
		}
	}
}
