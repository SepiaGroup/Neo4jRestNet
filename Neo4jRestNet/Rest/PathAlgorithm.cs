namespace Neo4jRestNet.Rest
{
    public class PathAlgorithm
    {
		public static readonly PathAlgorithm ShortestPath = new PathAlgorithm("shortestPath");
		public static readonly PathAlgorithm AllPaths = new PathAlgorithm("allPaths");
		public static readonly PathAlgorithm AllSimplePaths = new PathAlgorithm("allSimplePaths ");
		public static readonly PathAlgorithm Dijkstra = new PathAlgorithm("dijkstra");

		private readonly string _pathAlgorithm;

		private PathAlgorithm(string pathAlgorithm)
		{
			_pathAlgorithm = pathAlgorithm;
		}

        public override string ToString()
        {
			return _pathAlgorithm;
        }
    }
}