namespace Neo4jRestNet.Core
{
    public interface IGraphObject 
    {
		long Id { get; }
		string DbUrl { get; }
        string Self { get; }

/*
        /// <summary>
        /// Database identity
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Gets the propeties for the object
        /// </summary>
        /// <typeparam name="TPropertyContainer">Property Container Object</typeparam>
        /// <returns>Property container object</returns>
        TPropertyContainer GetProperties<TPropertyContainer>() where TPropertyContainer : Properties, new();

        /// <summary>
        /// Updates the graph object's properties
        /// </summary>
        /// <param name="properties"></param>
        void CommitChanges(Properties properties);   
*/     
    }
}