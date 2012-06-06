using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jRestNet.Core;
using System.Security.Cryptography;

namespace Neo4jRestNet.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class EncryptedIDTest
	{

		[TestMethod]
		public void GetNode()
		{
			var node = Node.GetNode(0);

			Assert.AreEqual(node.Id, 0);
		}

		[TestMethod]
		public void NewGEIDFromLong()
		{
			var geid = new EncryptId(123);

			Assert.AreEqual(123, (long)geid);
		}

		[TestMethod]
		public void NewGEIDFromString()
		{
			var geid = new EncryptId(123);
			string strGEID = geid.ToString();

			var impGEID = new EncryptId(strGEID);

			Assert.AreEqual(strGEID, impGEID.ToString());
			Assert.AreEqual(strGEID, impGEID.ToString());
		}

		[TestMethod]
		public void TryParseValid()
		{
			var geid = new EncryptId(123);
			var strGEID = geid.ToString();

			EncryptId tryGEID;
			Assert.IsTrue(EncryptId.TryParse(strGEID, out tryGEID));
			Assert.AreEqual(geid, tryGEID);
		}

		[TestMethod]
		public void TryParseNotValid()
		{
			var strGEID = "SomeBadValue";

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
			var geid1 = new EncryptId(123);
			var geid2 = new EncryptId(123);
			var geid3 = new EncryptId(321);

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
			var geid1 = new EncryptId(123);
			var geid2 = new EncryptId(321);
			var geid3 = new EncryptId(123);

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
			var geidNull = EncryptId.Null;
			var geid = new EncryptId(123);

			Assert.IsTrue(geidNull == EncryptId.Null);
			Assert.IsFalse(geidNull != EncryptId.Null);

			Assert.IsFalse(geid == EncryptId.Null);
			Assert.IsTrue(geid != EncryptId.Null);
		}
	}
}
