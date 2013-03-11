using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.Exceptions;

namespace Neo4jRestNet.Test
{
	[TestClass]
	public class NodeClassTest
	{
		private string UniqueValue()
		{
			return System.Web.Security.Membership.GeneratePassword(5, 0);
		}

        [TestMethod]
        public void CreateNode()
        {
            var node = Node.CreateNode();
            Assert.IsNotNull(node);
        }

		[TestMethod]
		public void CreateNodeWithNodeType()
		{
			var prop = new Properties();
			prop.SetProperty("NodeType", "myNodeType");

			var node = Node.CreateNode(prop);
			Assert.IsNotNull(node);
			Assert.IsTrue(node.Properties.GetProperty<string>("NodeType") == "myNodeType");
		}

		[TestMethod]
		public void CreateNodeWithPropertyDictionary()
		{
			var properties = new Dictionary<string, object>();
			properties.Add("A", 0);
			properties.Add("B", "Foobar");
			properties.Add("C", new DateTime(2012, 1, 1));
			var node = Node.CreateNode(new Properties(properties));
			Assert.IsNotNull(node);
			Assert.IsTrue(node.Properties.GetProperty("A").Equals(0));
			Assert.IsTrue(node.Properties.GetProperty("B").Equals("Foobar"));
			Assert.IsTrue(node.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
			Assert.IsTrue(node.Properties.ToDictionary().Count == 3);
		}

        [TestMethod]
        public void CreateNodeWithProperties()
        {
            var properties = new Properties();
            properties.SetProperty("A", 0);
            properties.SetProperty("B", "Foobar");
            properties.SetProperty("C", new DateTime(2012, 1, 1));
			var node = Node.CreateNode(properties);
            Assert.IsNotNull(node);
            Assert.IsTrue(node.Properties.GetProperty("A").Equals(0));
            Assert.IsTrue(node.Properties.GetProperty("B").Equals("Foobar"));
			Assert.IsTrue(node.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
            Assert.IsTrue(node.Properties.ToDictionary().Count == 3);
        }

        [TestMethod]
        public void GetNode()
        {
			var node1 = Node.CreateNode();
			var node2 = Node.GetNode(node1.Id);
            Assert.IsNotNull(node2);
            Assert.IsTrue(node1.Equals(node2));
        }

        [TestMethod]
        public void GetNodeWithPropertyDictionary()
        {
			var dicProperties = new Dictionary<string, object>();
            dicProperties.Add("A", 0);
			dicProperties.Add("B", "Foobar");
			dicProperties.Add("C", new DateTime(2012, 1, 1));

			var properties = new Properties(dicProperties);

			var node1 = Node.CreateNode(properties);
            Assert.IsNotNull(node1);
            Assert.IsTrue(node1.Properties.GetProperty("A").Equals(0));
            Assert.IsTrue(node1.Properties.GetProperty("B").Equals("Foobar"));
			Assert.IsTrue(node1.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
            Assert.IsTrue(node1.Properties.ToDictionary().Count == 3);

			var node2 = Node.GetNode(node1.Id);
            Assert.IsNotNull(node2);
            Assert.IsTrue(node1.Equals(node2));
            Assert.IsTrue(node2.Properties.GetProperty("A").Equals(0));
            Assert.IsTrue(node2.Properties.GetProperty("B").Equals("Foobar"));
            Assert.IsTrue(node2.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
            Assert.IsTrue(node2.Properties.ToDictionary().Count == 3);
        }

        [TestMethod]
        public void GetRootNode()
        {
            var node1 = Node.GetRootNode();
            Assert.IsNotNull(node1);
        }

		[TestMethod]
		[ExpectedException(typeof(NodeNotFoundException))]
		public void DeleteNode()
		{
			var node1 = Node.CreateNode();
			node1.Delete();
			Node.GetNode(node1.Id); // Should throw NodeNotFoundException
		}

		[TestMethod]
		public void AddNodeToIndex()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var value1 = UniqueValue();
			var value2 = UniqueValue();
			var value3 = UniqueValue();
			
			var node3 = Node.AddToIndex(node1, "nodes", "a", value1);
			Assert.IsNotNull(node3);
			Assert.IsTrue(node3.Equals(node1));

			var nodes = Node.GetNode("nodes", "a", value1);
			Assert.IsTrue(nodes.Count() == 1);
			Assert.IsTrue(nodes.First() == (node3));

			var node4 = Node.AddToIndex(node1, "nodes", "b", value2);
			Assert.IsNotNull(node4);
			Assert.IsTrue(node4 == node1);

			var node5 = Node.GetNode("nodes", "b", value2).FirstOrDefault();
			Assert.IsNotNull(node5);
			Assert.IsTrue(node5 == node1);

			var node6 = Node.AddToIndex(node1, "nodes", "b", value3);
			Assert.IsNotNull(node6);
			Assert.IsTrue(node6 == node1);

			nodes = Node.GetNode("nodes", "b", value3);
			Assert.IsTrue(nodes.Count() == 1);
			Assert.IsTrue(nodes.First() == node1);

			var node9 = Node.AddToIndex(node1, "nodes", "a", value1);
			var node10 = Node.AddToIndex(node2, "nodes", "a", value1);
			Assert.IsNotNull(node9);
			Assert.IsTrue(node9.Equals(node1));
			Assert.IsNotNull(node10);
			Assert.IsTrue(node10.Equals(node2));

			nodes = Node.GetNode("nodes", "a", value1);
			Assert.IsNotNull(nodes);

			Assert.IsTrue(nodes.Count() == 2);
			if (nodes.First() == node1)
			{
				Assert.IsTrue(nodes.First().Equals(node1));
				Assert.IsTrue(nodes.ElementAt(1).Equals(node2));
			}
			else
			{
				Assert.IsTrue(nodes.First().Equals(node2));
				Assert.IsTrue(nodes.ElementAt(1).Equals(node1));
			}
		}

		[TestMethod]
		public void RemoveNodeFromIndex()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var value1 = UniqueValue();
			var value2 = UniqueValue();
			
			var nodeA = Node.AddToIndex(node1, "nodes", "a", value1);
			Node.AddToIndex(node2, "nodes", "a", value1);
			
			var result1 = Node.RemoveFromIndex(node1, "nodes");
			Assert.IsTrue(result1);
			
			var nodes = Node.GetNode("nodes", "a", value1);
			Assert.IsTrue(nodes.Count() == 1);
			Assert.IsTrue(nodes.First().Equals(node2));

			Node.AddToIndex(node1, "nodes", "b", value1);
			Node.AddToIndex(node1, "nodes", "b", value2);
			Node.AddToIndex(node2, "nodes", "c", value2);
			var result2 = Node.RemoveFromIndex(node1, "nodes", "b", value1);

			Assert.IsTrue(result2);
			Assert.IsTrue(Node.GetNode("nodes", "b", value2).Count() == 1);

			var nodesB = Node.GetNode("nodes", "b", value2);
			Assert.IsTrue(nodesB.Count() == 1);
			Assert.IsTrue(nodesB.First().Equals(node1));

			var nodesC = Node.GetNode("nodes", "c", value2);
			Assert.IsTrue(nodesC.Count() == 1);
			Assert.IsTrue(nodesC.First().Equals(node2));
		}

        [TestMethod]
        public void CreateRelationshipTo()
        {
            var node1 = Node.CreateNode();
            var node2 = Node.CreateNode();
            var relationship = node1.CreateRelationshipTo(node2, "Test");
            Assert.IsNotNull(relationship);
			Assert.IsTrue(relationship.StartNode.Equals(node1));
			Assert.IsTrue(relationship.EndNode.Equals(node2));
			Assert.IsTrue(relationship.Type == "Test");
        }

        [TestMethod]
        public void CreateRelationshipToWithProperties()
        {
            var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
            var properties = new Properties();
            properties.SetProperty("A", 0);
            properties.SetProperty("B", "Foobar");
            properties.SetProperty("C", new DateTime(2012, 1, 1));
            var relationship = node1.CreateRelationshipTo(node2, "Test", properties);
            Assert.IsNotNull(relationship);
			Assert.IsTrue(relationship.StartNode.Equals(node1));
			Assert.IsTrue(relationship.EndNode.Equals(node2));
			Assert.IsTrue(relationship.Type == "Test");
            Assert.IsTrue(relationship.Properties.GetProperty("A").Equals(0));
            Assert.IsTrue(relationship.Properties.GetProperty("B").Equals("Foobar"));
			Assert.IsTrue(relationship.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
            Assert.IsTrue(relationship.Properties.ToDictionary().Count == 3);
        }

		[TestMethod]
		public void GetRelationship()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var relationship1 = node1.CreateRelationshipTo(node2, "Test");
			var relationship2 = Relationship.GetRelationship(relationship1.Id);
			Assert.IsNotNull(relationship2);
			Assert.IsTrue(relationship2.StartNode.Equals(node1));
			Assert.IsTrue(relationship2.EndNode.Equals(node2));
			Assert.IsTrue(relationship2.Type == "Test");
		}

		[TestMethod]
		public void GetRelationshipWithProperties()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var properties = new Properties();
			properties.SetProperty("A", 0);
			properties.SetProperty("B", "Foobar");
			properties.SetProperty("C", new DateTime(2012, 1, 1));
			var relationship1 = node1.CreateRelationshipTo(node2, "Test", properties);
			var relationship2 = Relationship.GetRelationship(relationship1.Id);
			Assert.IsNotNull(relationship2);
			Assert.IsTrue(relationship2.StartNode.Equals(node1));
			Assert.IsTrue(relationship2.EndNode.Equals(node2));
			Assert.IsTrue(relationship2.Type == "Test");
			Assert.IsTrue(relationship2.Properties.GetProperty("A").Equals(0));
			Assert.IsTrue(relationship2.Properties.GetProperty("B").Equals("Foobar"));
			Assert.IsTrue(relationship2.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
			Assert.IsTrue(relationship2.Properties.ToDictionary().Count == 3);
		}

		[TestMethod]
		public void GetRelationships()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var relationship1 = node1.CreateRelationshipTo(node2, "Test");
			var relationships = node1.GetRelationships("Test");
			Assert.IsNotNull(relationships);
			Assert.IsTrue(relationships.Count() == 1);
			Assert.IsTrue(relationships.First().Equals(relationship1));
		}

		[TestMethod]
		public void GetRelationships2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();
			var relationship1 = node1.CreateRelationshipTo(node2, "Test");
			var relationship2 = node2.CreateRelationshipTo(node3, "Test");
			var relationships1 = node1.GetRelationships("Test");
			Assert.IsNotNull(relationships1);
			Assert.IsTrue(relationships1.Count() == 1);
			Assert.IsTrue(relationships1.First().Equals(relationship1));

			var relationships2 = node2.GetRelationships("Test");
			Assert.IsNotNull(relationships2);
			Assert.IsTrue(relationships2.Count() == 2);

			if (relationships2.First().StartNode == node1)
			{
				Assert.IsTrue(relationships2.First().Equals(relationship1));
				Assert.IsTrue(relationships2.ElementAt(1).Equals(relationship2));
			}
			else
			{
				Assert.IsTrue(relationships2.First().Equals(relationship2));
				Assert.IsTrue(relationships2.ElementAt(1).Equals(relationship1));
			}

			var relationships3 = node3.GetRelationships("Test");
			Assert.IsNotNull(relationships3);
			Assert.IsTrue(relationships3.Count() == 1);
			Assert.IsTrue(relationships3.First().Equals(relationship2));
		}

