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
	public class EncryptId : IEquatable<EncryptId>
	{
		#region Encryption class

		private static class Encryption
		{
			private static readonly byte[] SecretKey = System.Text.Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["EncryptIdKey"].ToString());
			private static readonly byte[] SecretIV = System.Text.Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["EncryptIdIV"].ToString()); 


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

		long? _id = null;

		public static readonly EncryptId Null = new EncryptId((long?)null, true);

		private EncryptId(long? id, bool init)
		{
			this._id = id;
		}

		public EncryptId(long id)
		{
			this._id = id;
		}

		public EncryptId(string encryptedId)
		{
			if (encryptedId == null || encryptedId.Trim() == string.Empty)
				this._id = null;
			else
				this._id = long.Parse(Encryption.Decrypt(encryptedId));
		}

		public static bool TryParse(string InEncryptedId, out EncryptId OutEncryptedId)
		{
			if (string.IsNullOrWhiteSpace(InEncryptedId)) 
			{
				OutEncryptedId = null;
				return false;
			}

			try
			{
				OutEncryptedId = InEncryptedId;
				return true;
			}
			catch
			{
				OutEncryptedId = null;
				return false;
			}
		}

		public static implicit operator EncryptId(long id)
		{
			return new EncryptId(id);
		}

		public static implicit operator EncryptId(string encryptedId)
		{
			return new EncryptId(encryptedId);
		}

		public static implicit operator string(EncryptId encryptedId)
		{
			if (encryptedId == null)
				return null;

			return encryptedId.ToString();
		}

		public static explicit operator long?(EncryptId encryptedId)
		{
			if (encryptedId == null)
				return null;

			return encryptedId._id;
		}

		public override string ToString()
		{
			if (_id == null)
				return null;

			return Encryption.Encrypt(_id.ToString());
		}

		#region IEquatable<EncryptId> Members

		public bool Equals(EncryptId other)
		{
			if (ReferenceEquals(other, null))
				return false;
			else if (ReferenceEquals(this, other))
				return true;
			else if (this._id == null && other._id == null)
				return true;
			else if (this._id == null || other._id == null)
				return false;
			else if (this._id.Value == other._id.Value)
				return true;

			return false;
		}

		public override bool Equals(Object obj)
		{
			if (obj == null) return base.Equals(obj);

			if (!(obj is EncryptId))
				throw new InvalidCastException("The 'obj' argument is not a EncryptId object.");
			else
				return Equals(obj as EncryptId);
		}

		public override int GetHashCode()
		{
			return this._id.GetHashCode();
		}

		public static bool operator ==(EncryptId id1, EncryptId id2)
		{
			if (ReferenceEquals(id1, null) && ReferenceEquals(id2, null))
				return true;

			return ReferenceEquals(id1, null) ? false : id1.Equals(id2);
		}

		public static bool operator !=(EncryptId id1, EncryptId id2)
		{
			if (ReferenceEquals(id1, null) && ReferenceEquals(id2, null))
				return false;

			return !(ReferenceEquals(id1, null) ? false : id1.Equals(id2));
		}

		#endregion
	}
}
