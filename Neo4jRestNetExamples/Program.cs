using System;
using System.Collections.Generic;
using Neo4jRestNet.Core;
using Neo4jRestNet.GremlinPlugin;
using Neo4jRestNet.CypherPlugin;
using System.Data;

namespace Example
{
	class Program
	{

		public enum NodeProperty
		{
			FirstName,
			LastName,
			PWD,
			UID
		}

		public enum NodeType
		{
			User,
			Supplier,
			Movie,
			Content
		}

		public enum RelationshipProperty
		{
			Name
		}

		public enum RelationshipType
		{
			Likes,
			Knows
		}

		static void Main(string[] args)
		{

			//var dnode = Node.GetNode(81);

			//var node = Node.CreateNode("MyType");
			//node.Properties.SetProperty("myDate", new DateTime(2011, 12, 15));

			//node.SaveProperties();

			//Neo4jRestNet.CleanDbPlugin.CleanDbPlugin.CleanDb();

			// Get Root Node from graphDB
			var rootNode = Node.GetRootNode();

			// Create a User Node with no Properties 
			var nodeUser = Node.CreateNode(NodeType.User.ToString());

			// Create a User Node with Properties
			var prop = new Properties();
			prop.SetProperty(NodeProperty.FirstName.ToString(), "Joe");
			prop.SetProperty(NodeProperty.LastName.ToString(), "Smith");

			var nodeUserWithName = Node.CreateNode(NodeType.User.ToString(), prop);

			// Create Relationships to Nodes
			rootNode.CreateRelationshipTo(nodeUser, RelationshipType.Likes.ToString());
			rootNode.CreateRelationshipTo(nodeUserWithName, RelationshipType.Likes.ToString());

			// Create Relationship with Properties
			var relProp = new Properties();
			relProp.SetProperty(RelationshipProperty.Name.ToString(), "MyRelationship");
			relProp.SetProperty("CustomRelProp", "CustomPropValue");

			nodeUserWithName.CreateRelationshipTo(nodeUser, RelationshipType.Knows.ToString(), relProp);

			// Get Id From Node
			var id = rootNode.Id;	
			var geid = rootNode.EncryptedId;


			// Gremlin 

		
			// Same as above
			var sameLikeNodes = Gremlin.Post<Node>(new GremlinScript(rootNode).Out(RelationshipType.Likes.ToString()));

			// More Gremlin example
			var script = new GremlinScript(rootNode);
			script.OutE()
				.InV()
				.OutE()
				.Filter("it.getProperty('{0}') == '{1}'", RelationshipProperty.Name, "MyRelationship");

			var myRelationship = Gremlin.Post<Relationship>(script);

			// More Gremlin example
			var script1 = new GremlinScript(rootNode);
			script1.OutE()
				.InV()
				.OutE()
				.Filter(it => it.GetProperty(RelationshipProperty.Name.ToString()) == "MyRelationship");

			IEnumerable<Relationship> myRelationship1 = Gremlin.Post<Relationship>(script1);

			// Gremlin returning a datatable
			var tblScript = new GremlinScript();
			tblScript.NewTable("t")
				.gV(rootNode)   
				.Out(RelationshipType.Likes.ToString())
				.As("Like")
				.Table("t", "Like")
				.Append(" >> -1; t;");

			DataTable dt = Gremlin.GetTable(tblScript);

			// Basic Cypher query
			var c1 = new Cypher();
			c1.Start(s => s.Node("A", 0));
			c1.Return( r => r.Node("A"));

			DataTable tbl = c1.Post();

			// Cypher with Match clause
			var c2 = new Cypher();
			c2.Start(s => s.Node("A", 0));
			c2.Match(m => m.Node("A").To("r", "Likes").Node("B"));
			c2.Return(r => r.Node("A").Relationship("r").Node("B"));

			tbl = c2.Post();

			// Cypher with multi start and return optional property
			var c3 = new Cypher();
			c3.Start(s => s.Node("A", 0, 1));
			c3.Match(m => m.Node("A").Any("r", "Likes").Node("C"));
		//	c3.Return(r => r.Node("C").Node("C").Property("Name?"));

			tbl = c3.Post();

			// Multi Start
			var c4 = new Cypher();
			c4.Start(s => s.Node("A", 0).Node("B",1));
			c4.Return(r => r.Node("A").Node("B"));

			tbl = c4.Post();

			// Cypher with Where clause
			var c5 = new Cypher();
			c5.Start(s => s.Node("A", 0, 1));
			c5.Where(w => w.Node("A").Property("Age?") < 30 && w.Node("A").Property("Name?") == "Tobias" || !(w.Node("A").Property("Name?") == "Tobias"));
			c5.Return(r => r.Node("A"));

			tbl = c5.Post();

			// Alt Syntax
			var c6 = new Cypher();
			c6.Start(s =>	{
								s.Node("A", 0);
								s.Node("B", 1);
								return s;
							});

			c6.Return(r =>	{
								r.Node("A");
								r.Node("B");
								return r;
							});

			tbl = c6.Post();

			// Alt Syntax
			var c7 = new Cypher();
			c7.Start(s => s.Node("MyNode", "Index-Name", "QueryString"));
			c7.Start(s => s.Node("A", 0));
			c7.Start(s => s.Node("B", 1));
			
			c7.Return(r => r.Node("A"));
			c7.Return(r => r.Node("B"));

			tbl = c7.Post();

			// Test Cypher Dates
			var date = new DateTime(2011, 12, 15);

			//// Basic Cypher query
			//var c1 = new Cypher();
			//c1.Start(s => s.Node("A", 81));
			//c1.Where(w => w.Node("A").Property("myDate") < date.AddDays(2)
			//    && w.Node("A").Property("myDate") == new DateTime(2011, 12, 15));

			//c1.Return(r => r.Node("A"));

			//tbl = c1.Post();

		}
	}
}
