# Neo4jRestNet
# .Net wrapper for the Neo4j REST server
## Configuration:
    <configuration>
        <connectionStrings>
            <add name="neo4j"  connectionString="http://localhost:7474/db/data/" />
            <add name="neo4jGremlinExtension"  connectionString="/ext/GremlinPlugin/graphdb/execute_script/" />
        </connectionStrings>

        <appSettings>
            <add key="GEIDKey" value="KeyForEncrypting"/>
            <add key="GEIDIV" value="IVForEncrypting1"/>
        </appSettings>
    </configuration>

## Examples:
### Get Root Node:
    Node RootNode = Node.GetRootNode();
### Create a User Node with no Properties 
    Node nodeUser = Node.CreateNode(NodeType.User);

### Create a User Node with Properties
    Properties prop = new Properties();
    prop.SetProperty(NodeProperty.FirstName, "Joe");
    prop.SetProperty(NodeProperty.LastName, "Smith");
    Node nodeUserWithName = Node.CreateNode(NodeType.User, prop);

### Create Relationships to Nodes
    RootNode.CreateRelationshipTo(nodeUser, RelationshipType.Likes);
    RootNode.CreateRelationshipTo(nodeUserWithName, RelationshipType.Likes);

### Create Relationship with Properties
    Properties RelProp = new Properties();
    RelProp.SetProperty(RelationshipProperty.Name, "MyRelationship");
    RelProp.SetProperty("CustomRelProp", "CustomPropValue");
    nodeUserWithName.CreateRelationshipTo(nodeUser, RelationshipType.Knows, RelProp);

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

