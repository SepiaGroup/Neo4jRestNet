using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using System.Security.Cryptography;
using Neo4jRestNet.GremlinPlugin;

namespace Neo4jRestNet.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class EncryptedIDTest
	{

		[TestMethod]
		public void NewGEIDFromLong()
		{
			EncryptId geid = new EncryptId(123);

			Assert.AreEqual(123, (long)geid);
		}

		[TestMethod]
		public void NewGEIDFromString()
		{
			EncryptId geid = new EncryptId(123);
			string strGEID = geid.ToString();

			EncryptId impGEID = new EncryptId(strGEID);

			Assert.AreEqual(strGEID, impGEID.ToString());
			Assert.AreEqual(strGEID, impGEID.ToString());
		}

		[TestMethod]
		public void TryParseValid()
		{
			EncryptId geid = new EncryptId(123);
			string strGEID = geid.ToString();

			EncryptId tryGEID;
			Assert.IsTrue(EncryptId.TryParse(strGEID, out tryGEID));
			Assert.AreEqual(geid, tryGEID);
		}

		[TestMethod]
		public void TryParseNotValid()
		{
			string strGEID = "SomeBadValue";

			EncryptId tryGEID;
			Assert.IsFalse(EncryptId.TryParse(strGEID, out tryGEID));
		}

		[TestMethod]
		[ExpectedException(typeof(CryptographicException), "Length of the data to decrypt is invalid")]
		public void ImplicitBadStringValue()
		{
			EncryptId geid = "SomeBadValue";
		}

		[TestMethod]
		public void Equality()
		{
			EncryptId geid1 = new EncryptId(123);
			EncryptId geid2 = new EncryptId(123);
			EncryptId geid3 = new EncryptId(321);

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
			EncryptId geid1 = new EncryptId(123);
			EncryptId geid2 = new EncryptId(321);
			EncryptId geid3 = new EncryptId(123);

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
			EncryptId geidNull = EncryptId.Null;
			EncryptId geid = new EncryptId(123);

			Assert.IsTrue(geidNull == EncryptId.Null);
			Assert.IsFalse(geidNull != EncryptId.Null);

			Assert.IsFalse(geid == EncryptId.Null);
			Assert.IsTrue(geid != EncryptId.Null);
		}
	}
}