		[TestMethod]
		public void GetRelationships3()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();
			var relationship1 = node1.CreateRelationshipTo(node2, "Test");
			var relationship2 = node1.CreateRelationshipTo(node3, "Test");

			var relationships1 = node1.GetRelationships("Test");
			Assert.IsNotNull(relationships1);
			Assert.IsTrue(relationships1.Count() == 2);

			if (relationships1.First().EndNode == node2)
			{
				Assert.IsTrue(relationships1.First().Equals(relationship1));
				Assert.IsTrue(relationships1.ElementAt(1).Equals(relationship2));
			}
			else
			{
				Assert.IsTrue(relationships1.First().Equals(relationship2));
				Assert.IsTrue(relationships1.ElementAt(1).Equals(relationship1));
			}

			var relationships2 = node2.GetRelationships("Test");
			Assert.IsNotNull(relationships2);
			Assert.IsTrue(relationships2.Count() == 1);
			Assert.IsTrue(relationships2.First().Equals(relationship1));

			var relationships3 = node3.GetRelationships("Test");
			Assert.IsNotNull(relationships3);
			Assert.IsTrue(relationships3.Count() == 1);
			Assert.IsTrue(relationships3.First().Equals(relationship2));
		}

		[TestMethod]
		public void CreateUniqueNode()
		{
			var value = string.Concat("michael-", DateTime.Now.Millisecond);
			var node1 = Node.CreateUniqueNode("people", "name", value, IndexUniqueness.CreateOrFail);
			Assert.IsNotNull(node1);

			var node2 = Node.CreateUniqueNode("people", "name", value, IndexUniqueness.GetOrCreate);
			Assert.IsNull(node2);
		}

