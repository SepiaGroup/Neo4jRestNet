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
    IEnumerable<Node> LikeNodes = Gremlin.Post<Node>(RootNode.NodeId, "out('Likes')");

### Same as above
    IEnumerable<Node> SameLikeNodes = Gremlin.Post<Node>(new GremlinScript(RootNode).Out(RelationshipType.Likes).ToString());

### More Gremlin example
    GremlinScript script = new GremlinScript(RootNode);
    script.OutE()
            .InV()
            .OutE()
            .Filter("it.getProperty('{0}') == '{1}'", RelationshipProperty.Name, "MyRelationship");
    
    IEnumerable<Relationship> myRelationship = Gremlin.Post<Relationship>(script.ToString());`

### Gremlin returning a datatable
    GremlinScript tblScript = new GremlinScript();
    tblScript.NewTable("t")
                    .gV(RootNode.NodeId)   
                    .Out(RelationshipType.Likes)
                    .As("Like")
                    .Table("t", new List<string>{ "Like" })
                    .Append(" >> -1; t;");
    
    DataTable dt = Gremlin.GetTable(tblScript.ToString());

## GEID
### What is a GEID
All Graph Elements (Nodes, Relationships) have their IDs defined as GEID.  A GEID will encrypt a Graph Element ID and return the encrypted string by default.  If you explicitly convert a GEID to a long you will obtain the unencrypted long ID.  This is done so that you can pass the GEID to a webpage without worrying about the ID being hijacked.  The internal representation of a GEID is a long so there is very little overhead using this class.  The keys for the encrypting are set in the configure settings (see above)

#### Get ID from a Node
    long Id = (long)RootNode.NodeId;  
    string strId = RootNode.NodeId;  // the encrypted Id

#### Convert a string to GEID

    string myStringGIED == “SOME STRING GEID VALUE”;
    GEID geid = myStringGEID;
    
    // or you can do

    GEID newGEID;
    If( GEID.TryParse(myStringGEID, out newGEID)
    {
        // Valid GEID
    }
    
## Type/Property Classes
### NodeTypeBase
### NodePropertyBase
### RelationshipTypeBase
### RelationshipPropertyBase

These classes are used as helper classes to reduce the use of string names throughout your code.  You do not need to use these this wrapper but you do need to initialize them once.  See code for examples.   

