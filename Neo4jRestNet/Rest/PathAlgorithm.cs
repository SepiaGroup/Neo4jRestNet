using System.Collections.Generic;
using Neo4jRestNet.Core;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class PathAlgorithm
    {
		public static readonly PathAlgorithm ShortestPath = new PathAlgorithm("shortestPath");
		public static readonly PathAlgorithm AllPaths = new PathAlgorithm("allPaths");
		public static readonly PathAlgorithm AllSimplePaths = new PathAlgorithm("allSimplePaths ");
		public static readonly PathAlgorithm Dijkstra = new PathAlgorithm("dijkstra");

		private string _PathAlgorithm;

		private PathAlgorithm(string PathAlgorithm)
		{
			_PathAlgorithm = PathAlgorithm;
		}

        public override string ToString()
        {
			return _PathAlgorithm;
        }
    }
}