# Neo4jRestNet
# .Net wrapper for the Neo4j REST server
## Configuration:
    <configuration>
        <configSections>
    		<section name="neo4jRestNet" type="Neo4jRestNet.Configuration.ConnectionSettings, Neo4jRestNet" />
    	</configSections>
    
    	<neo4jRestNet>
    		<databases>
    			<add name="neo4j" default="true" https="false" domain="localhost" port="7474" />
    		</databases>
    	</neo4jRestNet>
    
        <!-- Only needed if you are using the encryption class -->    
    	<appSettings>
    		<add key="EncryptIdKey" value="KeyForEncrypting"/>
    		<add key="EncryptIdIV" value="IVForEncrypting1"/>
    	</appSettings>
    </configuration>

## Examples:
### Get Root Node:
    Node RootNode = Node.GetRootNode();
### Create a Node with no Properties 
    Node node = Node.CreateNode();

### Create a Node with Properties
    Properties prop = new Properties();
    prop.SetProperty(NodeProperty.FirstName, "Joe");
    prop.SetProperty(NodeProperty.LastName, "Smith");
    Node nodeUserWithName = Node.CreateNode(prop);

### Create Relationships to Nodes
    RootNode.CreateRelationshipTo(nodeUserWithName, "Likes");

### Create Relationship with Properties
    Properties RelProp = new Properties();
    RelProp.SetProperty("CustomRelProp", "CustomPropValue");
    nodeUserWithName.CreateRelationshipTo(node, "Knows", RelProp);

## Gremlin 
### Get Like relationships from the Root Node
    IEnumerable<Node> LikeNodes = Gremlin.Post<Node>(RootNode.Id, "out('Likes')");

### Same as above
    IEnumerable<Node> SameLikeNodes = Gremlin.Post<Node>(new GremlinScript(RootNode).Out(RelationshipType.Likes.ToString()));

### More Gremlin example
    GremlinScript script = new GremlinScript(RootNode);
    script.OutE()
            .InV()
            .OutE()
            .Filter(it => it.getProperty(RelationshipProperty.Name.ToString()) == "MyRelationship");
    
    IEnumerable<Relationship> myRelationship = Gremlin.Post<Relationship>(script);`

### Gremlin returning a datatable
    GremlinScript tblScript = new GremlinScript();
    tblScript.NewTable("t")
                    .gV(RootNode)   
                    .Out(RelationshipType.Likes.ToString())
                    .As("Like")
                    .Table("t", new List<string>{ "Like" })
                    .Append(" >> -1; t;");
    
    DataTable dt = Gremlin.GetTable(tblScript);

