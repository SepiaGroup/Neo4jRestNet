namespace Neo4jRestNet.Core
{
    public interface IGraphObject 
    {
		long Id { get; }
		string DbUrl { get; }
        string Self { get; }
    }
}