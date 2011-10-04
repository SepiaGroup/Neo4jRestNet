using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;

namespace Neo4jRestNet.Core
{
	public class GEID : IEquatable<GEID>
	{
		#region Encryption class

		private static class Encryption
		{
			private static readonly byte[] SecretKey = System.Text.Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["GEIDKey"].ToString()); 
			private static readonly byte[] SecretIV =  System.Text.Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["GEIDIV"].ToString()); 


			//Creates a symmetric RijndaelCipher encryptor object. 
			private static RijndaelManaged esp = new RijndaelManaged();

			//Creates a symmetric DES encryptor object.
			//DESCryptoServiceProvider esp = new DESCryptoServiceProvider();

			//Creates a symmetric AES decryptor object.
			//private static AesManaged esp = new AesManaged();

			//Creates a symmetric AES decryptor object.
			//private static RC2CryptoServiceProvider esp = new RC2CryptoServiceProvider();

			//private static ICryptoTransform Encryptor = esp.CreateEncryptor(SecretKey, SecretIV);
			//private static ICryptoTransform Decryptor = esp.CreateDecryptor(SecretKey, SecretIV);

			public static string Decrypt(string TextToBeDecrypted)
			{
				esp.Padding = PaddingMode.Zeros;
				ICryptoTransform Decryptor = esp.CreateDecryptor(SecretKey, SecretIV);

				byte[] EncryptedData = Convert.FromBase64String(TextToBeDecrypted.Replace('-', '/').Replace('_', '+')); // replace reserved HTTP charactors
				MemoryStream memoryStream = new MemoryStream(EncryptedData);

				//Defines the cryptographics stream for decryption.THe stream contains decrpted data
				CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

				byte[] PlainText = new byte[EncryptedData.Length];
				int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

				memoryStream.Close();
				cryptoStream.Close();

				//Converting to string
				return Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
			}

			public static string Encrypt(string TextToBeEncrypted)
			{
				esp.Padding = PaddingMode.Zeros;
				ICryptoTransform Encryptor = esp.CreateEncryptor(SecretKey, SecretIV);

				byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToBeEncrypted);
				MemoryStream memoryStream = new MemoryStream();

				//Defines a stream that links data streams to cryptographic transformations
				CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
				cryptoStream.Write(PlainText, 0, PlainText.Length);

				//Writes the final state and clears the buffer
				cryptoStream.FlushFinalBlock();
				byte[] CipherBytes = memoryStream.ToArray();

				memoryStream.Close();
				cryptoStream.Close();

				return Convert.ToBase64String(CipherBytes).Replace('/', '-').Replace('+', '_');  // replace reserved HTTP charactors	
			}
		}
		#endregion

		long? _geid = null;

		public static readonly GEID Null = new GEID((long?)null, true);

		private GEID(long? geid, bool init)
		{
			this._geid = geid;
		}

		public GEID(long geid)
		{
			this._geid = geid;
		}

		public GEID(string geid)
		{
			if (geid == null || geid.Trim() == string.Empty)
				this._geid = null;
			else
				this._geid = long.Parse(Encryption.Decrypt(geid));
		}

		public static bool TryParse(string strGEID, out GEID outGEID)
		{
			if (string.IsNullOrWhiteSpace(strGEID))
			{
				outGEID = null;
				return false;
			}

			try
			{
				outGEID = strGEID;
				return true;
			}
			catch
			{
				outGEID = null;
				return false;
			}
		}

		public static implicit operator GEID(long geid)
		{
			return new GEID(geid);
		}

		public static implicit operator GEID(string geid)
		{
			return new GEID(geid);
		}

		public static implicit operator string(GEID geid)
		{
			if (geid == null)
				return null;

			return geid.ToString();
		}

		public static explicit operator long?(GEID geid)
		{
			if (geid == null)
				return null;

			return geid._geid;
		}

		public override string ToString()
		{
			if (_geid == null)
				return null;

			return Encryption.Encrypt(_geid.ToString());
		}

		#region IEquatable<GEID> Members

		public bool Equals(GEID other)
		{
			if (ReferenceEquals(other, null))
				return false;
			else if (ReferenceEquals(this, other))
				return true;
			else if (this._geid == null && other._geid == null)
				return true;
			else if (this._geid == null || other._geid == null)
				return false;
			else if (this._geid.Value == other._geid.Value)
				return true;

			return false;
		}

		public override bool Equals(Object obj)
		{
			if (obj == null) return base.Equals(obj);

			if (!(obj is GEID))
				throw new InvalidCastException("The 'obj' argument is not a GEID object.");
			else
				return Equals(obj as GEID);
		}

		public override int GetHashCode()
		{
			return this._geid.GetHashCode();
		}

		public static bool operator ==(GEID geid1, GEID geid2)
		{
			if (ReferenceEquals(geid1, null) && ReferenceEquals(geid2, null))
				return true;

			return ReferenceEquals(geid1, null) ? false : geid1.Equals(geid2);
		}

		public static bool operator !=(GEID geid1, GEID geid2)
		{
			if (ReferenceEquals(geid1, null) && ReferenceEquals(geid2, null))
				return false;

			return !(ReferenceEquals(geid1, null) ? false : geid1.Equals(geid2));
		}

		#endregion
	}
}
