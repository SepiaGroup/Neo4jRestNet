using System;
using System.Text;
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
			private static readonly byte[] SecretKey = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["EncryptIdKey"]);
			private static readonly byte[] SecretIV = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["EncryptIdIV"]); 


			//Creates a symmetric RijndaelCipher encryptor object. 
			private static readonly RijndaelManaged Esp = new RijndaelManaged();

			//Creates a symmetric DES encryptor object.
			//DESCryptoServiceProvider esp = new DESCryptoServiceProvider();

			//Creates a symmetric AES decryptor object.
			//private static AesManaged esp = new AesManaged();

			//Creates a symmetric AES decryptor object.
			//private static RC2CryptoServiceProvider esp = new RC2CryptoServiceProvider();

			//private static ICryptoTransform Encryptor = esp.CreateEncryptor(SecretKey, SecretIV);
			//private static ICryptoTransform Decryptor = esp.CreateDecryptor(SecretKey, SecretIV);

			public static string Decrypt(string textToBeDecrypted)
			{
				Esp.Padding = PaddingMode.Zeros;
				ICryptoTransform decryptor = Esp.CreateDecryptor(SecretKey, SecretIV);

				byte[] encryptedData = Convert.FromBase64String(textToBeDecrypted.Replace('-', '/').Replace('_', '+')); // replace reserved HTTP charactors
				var memoryStream = new MemoryStream(encryptedData);

				//Defines the cryptographics stream for decryption.THe stream contains decrpted data
				var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

				var plainText = new byte[encryptedData.Length];
				int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);

				memoryStream.Close();
				cryptoStream.Close();

				//Converting to string
				return Encoding.Unicode.GetString(plainText, 0, decryptedCount);
			}

			public static string Encrypt(string textToBeEncrypted)
			{
				Esp.Padding = PaddingMode.Zeros;
				ICryptoTransform encryptor = Esp.CreateEncryptor(SecretKey, SecretIV);

				var plainText = Encoding.Unicode.GetBytes(textToBeEncrypted);
				var memoryStream = new MemoryStream();

				//Defines a stream that links data streams to cryptographic transformations
				var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
				cryptoStream.Write(plainText, 0, plainText.Length);

				//Writes the final state and clears the buffer
				cryptoStream.FlushFinalBlock();
				byte[] cipherBytes = memoryStream.ToArray();

				memoryStream.Close();
				cryptoStream.Close();

				return Convert.ToBase64String(cipherBytes).Replace('/', '-').Replace('+', '_');  // replace reserved HTTP charactors	
			}
		}
		#endregion

		long? _id;

		public static readonly EncryptId Null = new EncryptId(null, true);

		private EncryptId(long? id, bool init)
		{
			_id = id;
		}

		public EncryptId(long id)
		{
			_id = id;
		}

		public EncryptId(string encryptedId)
		{
			if (encryptedId == null || encryptedId.Trim() == string.Empty)
				_id = null;
			else
				_id = long.Parse(Encryption.Decrypt(encryptedId));
		}

		public static bool TryParse(string inEncryptedId, out EncryptId outEncryptedId)
		{
			if (string.IsNullOrWhiteSpace(inEncryptedId)) 
			{
				outEncryptedId = null;
				return false;
			}

			try
			{
				outEncryptedId = inEncryptedId;
				return true;
			}
			catch
			{
				outEncryptedId = null;
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
			return _id == null ? string.Empty : Encryption.Encrypt(_id.ToString());
		}

		#region IEquatable<EncryptId> Members

		public bool Equals(EncryptId other)
		{
			if (ReferenceEquals(other, null))
				return false;
			
			if (ReferenceEquals(this, other))
				return true;
			
			if (_id == null && other._id == null)
				return true;
			
			if (_id == null || other._id == null)
				return false;
			
			if (_id.Value == other._id.Value)
				return true;

			return false;
		}

		public override bool Equals(Object obj)
		{
			if (obj == null) return base.Equals(obj);

			if (!(obj is EncryptId))
			{
				throw new InvalidCastException("The 'obj' argument is not a EncryptId object.");
			}

			return Equals(obj as EncryptId);
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		public static bool operator ==(EncryptId id1, EncryptId id2)
		{
			if (ReferenceEquals(id1, null) && ReferenceEquals(id2, null))
				return true;

			return !ReferenceEquals(id1, null) && id1.Equals(id2);
		}

		public static bool operator !=(EncryptId id1, EncryptId id2)
		{
			if (ReferenceEquals(id1, null) && ReferenceEquals(id2, null))
				return false;

			return !(!ReferenceEquals(id1, null) && id1.Equals(id2));
		}

		#endregion
	}
}
