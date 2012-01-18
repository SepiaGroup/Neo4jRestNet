using System.Collections.Generic;
using System.Data;
using Neo4jRestNet.Core;
using Neo4jRestNet.Core.Implementation;
using Neo4jRestNet.Core.Interface;
using Neo4jRestNet.CypherPlugin;
using Neo4jRestNet.GremlinPlugin;

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

			// Get Root Node from graphDB - default implementation
			var rootNode = GraphFactory.CreateNode().GetRootNode();

			// Get Root Node from graphDB - using passedin implementation (using default implementation for tesing)
			rootNode = GraphFactory.CreateNode<Node>().GetRootNode();



			// Create a User Node with no Properties - default implementation  
			var nodeUser = GraphFactory.CreateNode().CreateNode(NodeType.User.ToString());

			// Create a User Node with no Properties - using passedin implementation (using default implementation for tesing) 
			nodeUser = GraphFactory.CreateNode<Node>().CreateNode(NodeType.User.ToString());


			// Create a User Node with Properties - default implementation
			var prop = new Properties();
			prop.SetProperty(NodeProperty.FirstName.ToString(), "Joe");
			prop.SetProperty(NodeProperty.LastName.ToString(), "Smith");

			var nodeUserWithName = GraphFactory.CreateNode().CreateNode(NodeType.User.ToString(), prop);

			// Create a User Node with Properties - using passedin implementation (using default implementation for tesing) 
			prop = new Properties();
			prop.SetProperty(NodeProperty.FirstName.ToString(), "Joe");
			prop.SetProperty(NodeProperty.LastName.ToString(), "Smith");

			nodeUserWithName = GraphFactory.CreateNode<Node>().CreateNode(NodeType.User.ToString(), prop);
			

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
			var sameLikeNodes = GremlinFactory.CreateGremlin().GetNodes(new GremlinScript(rootNode).Out(RelationshipType.Likes.ToString()));

			sameLikeNodes = GremlinFactory.CreateGremlin<Gremlin>().GetNodes<Node>(new GremlinScript(rootNode).Out(RelationshipType.Likes.ToString()));

			// More Gremlin example
			var script = new GremlinScript(rootNode);
			script.OutE()
				.InV()
				.OutE()
				.Filter("it.getProperty('{0}') == '{1}'", RelationshipProperty.Name, "MyRelationship");

			var myRelationship = GremlinFactory.CreateGremlin().GetRelationships(script);

			myRelationship = GremlinFactory.CreateGremlin().GetRelationships<Relationship>(script);

			// More Gremlin example
			var script1 = new GremlinScript(rootNode);
			script1.OutE()
				.InV()
				.OutE()
				.Filter(it => it.GetProperty(RelationshipProperty.Name.ToString()) == "MyRelationship");

			IEnumerable<IRelationship> myRelationship1 = GremlinFactory.CreateGremlin().GetRelationships(script1);

			myRelationship1 = GremlinFactory.CreateGremlin().GetRelationships<Relationship>(script1);

			// Gremlin returning a datatable
			var tblScript = new GremlinScript();
			tblScript.NewTable("t")
				.gV(rootNode)   
				.Out(RelationshipType.Likes.ToString())
				.As("Like")
				.Table("t", "Like")
				.Append(" >> -1; t;");

			DataTable dt = GremlinFactory.CreateGremlin().GetTable(tblScript);

			// Basic Cypher query
			var c1 = CypherFactory.CreateCypher();
			c1.Start(s => s.Node("A", 0));
			c1.Return( r => r.Node("A"));

			DataTable tbl = c1.Post();

			var c1i = CypherFactory.CreateCypher<Cypher>();
			c1i.Start(s => s.Node("A", 0));
			c1i.Return(r => r.Node("A"));

			tbl = c1i.Post();


			// Cypher with Match clause
			var c2 = CypherFactory.CreateCypher();
			c2.Start(s => s.Node("A", 0));
			c2.Match(m => m.Node("A").To("r", "Likes").Node("B"));
			c2.Return(r => r.Node("A").Relationship("r").Node("B"));

			tbl = c2.Post();

			// Cypher with multi start and return optional property
			var c3 = CypherFactory.CreateCypher();
			c3.Start(s => s.Node("A", 0, 1));
			c3.Match(m => m.Node("A").Any("r", "Likes").Node("C"));
			c3.Return(r => r.Node("C").Node("C").Property("Name?"));

			tbl = c3.Post();

			// Multi Start
			var c4 = CypherFactory.CreateCypher();
			c4.Start(s => s.Node("A", 0).Node("B",1));
			c4.Return(r => r.Node("A").Node("B"));

			tbl = c4.Post();

			// Cypher with Where clause
			var c5 = CypherFactory.CreateCypher();
			c5.Start(s => s.Node("A", 0, 1));
			c5.Where(w => w.Node("A").Property("Age?") < 30 && w.Node("A").Property("Name?") == "Tobias" || !(w.Node("A").Property("Name?") == "Tobias"));
			c5.Return(r => r.Node("A"));

			tbl = c5.Post();

			// Alt Syntax
			var c6 = CypherFactory.CreateCypher();
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
			var c7 = CypherFactory.CreateCypher();
			c7.Start(s => s.Node("MyNode", "Index-Name", "QueryString"));
			c7.Start(s => s.Node("A", 0));
			c7.Start(s => s.Node("B", 1));
			
			c7.Return(r => r.Node("A"));
			c7.Return(r => r.Node("B"));

			tbl = c7.Post();


		 }
 
	}
}
