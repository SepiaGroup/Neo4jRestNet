using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Test
{
	[TestClass]
	public class BatchStoreTest
	{
		private string UniqueValue()
		{
			return System.Web.Security.Membership.GeneratePassword(5, 0);
		}

		[TestMethod]
		public void CreateNodeNoProperties()
		{
			var batch = new BatchStore();
			var batchNode = Node.CreateNode(null, batch);

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			Assert.IsTrue(restNode.Id > 0);
		}

		[TestMethod]
		public void CreateTwoNodeNoProperties()
		{
			var batch = new BatchStore();
			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);

			Assert.IsTrue(batch.Execute());

			var restNode1 = batch.GetGraphObject(batchNode1);
			var restNode2 = batch.GetGraphObject(batchNode2);

			Assert.IsTrue(restNode1.Id > 0);
			Assert.IsTrue(restNode2.Id > 0 && restNode1.Id != restNode2.Id);
		}

		[TestMethod]
		public void CreateNodeWithProperties()
		{
			var batch = new BatchStore();

			var prop1 = new Properties();
			prop1.SetProperty("name", "michael");
			prop1.SetProperty("location", "new york");

			var prop2 = new Properties();
			prop2.SetProperty("color", "blue");
			prop2.SetProperty("tree", "elm");

			var batchNode1 = Node.CreateNode(prop1, batch);
			var batchNode2 = Node.CreateNode(prop2, batch);

			Assert.IsTrue(batch.Execute());

			var restNode1 = batch.GetGraphObject(batchNode1);
			var restNode2 = batch.GetGraphObject(batchNode2);

			Assert.IsTrue(restNode1.Id > 0);
			Assert.IsTrue(restNode2.Id > 0 && restNode1.Id != restNode2.Id);

			Assert.IsTrue(restNode1.Properties.GetProperty<string>("name") == "michael");
			Assert.IsTrue(restNode1.Properties.GetProperty<string>("location") == "new york");

			Assert.IsTrue(restNode2.Properties.GetProperty<string>("color") == "blue");
			Assert.IsTrue(restNode2.Properties.GetProperty<string>("tree") == "elm");
		}

		[TestMethod]
		public void GetNode()
		{
			var node = Node.CreateNode();

			var batch = new BatchStore();
			var batchNode = Node.GetNode(node.Id, batch);

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			Assert.IsTrue(node == restNode);
		}

		[TestMethod]
		[ExpectedException(typeof(NodeNotFoundException))]
		public void DeleteNode()
		{
			var node = Node.CreateNode();

			var batch = new BatchStore();

			batch.Delete(node);

			Assert.IsTrue(batch.Execute());

			Node.GetNode(node.Id);
		}

		[TestMethod]
		[ExpectedException(typeof(BatchDeleteNotSupportedException))]
		public void DeleteBatchNode1()
		{
			var batch = new BatchStore();
			var node = Node.CreateNode(batch);

			node.Delete();

			batch.Execute();
		}

		[TestMethod]
		[ExpectedException(typeof(BatchDeleteNotSupportedException))]
		public void DeleteBatchNode2()
		{
			var batch = new BatchStore();
			var node = Node.CreateNode(batch);

			batch.Delete(node);
			batch.Execute();
		}

		[TestMethod]
		public void CreateRelationshipNoProperties1()
		{
			var batch = new BatchStore();
			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);

			var batchRelationship = batchNode1.CreateRelationshipTo(batchNode2, "friend");

			Assert.IsTrue(batch.Execute());

			var restNode1 = batch.GetGraphObject(batchNode1);
			var restNode2 = batch.GetGraphObject(batchNode2);
			var restRelationship = batch.GetGraphObject(batchRelationship);

			Assert.IsTrue(restNode1.Id > 0);
			Assert.IsTrue(restNode2.Id > 0 && restNode1.Id != restNode2.Id);

			Assert.IsTrue(restRelationship.StartNode == restNode1);
			Assert.IsTrue(restRelationship.EndNode == restNode2);
			Assert.IsTrue(restRelationship.Type == "friend");
		}

		[TestMethod]
		public void CreateRelationshipNoProperties2()
		{
			var batch = new BatchStore();
			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);

			var batchRelationship = Relationship.CreateRelationship(batchNode1, batchNode2, "friend", null, batch);

			Assert.IsTrue(batch.Execute());

			var restNode1 = batch.GetGraphObject(batchNode1);
			var restNode2 = batch.GetGraphObject(batchNode2);
			var restRelationship = batch.GetGraphObject(batchRelationship);

			Assert.IsTrue(restNode1.Id > 0);
			Assert.IsTrue(restNode2.Id > 0 && restNode1.Id != restNode2.Id);

			Assert.IsTrue(restRelationship.StartNode == restNode1);
			Assert.IsTrue(restRelationship.EndNode == restNode2);
			Assert.IsTrue(restRelationship.Type == "friend");
		}

		[TestMethod]
		public void CreateRelationshipWithProperties1()
		{
			var batch = new BatchStore();
			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);

			var prop = new Properties();

			var since = DateTime.Now;

			prop.SetProperty("since", since);
			prop.SetProperty("approved", true);

			var batchRelationship = batchNode1.CreateRelationshipTo(batchNode2, "friend", prop);

			Assert.IsTrue(batch.Execute());

			var restNode1 = batch.GetGraphObject(batchNode1);
			var restNode2 = batch.GetGraphObject(batchNode2);
			var restRelationship = batch.GetGraphObject(batchRelationship);

			Assert.IsTrue(restNode1.Id > 0);
			Assert.IsTrue(restNode2.Id > 0 && restNode1.Id != restNode2.Id);

			Assert.IsTrue(restRelationship.StartNode == restNode1);
			Assert.IsTrue(restRelationship.EndNode == restNode2);
			Assert.IsTrue(restRelationship.Type == "friend");

			Assert.IsTrue(restRelationship.Properties.GetProperty<DateTime>("since") == since);
			Assert.IsTrue(restRelationship.Properties.GetProperty<bool>("approved"));
		}

		[TestMethod]
		public void CreateRelationshipWithProperties2()
		{
			var batch = new BatchStore();
			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);

			var prop = new Properties();

			var since = DateTime.Now;

			prop.SetProperty("since", since);
			prop.SetProperty("approved", true);

			var batchRelationship = Relationship.CreateRelationship(batchNode1, batchNode2, "friend", prop, batch);

			Assert.IsTrue(batch.Execute());

			var restNode1 = batch.GetGraphObject(batchNode1);
			var restNode2 = batch.GetGraphObject(batchNode2);
			var restRelationship = batch.GetGraphObject(batchRelationship);

			Assert.IsTrue(restNode1.Id > 0);
			Assert.IsTrue(restNode2.Id > 0 && restNode1.Id != restNode2.Id);

			Assert.IsTrue(restRelationship.StartNode == restNode1);
			Assert.IsTrue(restRelationship.EndNode == restNode2);
			Assert.IsTrue(restRelationship.Type == "friend");

			Assert.IsTrue(restRelationship.Properties.GetProperty<DateTime>("since") == since);
			Assert.IsTrue(restRelationship.Properties.GetProperty<bool>("approved"));
		}


		[TestMethod]
		public void GetRelationshipWithProperties()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var props = new Properties();
			props.SetProperty("name", "michael");
			props.SetProperty("approved", true);
			props.SetProperty("length", 123);

			var relationship = node1.CreateRelationshipTo(node2, "friend", props);

			var batch = new BatchStore();
			var batchRelationship = Relationship.GetRelationship(relationship.Id, batch);

			Assert.IsTrue(batch.Execute());

			var restRelationship = batch.GetGraphObject(batchRelationship);

			Assert.IsTrue(restRelationship == relationship);
			Assert.IsTrue(restRelationship.Properties.GetProperty<string>("name") == "michael");
			Assert.IsTrue(restRelationship.Properties.GetProperty<bool>("approved"));
			Assert.IsTrue(restRelationship.Properties.GetProperty<int>("length") == 123);
		}

		[TestMethod]
		public void UpdateRelationshipProperties1()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var batch = new BatchStore();

			var batchRelationship = Relationship.CreateRelationship(node1, node2, "likes", null, batch);

			batchRelationship.Properties.SetProperty("name", "todd");
			batchRelationship.Properties.SetProperty("moved", true);
			batchRelationship.Properties.SetProperty("height", 13);

			batchRelationship.SaveProperties();

			Assert.IsTrue(batch.Execute());

			var restRelationship = batch.GetGraphObject(batchRelationship);

			Assert.IsTrue(restRelationship.Properties.GetProperty<string>("name") == "todd");
			Assert.IsTrue(restRelationship.Properties.GetProperty<bool>("moved"));
			Assert.IsTrue(restRelationship.Properties.GetProperty<int>("height") == 13);
		}

		[TestMethod]
		public void RemoveRelationshipProperties()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var batch = new BatchStore();

			var batchRelationship = Relationship.CreateRelationship(node1, node2, "likes", null, batch);
			batchRelationship.Properties.SetProperty("name", "joe");
			batchRelationship.Properties.SetProperty("age", 12);

			batchRelationship.SaveProperties();

			batchRelationship.Properties.RemoveProperty("age");

			batchRelationship.SaveProperties();

			Assert.IsTrue(batch.Execute());

			var restRelationship = batch.GetGraphObject(batchRelationship);

			Assert.IsTrue(restRelationship.Properties.GetProperty<string>("name") == "joe");
			Assert.IsTrue(restRelationship.Properties.GetPropertyOrDefault<int>("age") == 0);
		}


		[TestMethod]
		[ExpectedException(typeof(BatchGetRelationshipsNotSupportedException))]
		public void GetAllRelationships1()
		{
			var batch = new BatchStore();

			var batchNode = Node.CreateNode(batch);
			batchNode.GetRelationships();

			batch.Execute();
		}

		[TestMethod]
		public void GetAllRelationships2()
		{
			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();
			var node3 = Node.CreateNode();
			var node4 = Node.CreateNode();

			var restRelationship1 = node1.CreateRelationshipTo(node2, "like");
			var restRelationship2 = node1.CreateRelationshipTo(node3, "like");
			var restRelationship3 = node1.CreateRelationshipTo(node4, "like");

			var batch = new BatchStore();
			var batchRelationships = batch.GetRelationships(node1, RelationshipDirection.All);

			Assert.IsTrue(batch.Execute());

			var restRelationships = batch.GetGraphObject(batchRelationships);

			Assert.IsNotNull(restRelationships);

			Assert.IsTrue(restRelationships.Count() == 3);
		}

		[TestMethod]
		public void UpdatePropertyOnNode1()
		{
			var batch = new BatchStore();
			var batchNode = Node.CreateNode(batch);

			var props = new Properties();
			props.SetProperty("name", "todd");
			props.SetProperty("moved", true);
			props.SetProperty("height", 13);

			batchNode.SaveProperties(props);

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			Assert.IsTrue(restNode.Properties.GetProperty<string>("name") == "todd");
			Assert.IsTrue(restNode.Properties.GetProperty<bool>("moved"));
			Assert.IsTrue(restNode.Properties.GetProperty<int>("height") == 13);
		}

		[TestMethod]
		public void UpdatePropertyOnNode2()
		{
			var batch = new BatchStore();

			var batchNode = Node.CreateNode(batch);

			batchNode.Properties.SetProperty("name", "todd");
			batchNode.Properties.SetProperty("moved", true);
			batchNode.Properties.SetProperty("height", 13);

			batchNode.SaveProperties();

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			Assert.IsTrue(restNode.Properties.GetProperty<string>("name") == "todd");
			Assert.IsTrue(restNode.Properties.GetProperty<bool>("moved"));
			Assert.IsTrue(restNode.Properties.GetProperty<int>("height") == 13);
		}

		[TestMethod]
		public void GetPropertiesOnNode1()
		{
			var batch = new BatchStore();

			var node = Node.CreateNode();

			node.Properties.SetProperty("name", "todd");
			node.Properties.SetProperty("moved", true);
			node.Properties.SetProperty("height", 13);

			node.SaveProperties();

			var batchNode = Node.GetNode(node.Id, batch);

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			Assert.IsTrue(restNode.Properties.GetProperty<string>("name") == "todd");
			Assert.IsTrue(restNode.Properties.GetProperty<bool>("moved"));
			Assert.IsTrue(restNode.Properties.GetProperty<int>("height") == 13);
		}

		[TestMethod]
		public void DeleteAPropertyFromNode()
		{
			var batch = new BatchStore();

			var batchNode = Node.CreateNode(batch);

			batchNode.Properties.SetProperty("name", "todd");
			batchNode.Properties.SetProperty("moved", true);
			batchNode.Properties.SetProperty("height", 13);

			batchNode.SaveProperties();

			batchNode.Properties.RemoveProperty("moved");

			batchNode.SaveProperties();

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			Assert.IsTrue(restNode.Properties.GetProperty<string>("name") == "todd");
			Assert.IsFalse(restNode.Properties.GetPropertyOrDefault<bool>("moved"));
			Assert.IsTrue(restNode.Properties.GetProperty<int>("height") == 13);
		}

		[TestMethod]
		public void AddNodeToIndex()
		{
			var batch = new BatchStore();

			var batchNode = Node.CreateNode(batch);

			var value1 = UniqueValue();

			batchNode.AddToIndex("nodes", "a", value1);

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			var nodes = Node.GetNode("nodes", "a", value1);
			Assert.IsTrue(nodes.Count() == 1);
			Assert.IsTrue(nodes.First() == restNode);
		}

		[TestMethod]
		public void AddRelationshipsToIndex()
		{
			var batch = new BatchStore();

			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);

			var batchRelationship = batchNode1.CreateRelationshipTo(batchNode2, "friend");

			var value1 = UniqueValue();

			batchRelationship.AddToIndex("relationships", "a", value1);

			Assert.IsTrue(batch.Execute());

			var restRelationship = batch.GetGraphObject(batchRelationship);

			var relationships = Relationship.GetRelationship("relationships", "a", value1);
			Assert.IsTrue(relationships.Count() == 1);
			Assert.IsTrue(relationships.First() == restRelationship);
		}

		[TestMethod]
		[ExpectedException(typeof(BatchRemoveFromIndexNotSupportedException))]
		public void RemoveNodeToIndex1()
		{
			var batch = new BatchStore();

			var batchNode = Node.CreateNode(batch);

			var value1 = UniqueValue();
			var value2 = UniqueValue();

			batchNode.AddToIndex("nodes", "a", value1);

			batchNode.RemoveFromIndex("nodes");
		}

		[TestMethod]
		public void RemoveNodeToIndex2()
		{
			var batch = new BatchStore();

			var node = Node.CreateNode();

			var value1 = UniqueValue();
			var value2 = UniqueValue();

			node.AddToIndex("nodes", "a", value1);
			node.AddToIndex("nodes", "b", value2);

			Node.RemoveFromIndex(node, "nodes", null, null, batch);

			Assert.IsTrue(batch.Execute());

			var nodes1 = Node.GetNode("nodes", "a", value1);
			var nodes2 = Node.GetNode("nodes", "b", value2);

			Assert.IsTrue(!nodes1.Any());
			Assert.IsTrue(!nodes2.Any());
		}

		[TestMethod]
		public void RemoveNodeToIndex3()
		{
			var batch = new BatchStore();

			var node = Node.CreateNode();

			var value1 = UniqueValue();

			node.AddToIndex("nodes", "a", value1);
			node.AddToIndex("nodes", "b", value1);

			Node.RemoveFromIndex(node, "nodes", "a", null, batch);

			Assert.IsTrue(batch.Execute());

			var nodes1 = Node.GetNode("nodes", "a", value1);
			var nodes2 = Node.GetNode("nodes", "b", value1);

			Assert.IsTrue(!nodes1.Any());
			Assert.IsTrue(nodes2.Count() == 1);
			Assert.IsTrue(nodes2.First() == node);
		}

		[TestMethod]
		public void RemoveNodeToIndex4()
		{
			var batch = new BatchStore();

			var node = Node.CreateNode();

			var value1 = UniqueValue();
			var value2 = UniqueValue();

			node.AddToIndex("nodes", "a", value1);
			node.AddToIndex("nodes", "a", value2);
			node.AddToIndex("nodes", "b", value2);

			Node.RemoveFromIndex(node, "nodes", "a", value2, batch);

			Assert.IsTrue(batch.Execute());

			var nodes1 = Node.GetNode("nodes", "a", value1);
			var nodes2 = Node.GetNode("nodes", "a", value2);
			var nodes3 = Node.GetNode("nodes", "b", value2);

			Assert.IsTrue(nodes1.Count() == 1);
			Assert.IsTrue(nodes2.Count() == 0);
			Assert.IsTrue(nodes3.Count() == 1);

			Assert.IsTrue(nodes1.First() == node);
			Assert.IsTrue(nodes3.First() == node);
		}

		[TestMethod]
		[ExpectedException(typeof(BatchRemoveFromIndexNotSupportedException))]
		public void RemoveRelationshpToIndex1()
		{
			var batch = new BatchStore();

			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);
			var batchRelationship = batchNode1.CreateRelationshipTo(batchNode2, "like");

			var value1 = UniqueValue();

			batchRelationship.AddToIndex("relationships", "a", value1);

			batchRelationship.RemoveFromIndex("nodes");
		}

		[TestMethod]
		public void RemoveRelationshpToIndex2()
		{
			var batch = new BatchStore();

			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var relationship1 = node1.CreateRelationshipTo(node2, "like");

			var value1 = UniqueValue();
			var value2 = UniqueValue();

			relationship1.AddToIndex("relationships", "a", value1);
			relationship1.AddToIndex("relationships", "b", value2);

			Relationship.RemoveFromIndex(relationship1, "relationships", null, null, batch);

			Assert.IsTrue(batch.Execute());

			var relationships1 = Relationship.GetRelationship("relationships", "a", value1);
			var relationships2 = Relationship.GetRelationship("relationships", "b", value2);

			Assert.IsTrue(!relationships1.Any());
			Assert.IsTrue(!relationships2.Any());
		}

		[TestMethod]
		public void RemoveRelationshipToIndex3()
		{
			var batch = new BatchStore();

			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var relationship1 = node1.CreateRelationshipTo(node2, "like");

			var value1 = UniqueValue();

			relationship1.AddToIndex("relationships", "a", value1);
			relationship1.AddToIndex("relationships", "b", value1);

			Relationship.RemoveFromIndex(relationship1, "relationships", "a", null, batch);

			Assert.IsTrue(batch.Execute());

			var relationships1 = Relationship.GetRelationship("relationships", "a", value1);
			var relationships2 = Relationship.GetRelationship("relationships", "b", value1);

			Assert.IsTrue(!relationships1.Any());
			Assert.IsTrue(relationships2.Count() == 1);
			Assert.IsTrue(relationships2.First() == relationship1);
		}

		[TestMethod]
		public void RemoveRelationshipToIndex4()
		{
			var batch = new BatchStore();

			var node1 = Node.CreateNode();
			var node2 = Node.CreateNode();

			var relationship1 = node1.CreateRelationshipTo(node2, "like");

			var value1 = UniqueValue();
			var value2 = UniqueValue();

			relationship1.AddToIndex("relationships", "a", value1);
			relationship1.AddToIndex("relationships", "a", value2);
			relationship1.AddToIndex("relationships", "b", value2);

			Relationship.RemoveFromIndex(relationship1, "relationships", "a", value2, batch);

			Assert.IsTrue(batch.Execute());

			var relationships1 = Relationship.GetRelationship("relationships", "a", value1);
			var relationships2 = Relationship.GetRelationship("relationships", "a", value2);
			var relationships3 = Relationship.GetRelationship("relationships", "b", value2);

			Assert.IsTrue(relationships1.Count() == 1);
			Assert.IsTrue(relationships2.Count() == 0);
			Assert.IsTrue(relationships3.Count() == 1);

			Assert.IsTrue(relationships1.First() == relationship1);
			Assert.IsTrue(relationships3.First() == relationship1);
		}

		[TestMethod]
		public void AddNodeToIndexUnique1()
		{
			var batch = new BatchStore();

			var batchNode = Node.CreateNode(batch);

			var value1 = UniqueValue();

			var batchUniqueNode = batchNode.AddToIndex("nodes", "a", value1, true);

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchUniqueNode);

			var nodes = Node.GetNode("nodes", "a", value1);
			Assert.IsTrue(nodes.Count() == 1);
			Assert.IsTrue(nodes.First() == restNode);
		}

		[TestMethod]
		public void AddNodeToIndexUnique2()
		{
			var batch = new BatchStore();

			var batchNode1 = Node.CreateNode(batch);
			var batchNode2 = Node.CreateNode(batch);

			var value1 = UniqueValue();

			var batchUniqueNode1 = batchNode1.AddToIndex("nodes", "a", value1, true);
			var batchUniqueNode2 = batchNode2.AddToIndex("nodes", "a", value1, true);

			Assert.IsTrue(batch.Execute());

			var restNode1 = batch.GetGraphObject(batchUniqueNode1);
			var restNode2 = batch.GetGraphObject(batchUniqueNode2);

			var nodes = Node.GetNode("nodes", "a", value1);
			Assert.IsTrue(nodes.Count() == 1);
			Assert.IsTrue(nodes.First() == restNode1);
			Assert.IsTrue(restNode1 == restNode2);
		}

		[TestMethod]
		public void CreateUniqueNodeInIndex1()
		{
			var batch = new BatchStore();

			var value1 = UniqueValue();

			var props = new Properties();
			props.SetProperty("name", "jack");

			var batchNode = Node.CreateUniqueNode("nodes", "a", value1, IndexUniqueness.CreateOrFail, props, batch);

			Assert.IsTrue(batch.Execute());

			var restNode = batch.GetGraphObject(batchNode);

			var nodes = Node.GetNode("nodes", "a", value1);

			Assert.IsTrue(nodes.Count() == 1);
			Assert.IsTrue(nodes.First() == restNode);
			Assert.IsTrue(restNode.Properties.GetProperty<string>("name") == "jack");
		}

		[TestMethod]
		public void CreateUniqueNodeInIndex2()
		{
			var batch = new BatchStore();

			var value1 = UniqueValue();

			var props = new Properties();
			props.SetProperty("name", "jack");

			var node = Node.CreateUniqueNode("nodes", "a", value1, IndexUniqueness.GetOrCreate, props);
			
			var batchNode = Node.CreateUniqueNode("nodes", "a", value1, IndexUniqueness.CreateOrFail, props, batch);

			//Should fail because dup key in index
			Assert.IsFalse(batch.Execute());

		}

		[TestMethod]
		public void CreateUniqueNodeInIndex3()
		{
			var batch = new BatchStore();

			var value1 = UniqueValue();

			var props1 = new Properties();
			props1.SetProperty("name", "jack");
			
			var props2 = new Properties();
			props2.SetProperty("name", "frank");

			var batchNode1 = Node.CreateUniqueNode("nodes", "a", value1, IndexUniqueness.CreateOrFail, props1, batch);
			var batchNode2 = Node.CreateUniqueNode("nodes", "a", value1, IndexUniqueness.CreateOrFail, props2, batch);
			
			//var batchNode1 = Node.CreateUniqueNode("nodes", "a", value1, IndexUniqueness.CreateOrFail, null, batch);
			//var batchNode2 = Node.CreateUniqueNode("nodes", "a", value1, IndexUniqueness.CreateOrFail, null, batch);


			Assert.IsFalse(batch.Execute());

		
		}
	}
}
