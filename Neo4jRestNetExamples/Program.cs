using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

			// Get Root Node from graphDB
			Node RootNode = Node.GetRootNode();

			// Create a User Node with no Properties 
			Node nodeUser = Node.CreateNode(NodeType.User.ToString());

			// Create a User Node with Properties
			Properties prop = new Properties();
			prop.SetProperty(NodeProperty.FirstName.ToString(), "Joe");
			prop.SetProperty(NodeProperty.LastName.ToString(), "Smith");

			Node nodeUserWithName = Node.CreateNode(NodeType.User.ToString(), prop);

			// Create Relationships to Nodes
			RootNode.CreateRelationshipTo(nodeUser, RelationshipType.Likes.ToString());
			RootNode.CreateRelationshipTo(nodeUserWithName, RelationshipType.Likes.ToString());

			// Create Relationship with Properties
			Properties RelProp = new Properties();
			RelProp.SetProperty(RelationshipProperty.Name.ToString(), "MyRelationship");
			RelProp.SetProperty("CustomRelProp", "CustomPropValue");

			nodeUserWithName.CreateRelationshipTo(nodeUser, RelationshipType.Knows.ToString(), RelProp);

			// Get Id From Node
			long Id = RootNode.Id;	
			string geid = RootNode.EncryptedId;


			// Gremlin 

			// Get Like relationships from the Root Node
			
			IEnumerable<Node> LikeNodes = Gremlin.Post<Node>(RootNode.Id, "out('Likes')");
			
			// Same as above
			IEnumerable<Node> SameLikeNodes = Gremlin.Post<Node>(new GremlinScript(RootNode).Out(RelationshipType.Likes.ToString()));

			// More Gremlin example
			GremlinScript script = new GremlinScript(RootNode);
			script.OutE()
				.InV()
				.OutE()
				.Filter("it.getProperty('{0}') == '{1}'", RelationshipProperty.Name, "MyRelationship");

			IEnumerable<Relationship> myRelationship = Gremlin.Post<Relationship>(script.ToString());

			// More Gremlin example
			GremlinScript script1 = new GremlinScript(RootNode);
			script1.OutE()
				.InV()
				.OutE()
				.Filter(it => it.GetProperty(RelationshipProperty.Name.ToString()) == "MyRelationship");

			IEnumerable<Relationship> myRelationship1 = Gremlin.Post<Relationship>(script1.ToString());

			// Gremlin returning a datatable
			GremlinScript tblScript = new GremlinScript();
			tblScript.NewTable("t")
				.gV(RootNode)   
				.Out(RelationshipType.Likes.ToString())
				.As("Like")
				.Table("t", "Like")
				.Append(" >> -1; t;");

			DataTable dt = Gremlin.GetTable(tblScript.ToString());

			// Basic Cypher query
			CypherQuery c1 = new CypherQuery();
			c1.Start(s => s.Node("A", 0));
			c1.Return( r => r.Node("A"));

			DataTable tbl = Cypher.Post(c1.ToString());

			// Cypher with Match clause
			CypherQuery c2 = new CypherQuery();
			c2.Start(s => s.Node("A", 0));
			c2.Match(m => m.Node("A").To("r", "Likes").Node("B"));
			c2.Return(r => r.Node("A").Relationship("r").Node("B"));

			tbl = Cypher.Post(c2);

			// Cypher with multi start and return optional property
			CypherQuery c3 = new CypherQuery();
			c3.Start(s => s.Node("A", 0, 1));
			c3.Match(m => m.Node("A").Any("r", "Likes").Node("C"));
			c3.Return(r => r.Node("C").Node("C").Property("Name?"));

			tbl = Cypher.Post(c3);

			// Multi Start
			CypherQuery c4 = new CypherQuery();
			c4.Start(s => s.Node("A", 0).Node("B",1));
			c4.Return(r => r.Node("A").Node("B"));

			tbl = Cypher.Post(c4.ToString());

			// Cypher with Where clause
			CypherQuery c5 = new CypherQuery();
			c5.Start(s => s.Node("A", 0, 1));
			c5.Where(w => w.Node("A").Property("Age?") < 30 && w.Node("A").Property("Name?") == "Tobias" || !(w.Node("A").Property("Name?") == "Tobias"));
			c5.Return(r => r.Node("A"));

			tbl = Cypher.Post(c5);

			// Alt Syntax
			CypherQuery c6 = new CypherQuery();
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

			tbl = Cypher.Post(c6);

			// Alt Syntax
			CypherQuery c7 = new CypherQuery();
			c7.Start(s => s.Node("A", 0));
			c7.Start(s => s.Node("B", 1));

			c7.Return(r => r.Node("A"));
			c7.Return(r => r.Node("B"));

			tbl = Cypher.Post(c7);

		}
	}
}
