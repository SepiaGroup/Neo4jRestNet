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
		static void Main(string[] args)
		{

			// If you were building a Web Application the following Initializations
			// would go in the Application_Start method in the global.asax

			// Initialize Node Types
			NodeTypeBase.Initialize();
			NodeType.Initialize();

			// Initialize Node Properties
			NodePropertyBase.Initialize();
			NodeProperty.Initialize();

			// Initialize Relationship Types
			RelationshipTypeBase.Initialize();
			RelationshipType.Initialize();

			// Initialize Relationship Properties
			RelationshipPropertyBase.Initialize();
			RelationshipProperty.Initialize();



			// Get Root Node from graphDB
			Node RootNode = Node.GetRootNode();

			// Create a User Node with no Properties 
			Node nodeUser = Node.CreateNode(NodeType.User);

			// Create a User Node with Properties
			Properties prop = new Properties();
			prop.SetProperty(NodeProperty.FirstName, "Joe");
			prop.SetProperty(NodeProperty.LastName, "Smith");

			Node nodeUserWithName = Node.CreateNode(NodeType.User, prop);

			// Create Relationships to Nodes
			RootNode.CreateRelationshipTo(nodeUser, RelationshipType.Likes);
			RootNode.CreateRelationshipTo(nodeUserWithName, RelationshipType.Likes);

			// Create Relationship with Properties
			Properties RelProp = new Properties();
			RelProp.SetProperty(RelationshipProperty.Name, "MyRelationship");
			RelProp.SetProperty("CustomRelProp", "CustomPropValue");

			nodeUserWithName.CreateRelationshipTo(nodeUser, RelationshipType.Knows, RelProp);

			// Get Id From Node
			long Id = (long)RootNode.NodeId;	//  ** NOTE: All Graph Element ID's are GEID
			string geid = RootNode.NodeId;		//  ** you must cast them to long to get their numeric value
												//  ** the string value from a GEID is encrypted 
												//  ** this is done so that you can pass the ID to 
												//  ** a web page without worring about it being hijacked.
												//  ** GEID implements IEquatable



			// Gremlin 

			// Get Like relationships from the Root Node
			IEnumerable<Node> LikeNodes = Gremlin.Post<Node>(RootNode.NodeId, "out('Likes')");
			
			// Same as above
			IEnumerable<Node> SameLikeNodes = Gremlin.Post<Node>(new GremlinScript(RootNode).Out(RelationshipType.Likes).ToString());

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
				.gV(RootNode.NodeId)   
				.Out(RelationshipType.Likes)
				.As("Like")
				.Table("t", "Like")
				.Append(" >> -1; t;");

			DataTable dt = Gremlin.GetTable(tblScript.ToString());
		}
	}
}
