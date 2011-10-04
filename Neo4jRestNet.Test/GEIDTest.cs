using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using System.Security.Cryptography;

namespace Neo4jRestNet.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class GEIDTest
	{

		[TestMethod]
		public void NewGEIDFromLong()
		{
			GEID geid = new GEID(123);

			Assert.AreEqual(123, (long)geid);
		}

		[TestMethod]
		public void NewGEIDFromString()
		{
			GEID geid = new GEID(123);
			string strGEID = geid.ToString();

			GEID impGEID = new GEID(strGEID);

			Assert.AreEqual(strGEID, impGEID.ToString());
			Assert.AreEqual(strGEID, impGEID.ToString());
		}

		[TestMethod]
		public void TryParseValid()
		{
			GEID geid = new GEID(123);
			string strGEID = geid.ToString();

			GEID tryGEID;
			Assert.IsTrue(GEID.TryParse(strGEID, out tryGEID));
			Assert.AreEqual(geid, tryGEID);
		}

		[TestMethod]
		public void TryParseNotValid()
		{
			string strGEID = "SomeBadValue";

			GEID tryGEID;
			Assert.IsFalse(GEID.TryParse(strGEID, out tryGEID));
		}

		[TestMethod]
		[ExpectedException(typeof(CryptographicException), "Length of the data to decrypt is invalid")]
		public void ImplicitBadStringValue()
		{
			GEID geid = "SomeBadValue";
		}

		[TestMethod]
		public void Equality()
		{
			GEID geid1 = new GEID(123);
			GEID geid2 = new GEID(123);
			GEID geid3 = new GEID(321);

			Assert.IsTrue(geid1 == geid2);
			Assert.IsTrue(geid1.ToString() == geid2.ToString());
			Assert.IsTrue(geid1 == geid2.ToString());
			Assert.IsTrue(geid1.ToString() == geid2); 

			Assert.IsFalse(geid1 == geid3);
			Assert.IsFalse(geid1.ToString() == geid3.ToString());
			Assert.IsFalse(geid1 == geid3.ToString());
			Assert.IsFalse(geid1.ToString() == geid3);
		}

		[TestMethod]
		public void Inequality()
		{
			GEID geid1 = new GEID(123);
			GEID geid2 = new GEID(321);
			GEID geid3 = new GEID(123);

			Assert.IsTrue(geid1 != geid2);
			Assert.IsTrue(geid1.ToString() != geid2.ToString());
			Assert.IsTrue(geid1 != geid2.ToString());
			Assert.IsTrue(geid1.ToString() != geid2);

			Assert.IsFalse(geid1 != geid3);
			Assert.IsFalse(geid1.ToString() != geid3.ToString());
			Assert.IsFalse(geid1 != geid3.ToString());
			Assert.IsFalse(geid1.ToString() != geid3);
		}
		
		[TestMethod]
		public void NullValue()
		{
			GEID geidNull = GEID.Null ;
			GEID geid = new GEID(123);

			Assert.IsTrue(geidNull == GEID.Null);
			Assert.IsFalse(geidNull != GEID.Null);

			Assert.IsFalse(geid == GEID.Null);
			Assert.IsTrue(geid != GEID.Null);
		}

	}
}
