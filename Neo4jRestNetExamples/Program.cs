using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4jRestNet.Core;
using Neo4jRestNet.GremlinPlugin;
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

			// Gremlin returning a datatable
			GremlinScript tblScript = new GremlinScript();
			tblScript.NewTable("t")
				.gV(RootNode)   
				.Out(RelationshipType.Likes.ToString())
				.As("Like")
				.Table("t", "Like")
				.Append(" >> -1; t;");

			DataTable dt = Gremlin.GetTable(tblScript.ToString());
		}
	}
}
