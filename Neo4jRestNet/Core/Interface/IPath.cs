using System;
using System.Collections.Generic;
using System.Linq;
using Neo4jRestNet.Core.Interface;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Neo4jRestNet.Core.Interface
{
	public interface IPath : IGraphObject
	{
		INode StartNode { get; }
		INode EndNode { get; }
		List<INode> Nodes { get; }
		List<IRelationship> Relationships { get; }
		string OriginalPathJson { get; }

		List<IPath> ParseJson(string jsonPaths);
		List<IPath> ParseJson(JArray jsonPaths);
	}
}
