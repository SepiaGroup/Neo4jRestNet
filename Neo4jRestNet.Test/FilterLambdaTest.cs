using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.GremlinPlugin;

namespace Neo4jRestNet.Test
{
	[TestClass]
	public class FilterLambdaTest
	{
		public enum NodeProperty
		{
			FirstName,
			LastName,
			PWD,
			UID
		}

		public enum NodeType
		{
			User,
			Supplier,
			Movie,
			Content
		}

		public enum RelationshipProperty
		{
			Name,
			Date
		}

		public enum RelationshipType
		{
			Likes,
			Knows
		}

		[TestMethod]
		public void FilterClause()
		{
			GremlinScript script = new GremlinScript();
			script.Append("g.v(0)")
				.Filter(it => it.GetProperty("MyProp") == "SomeValue");

			Assert.IsTrue(script.ToString() == "g.v(0).filter{it.getProperty('MyProp') == 'SomeValue'}");
		}

		[TestMethod]
		public void FilterClauseEqualBool()
		{
			GremlinScript script = new GremlinScript();
			script.Append("g.v(0)")
				.Filter(it => it.GetProperty("MyProp") == true);

			Assert.IsTrue(script.ToString() == "g.v(0).filter{it.getProperty('MyProp') == true}");
		}

		[TestMethod]
		public void FilterClauseEqualNumber()
		{
			GremlinScript script = new GremlinScript();
			script.Append("g.v(0)")
				.Filter(it => it.GetProperty("MyProp") == 123);

			Assert.IsTrue(script.ToString() == "g.v(0).filter{it.getProperty('MyProp') == 123}");
		}

		[TestMethod]
		public void FilterClauseChainCommands()
		{
			GremlinScript script = new GremlinScript();
			script.Append("g.v(0)")
				.Filter(it => it.GetProperty("MyProp").ToLowerCase().Contains("ContainThis"));

			Assert.IsTrue(script.ToString() == "g.v(0).filter{it.getProperty('MyProp').toLowerCase().contains('ContainThis')}");
		}


		[TestMethod]
		public void FilterClauseCompound()
		{
			GremlinScript script = new GremlinScript();
			script.Append("g.v(0)")
				.Filter(it => it.GetProperty("MyProp") && !it.GetProperty("NotProp") || it.GetProperty("OrProp").ToLowerCase() == "lower Value");

			Assert.IsTrue(script.ToString() == "g.v(0).filter{it.getProperty('MyProp') && !it.getProperty('NotProp') || it.getProperty('OrProp').toLowerCase() == 'lower Value'}");
		}

		[TestMethod]
		public void FilterClauseCompareToIgnoreCase1()
		{
			GremlinScript script = new GremlinScript();
			script.Append("g.v(0)")
				.Filter(it => it.GetProperty("MyProp").CompareToIgnoreCase("CompareValue"));

			Assert.IsTrue(script.ToString() == "g.v(0).filter{it.getProperty('MyProp').compareToIgnoreCase('CompareValue')}");
		}

		[TestMethod]
		public void FilterClauseCompareToIgnoreCase2()
		{
			GremlinScript script = new GremlinScript();
			script.Append("g.v(0)")
				.Filter(it => it.GetProperty("MyProp").CompareToIgnoreCase(it.GetProperty("CompareProperty").Name()));

			Assert.IsTrue(script.ToString() == "g.v(0).filter{it.getProperty('MyProp').compareToIgnoreCase(it.getProperty('CompareProperty').name)}");
		}

		[TestMethod]
		public void Table()
		{
			GremlinScript script = new GremlinScript();
			script.NewTable("t")
				.NodeIndexLookup(new Dictionary<string, object>() { { NodeProperty.FirstName.ToString() , "Jack" }, { NodeProperty.LastName.ToString(), "Shaw" } })
				.Filter(it => it.GetProperty(NodeProperty.UID.ToString()) == "jshaw") 
				.As("UserNode")
				.OutE(RelationshipType.Likes.ToString())
				.As("LikeRel")
				.InV()
				.As("FriendNode")
				.Table("t", "UserNode", "LikeRel", "FriendNode")
				.Append("{{it}}{{it.getProperty('{0}')}}{{it.getProperty('{1}')}} >> -1; t", RelationshipProperty.Date.ToString(), NodeProperty.FirstName.ToString());

			Assert.IsTrue(script.ToString() == "t = new Table();g.V[['FirstName':'Jack','LastName':'Shaw']].filter{it.getProperty('UID') == 'jshaw'}.as('UserNode').outE('Likes').as('LikeRel').inV().as('FriendNode').table(t, ['UserNode','LikeRel','FriendNode']){it}{it.getProperty('Date')}{it.getProperty('FirstName')} >> -1; t");
		}

	}
}
