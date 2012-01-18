using System;
using Neo4jRestNet.Core.Interface;

namespace Neo4jRestNet.Core
{
	public class GraphFactory
	{
		public static INode CreateNode()
		{
			return new Implementation.Node();
		}

		public static INode CreateNode<TNode>(params object[] args) where TNode : class, INode, new()
		{
			return (TNode)Activator.CreateInstance(typeof(TNode), args);
		}

		public static IRelationship CreateRelationship()
		{
			return new Implementation.Relationship();
		}

		public static IRelationship CreateRelationship<TRelationship>(params object[] args) where TRelationship : class, IRelationship, new()
		{
			return (TRelationship)Activator.CreateInstance(typeof(TRelationship), args);
		}

		public static IPath CreatePath()
		{
			return new Implementation.Path();
		}

		public static IPath CreatePath<TPath>(params object[] args) where TPath : class, IPath, new()
		{
			return (TPath)Activator.CreateInstance(typeof(TPath), args);
		}

	}
}