		[TestMethod]
		public void AddNodeToIndexUnique()
		{

			var node1 = Node.CreateNode();

			var node2 = Node.AddToIndex(node1, "people", "id", node1.Id, true);
			Assert.IsNotNull(node2);

			var node3 = node1.AddToIndex("people", "id", node1.Id, true);

			Assert.IsNull(node3);

			var node4 = node1.AddToIndex("people", "id", node1.Id);

			Assert.IsNotNull(node4);
		}


		[TestMethod]
		public void CreateUniqueRelationship()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var value = string.Concat("good-", DateTime.Now.Millisecond);

			var relationship1 = Relationship.CreateUniqueRelationship(node1, node2, "knows", "rels", "friends", value, IndexUniqueness.CreateOrFail);
			Assert.IsNotNull(relationship1);

			var relationship2 = Relationship.CreateUniqueRelationship(node1, node2, "knows", "rels", "friends", value, IndexUniqueness.GetOrCreate);
			Assert.IsNull(relationship2);
		}

		[TestMethod]
		public void AddRelationshipToIndexUnique()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var relationship1 = Relationship.CreateRelationship(node1, node2, "knows");

			var relationship2 = relationship1.AddToIndex("rels", "id", relationship1.Id, true);
			Assert.IsNotNull(relationship2);

			var relationship3 = relationship1.AddToIndex("rels", "id", relationship1.Id, true);

			Assert.IsNull(relationship3);

			var relationship4 = relationship1.AddToIndex("rels", "id", relationship1.Id);

			Assert.IsNotNull(relationship4);
		}

		[TestMethod]
		public void GetRelationships4()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();
			var relationship1 = Relationship.CreateRelationship(node1, node2, "a");
			var relationship2 = Relationship.CreateRelationship(node1, node3, "b");

			var relationships1 = node1.GetRelationships("a");
			Assert.IsNotNull(relationships1);
			Assert.IsTrue(relationships1.Count() == 1);
			Assert.IsTrue(relationships1.First().Equals(relationship1));

			var relationships2 = node1.GetRelationships("b");
			Assert.IsNotNull(relationships2);
			Assert.IsTrue(relationships2.Count() == 1);
			Assert.IsTrue(relationships2.First().Equals(relationship2));

			var relationships3 = node3.GetRelationships("a");
			Assert.IsNotNull(relationships3);
			Assert.IsTrue(relationships3.Count() == 0);
		}

		//[TestMethod]
		//public void GetRelationships5()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "a");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node3.Id, "b");

		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, new List<string> {"a", "b"});
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 2);
		//    Assert.IsTrue(relationships1.First().Equals(relationship1));
		//    Assert.IsTrue(relationships1.ElementAt(1).Equals(relationship2));

		//    var relationships2 = _nodeGraphStore.GetRelationships(node2.Id, new List<string> { "a", "b" });
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship1));

		//    var relationships3 = _nodeGraphStore.GetRelationships(node3.Id, new List<string> { "a" });
		//    Assert.IsNotNull(relationships3);
		//    Assert.IsTrue(relationships3.Count() == 0);
		//}

		//[TestMethod]
		//public void GetRelationshipsOut()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 1);
		//    Assert.IsTrue(relationships1.First().Equals(relationship1));

		//    var relationships2 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 0);
		//}

		//[TestMethod]
		//public void GetRelationshipsOut2()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node2.Id, node3.Id, "Test");
		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 1);
		//    Assert.IsTrue(relationships1.First().Equals(relationship1));

		//    var relationships2 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship2));
            
		//    var relationships3 = _nodeGraphStore.GetRelationships(node3.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships3);
		//    Assert.IsTrue(relationships3.Count() == 0);
		//}

		//[TestMethod]
		//public void GetRelationshipsOut3()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node3.Id, "Test");

		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 2);
		//    Assert.IsTrue(relationships1.First().Equals(relationship1));
		//    Assert.IsTrue(relationships1.ElementAt(1).Equals(relationship2));

		//    var relationships2 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 0);

		//    var relationships3 = _nodeGraphStore.GetRelationships(node3.Id, RelationshipDirection.Out);
		//    Assert.IsNotNull(relationships3);
		//    Assert.IsTrue(relationships3.Count() == 0);
		//}

		//[TestMethod]
		//public void GetRelationshipsOut4()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "a");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node3.Id, "b");

		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.Out, "a");
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 1);
		//    Assert.IsTrue(relationships1.First().Equals(relationship1));

		//    var relationships2 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.Out, "b");
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship2));

		//    var relationships3 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.Out, "b");
		//    Assert.IsNotNull(relationships3);
		//    Assert.IsTrue(relationships3.Count() == 0);
		//}

		//[TestMethod]
		//public void GetRelationshipsOut5()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "a");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node3.Id, "b");

		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.Out, new List<string> { "a", "b" });
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 2);
		//    Assert.IsTrue(relationships1.First().Equals(relationship1));
		//    Assert.IsTrue(relationships1.ElementAt(1).Equals(relationship2));

		//    var relationships2 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.Out, new List<string> { "b" });
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship2));

		//    var relationships3 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.Out, new List<string> { "a" });
		//    Assert.IsNotNull(relationships3);
		//    Assert.IsTrue(relationships3.Count() == 0);
		//}

		//[TestMethod]
		//public void GetRelationshipsIn()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 0);

		//    var relationships2 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship1));
		//}

		//[TestMethod]
		//public void GetRelationshipsIn2()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node2.Id, node3.Id, "Test");
		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 0);

		//    var relationships2 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship1));

		//    var relationships3 = _nodeGraphStore.GetRelationships(node3.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships3);
		//    Assert.IsTrue(relationships3.Count() == 1);
		//    Assert.IsTrue(relationships3.First().Equals(relationship2));
		//}

		//[TestMethod]
		//public void GetRelationshipsIn3()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node3.Id, "Test");

		//    var relationships1 = _nodeGraphStore.GetRelationships(node1.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships1);
		//    Assert.IsTrue(relationships1.Count() == 0);

		//    var relationships2 = _nodeGraphStore.GetRelationships(node2.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships2);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship1));

		//    var relationships3 = _nodeGraphStore.GetRelationships(node3.Id, RelationshipDirection.In);
		//    Assert.IsNotNull(relationships3);
		//    Assert.IsTrue(relationships3.Count() == 1);
		//    Assert.IsTrue(relationships3.First().Equals(relationship2));
		//}

		//[TestMethod]
		//[ExpectedException(typeof(RelationshipNotFoundException))]
		//public void DeleteRelationship()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var result = _nodeGraphStore.DeleteRelationship(relationship1.Id);
		//    Assert.IsTrue(result);
		//    _nodeGraphStore.GetRelationship(relationship1.Id); // Should throw RelationshipNotFoundException
		//}

		//[TestMethod]
		//public void AddRelationshipToIndex()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");

		//    var relationship2 = _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "a", "foobar");
		//    Assert.IsTrue(relationship1.Equals(relationship2));

		//    var relationship3 = _nodeGraphStore.GetRelationships("relationships", "a", "foobar").Last();
		//    Assert.IsTrue(relationship1.Equals(relationship3));

		//    var relationship4 = _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "b", 42);
		//    Assert.IsTrue(relationship1.Equals(relationship4));

		//    var relationship5 = _nodeGraphStore.GetRelationships("relationships", "b", 42).Last();
		//    Assert.IsTrue(relationship1.Equals(relationship5));

		//    var relationship6 = _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "c", new DateTime(2012, 1, 1));
		//    Assert.IsTrue(relationship1.Equals(relationship6));

		//    var relationship7 = _nodeGraphStore.GetRelationships("relationships", "c", new DateTime(2012, 1, 1)).Last();
		//    Assert.IsTrue(relationship1.Equals(relationship7));
		//}

		//[TestMethod]
		//public void RemoveRelationshipFromIndex()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();
		//    var node3 = _nodeGraphStore.CreateNode();
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "Test");
		//    var relationship2 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node3.Id, "Test");

		//    _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "a", "foobar");
		//    _nodeGraphStore.AddRelationshipToIndex(relationship2.Id, "relationships2", "a", "foobar");
		//    var result1 = _nodeGraphStore.RemoveRelationshipFromIndex(relationship1.Id, "relationships");
		//    Assert.IsTrue(result1);
		//    Assert.IsTrue(_nodeGraphStore.GetRelationships("relationships", "a", "foobar").Count() == 0);
		//    var relationships = _nodeGraphStore.GetRelationships("relationships2", "a", "foobar");
		//    Assert.IsTrue(relationships.Count() == 1);
		//    Assert.IsTrue(relationships.First().Equals(relationship2));

		//    _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "a", "foobar");
		//    _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "a", 42);
		//    _nodeGraphStore.AddRelationshipToIndex(relationship2.Id, "relationships", "b", 42);
		//    var result2 = _nodeGraphStore.RemoveRelationshipFromIndex(relationship1.Id, "relationships", "a");
		//    Assert.IsTrue(result2);
		//    Assert.IsTrue(_nodeGraphStore.GetRelationships("relationships", "a", "foobar").Count() == 0);
		//    Assert.IsTrue(_nodeGraphStore.GetRelationships("relationships", "a", 42).Count() == 0);
		//    var relationships1 = _nodeGraphStore.GetRelationships("relationships", "b", 42);
		//    Assert.IsTrue(relationships1.Count() == 1);
		//    Assert.IsTrue(relationships1.First().Equals(relationship2));

		//    _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "a", "foobar");
		//    _nodeGraphStore.AddRelationshipToIndex(relationship1.Id, "relationships", "a", 42);
		//    _nodeGraphStore.AddRelationshipToIndex(relationship2.Id, "relationships", "b", 42);
		//    var result3 = _nodeGraphStore.RemoveRelationshipFromIndex(relationship1.Id, "relationships", "a", "foobar");
		//    Assert.IsTrue(result3);
		//    Assert.IsTrue(_nodeGraphStore.GetRelationships("relationships", "a", "foobar").Count() == 0);
		//    var relationships2 = _nodeGraphStore.GetRelationships("relationships", "a", 42);
		//    Assert.IsTrue(relationships2.Count() == 1);
		//    Assert.IsTrue(relationships2.First().Equals(relationship1));
		//    var relationships3 = _nodeGraphStore.GetRelationships("relationships", "b", 42);
		//    Assert.IsTrue(relationships3.Count() == 1);
		//    Assert.IsTrue(relationships3.First().Equals(relationship2));
		//}

		//[TestMethod]
		//public void LoadNodeProperties()
		//{
		//    var properties1 = new Dictionary<string, object>();
		//    properties1.Add("A", 0);
		//    properties1.Add("B", "Foobar");
		//    properties1.Add("C", new DateTime(2012, 1, 1));
		//    var node1 = Node.CreateNode(new Properties(properties1));

		//    var properties2 = _nodeGraphStore.LoadNodeProperties(node1.Id);
		//    Assert.IsNotNull(properties2);
		//    Assert.IsTrue(properties2.GetProperty("A").Equals(0));
		//    Assert.IsTrue(properties2.GetProperty("B").Equals("Foobar"));
		//    Assert.IsTrue(properties2.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
		//    Assert.IsTrue(properties2.ToDictionary().Count == 3);
		//}


		//[TestMethod]
		//[ExpectedException(typeof(NodeNotFoundException))]
		//public void LoadNodePropertiesNotFound()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    _nodeGraphStore.LoadNodeProperties(node1.Id + 1); // Should throw exception
		//}

        [TestMethod]
        public void SaveNodeProperties()
        {
            var properties1 = new Dictionary<string, object>();
            properties1.Add("A", 0);
            properties1.Add("B", "Foobar");
            properties1.Add("C", new DateTime(2012, 1, 1));
            var node1 = Node.CreateNode();
            node1.SaveProperties(new Properties(properties1));

            var node2 = Node.GetNode(node1.Id);
            Assert.IsTrue(node2.Properties.GetProperty("A").Equals(0));
            Assert.IsTrue(node2.Properties.GetProperty("B").Equals("Foobar"));
			Assert.IsTrue(node2.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
            Assert.IsTrue(node2.Properties.ToDictionary().Count == 3);
        }

		//[TestMethod]
		//[ExpectedException(typeof(NodeNotFoundException))]
		//public void SaveNodePropertiesNotFound()
		//{
		//    var properties1 = new Dictionary<string, object>();
		//    properties1.Add("A", 0);
		//    properties1.Add("B", "Foobar");
		//    properties1.Add("C", new DateTime(2012, 1, 1));
		//    var node1 = _nodeGraphStore.CreateNode();
		//    _nodeGraphStore.SaveNodeProperties(node1.Id + 1, new Properties(properties1)); // Should throw exception
		//}

		//[TestMethod]
		//public void LoadRelationshipProperties()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();

		//    var properties1 = new Dictionary<string, object>();
		//    properties1.Add("A", 0);
		//    properties1.Add("B", "Foobar");
		//    properties1.Add("C", new DateTime(2012, 1, 1));
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "test", new Properties(properties1));

		//    var properties2 = _nodeGraphStore.LoadRelationshipProperties(relationship1.Id);
		//    Assert.IsNotNull(properties2);
		//    Assert.IsTrue(properties2.GetProperty("A").Equals(0));
		//    Assert.IsTrue(properties2.GetProperty("B").Equals("Foobar"));
		//    Assert.IsTrue(properties2.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
		//    Assert.IsTrue(properties2.ToDictionary().Count == 3);
		//}

		//[TestMethod]
		//public void SaveRelationshipProperties()
		//{
		//    var node1 = _nodeGraphStore.CreateNode();
		//    var node2 = _nodeGraphStore.CreateNode();

		//    var properties1 = new Dictionary<string, object>();
		//    properties1.Add("A", 0);
		//    properties1.Add("B", "Foobar");
		//    properties1.Add("C", new DateTime(2012, 1, 1));
		//    var relationship1 = _nodeGraphStore.CreateRelationshipTo(node1.Id, node2.Id, "test");
		//    _nodeGraphStore.SaveRelationshipProperties(relationship1.Id, new Properties(properties1));

		//    var relationship2 = _nodeGraphStore.GetRelationship(relationship1.Id);
		//    Assert.IsTrue(relationship2.Properties.GetProperty("A").Equals(0));
		//    Assert.IsTrue(relationship2.Properties.GetProperty("B").Equals("Foobar"));
		//    Assert.IsTrue(relationship2.Properties.GetProperty("C").Equals(new DateTime(2012, 1, 1).ToString("s")));
		//    Assert.IsTrue(relationship2.Properties.ToDictionary().Count == 3);
		//}
    }
}
