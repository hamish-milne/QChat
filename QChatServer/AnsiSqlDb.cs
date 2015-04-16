using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using QChatLib;

namespace QChatServer
{
	public enum DatabaseError
	{
		Success,
		InvalidUsername,
		InvalidData,
		EntryAlreadyExists,
	}

	public class AnsiSqlDb : Database
	{
		IDbConnection connection;
		HashAlgorithm hash;
		SymmetricAlgorithm encryption;
		byte[] buffer = new byte[0x200];

		const string stringType = "VARCHAR(255) NOT NULL";
		const string userTable = "Users";
		const string _username = "Username";
		const string _password = "Password";
		const string _permissions = "Permissions";
		const string _publicKey = "PublicKey";
		const string _privateKey = "PrivateKey";

		const string checkPassword = "SELECT COUNT(*) FROM (SELECT * FROM " +
			userTable + " WHERE " + _username + "=@" + _username + " AND " +
			_password + "=@" + _password + "))";
		const string updateText = "UPDATE " + userTable + " SET " + _password
			+ "=@" + _password + " WHERE " + _username + "=@" + _username;
		const string insertText = "INSERT INTO " + userTable + " (" +
			_username + "," + _password + ") VALUES (@" + _username + ",@" +
			_password + ")";
		const string permissionText = "SELECT " + _permissions + " FROM " +
			userTable + " WHERE " + _username + "=@" + _username;
		const string getPublicKey = "SELECT " + _publicKey + " FROM " +
			userTable + " WHERE " + _username + "=@" + _username;
		const string getPrivateKey = "SELECT " + _privateKey + " FROM " +
			userTable + " WHERE " + _username + "=@" + _username;

		const string contactTable = "Contacts";
		const string _sender = "Sender";
		const string _recipient = "Recipient";
		const string _message = "Message";
		const string _time = "Time";
		const string _hidden = "Hidden";

		const string userTableCreate = "CREATE TABLE " + userTable + " (" +
			_username + " " + stringType + "," +
			_password + " CHAR(344) NOT NULL)";
		const string contactTableCreate = "CREATE TABLE " + contactTable + " (" +
			_sender + " " + stringType + "," +
			_recipient + " " + stringType + "," +
			_message + " VARCHAR," +
			_time + " DATETIME NOT NULL," +
			"CONSTRAINT u_SenderRecipient UNIQUE (" + _sender + "," + _recipient + ")," +
			"CONSTRAINT fk_Sender FOREIGN KEY " + _sender +
				" REFERENCES " + userTable + "(" + _username + ")," +
			"CONSTRAINT fk_Recipient FOREIGN KEY " + _recipient +
				" REFERENCES " + userTable + "(" + _username + "))";

		const string hasContact = "SELECT COUNT(*) FROM (SELECT * FROM " +
			contactTable + " WHERE (" + _sender + "=@" + _sender + " AND " +
			_recipient + "=@" + _recipient + ") OR (" + _sender + "=@" +
			_recipient + " AND " + _recipient + "=@" + _sender + ") AND " +
			_message + " IS NOT NULL)";
		const string getContacts = "SELECT " + _sender + "," + _recipient + ","
			+ _message + "," + _time + " FROM " + contactTable + " WHERE " +
			_recipient + "=@" + _username + " OR " + _sender + "=@" + _username;
		const string sendRequest = "INSERT INTO " + contactTable + " (" +
			_sender + "," + _recipient + "," + _message + "," + _time +
			") VALUES (@" +
			_sender + ",@" + _recipient + ",@" + _message + ",@" + _time + ")";
		const string countRequests = "SELECT COUNT(*) FROM (SELECT * FROM " +
			contactTable + " WHERE " + _sender + "=@" + _username + " AND " +
			_message + " IS NOT NULL)";
		const string countContacts = "SELECT COUNT(*) FROM (SELECT * FROM " +
			contactTable + " WHERE (" + _sender + "=@" + _username + " OR " +
			_recipient + "=@" + _username + ") AND " + _message + " IS NULL)";

		public AnsiSqlDb(IDbConnection connection)
		{
			this.connection = connection;
			hash = SHA256.Create();
		}

		public virtual SymmetricAlgorithm Encryption
		{
			get
			{
				if (encryption == null)
					encryption = new AesManaged();
				return encryption;
			}
		}

		public virtual HashAlgorithm Hash
		{
			get
			{
				if (hash == null)
					hash = new SHA256Managed();
				return hash;
			}
		}

		protected void InitEncryption(string username, string password)
		{
			if (username == null)
				throw new ArgumentNullException("username");
			if (password == null)
				throw new ArgumentNullException("password");
			var bytes = Encoding.UTF8.GetBytes(password);
			if (bytes.Length != Encryption.KeySize / 8)
				Array.Resize(ref bytes, Encryption.KeySize / 8);
			Encryption.Key = bytes;
			bytes = Encoding.UTF8.GetBytes(username);
			if (bytes.Length != Encryption.BlockSize / 8)
				Array.Resize(ref bytes, Encryption.BlockSize / 8);
			Encryption.IV = bytes;
		}

		protected byte[] EncryptBytes(string username, string password, byte[] input)
		{
			InitEncryption(username, password);
			byte[] ret;
			using (var encryptor = Encryption.CreateEncryptor())
			using (var memStream = new MemoryStream(buffer))
			using (var cryptoStream = new CryptoStream(memStream, encryptor,
				CryptoStreamMode.Write))
			{
				cryptoStream.Write(input, 0, input.Length);
				ret = memStream.ToArray();
			}
			return ret;
		}

		protected byte[] DecryptBytes(string username, string password, byte[] input)
		{
			InitEncryption(username, password);
			byte[] ret;
			using (var decryptor = Encryption.CreateDecryptor())
			using (var memStream = new MemoryStream(input))
			using (var cryptoStream = new CryptoStream(memStream, decryptor,
				CryptoStreamMode.Read))
			{
				ret = new byte[cryptoStream.Length];
				cryptoStream.Read(ret, 0, (int)cryptoStream.Length);
			}
			return ret;
		}

		protected virtual string GetPasswordHash(string username, string password)
		{
			hash.Clear();
			var numUser = Encoding.UTF8.GetBytes(username, 0, username.Length, buffer, 0);
			var numPass = Encoding.UTF8.GetBytes(password, 0, password.Length, buffer, numUser);
			var hashBytes = hash.ComputeHash(buffer, 0, numUser + numPass);
			return Convert.ToBase64String(hashBytes);
		}

		protected object ScalarQuery(string query, string username, string password)
		{
			object ret;
			using (var command = connection.CreateCommand())
			{
				command.CommandText = query;
				var param = command.CreateParameter();
				param.ParameterName = _username;
				param.Value = username;
				if(password != null)
				{
					param = command.CreateParameter();
					param.ParameterName = _password;
					param.Value = password;
				}
				ret = command.ExecuteScalar();
			}
			return ret;
		}

		public override bool AuthenticateUser(string username, string password)
		{
			return (int)ScalarQuery(checkPassword, username,
				GetPasswordHash(username, password)) > 0;
		}

		public override byte[] GetPublicKey(string username)
		{
			var str = ScalarQuery(getPublicKey, username, null) as string;
			if(str == null)
				return null;
			return Convert.FromBase64String(str);
		}

		public override byte[] GetPrivateKey(string username, string password)
		{
			var str = ScalarQuery(getPublicKey, username, null) as string;
			if (str == null)
				return null;
			var cipher = Convert.FromBase64String(str);
			return DecryptBytes(username, password, cipher);
		}

		protected bool AddUpdate(string query, string username, string password)
		{
			int result;
			using(var command = connection.CreateCommand())
			{
				command.CommandText = query;
				var param = command.CreateParameter();
				param.ParameterName = _password;
				param.Value = GetPasswordHash(username, password);
				param = command.CreateParameter();
				param.ParameterName = _username;
				param.Value = username;
				result = command.ExecuteNonQuery();
			}
			return (result > 0);
		}

		public override bool AddUser(string username, string password)
		{
			return AddUpdate(insertText, username, password);
		}

		public override bool SetPassword(string username, string password)
		{
			return AddUpdate(updateText, username, password);
		}

		public override int GetPermissions(string username)
		{
			var value = (int?)ScalarQuery(permissionText, username, null);
			if (!value.HasValue)
				throw new ArgumentException("Username doesn't exist");
			return value.Value;
		}

		public override bool HasContact(string userA, string userB)
		{
			if (userA == userB)
				return true;
			bool ret;
			using(var command = connection.CreateCommand())
			{
				command.CommandText = hasContact;
				var param = command.CreateParameter();
				param.ParameterName = _sender;
				param.Value = userA;
				param = command.CreateParameter();
				param.ParameterName = _recipient;
				param.Value = userB;
				ret = ((int)command.ExecuteScalar() > 0);
			}
			return ret;
		}

		public override IList<Contact> GetContacts(string username)
		{
			List<Contact> ret;
			using(var command = connection.CreateCommand())
			{
				command.CommandText = getContacts;
				var param = command.CreateParameter();
				param.ParameterName = _username;
				param.Value = username;
				var reader = command.ExecuteReader();
				ret = new List<Contact>();
				while(reader.Read())
				{
					var sender = reader.GetString(0);
					var recipient = reader.GetString(1);
					var message = reader.IsDBNull(2) ? null : reader.GetString(2);
					var time = reader.GetDateTime(3);
					ContactState state;
					string name;
					if(message == null)
					{
						state = ContactState.Accepted;
						name = (username == sender) ? recipient : sender;
					} else if(username == sender)
					{
						state = ContactState.Sent;
						name = recipient;
					} else
					{
						state = ContactState.Received;
						name = sender;
					}
					ret.Add(new Contact(state, name, time, message));
				}
			}
			return ret;
		}

		public override bool UserExists(string username)
		{
			return (ScalarQuery(permissionText, username, null) != null);
		}

		public override int CountContacts(string username)
		{
			return (int)ScalarQuery(countContacts, username, null);
		}

		public override int CountRequests(string username)
		{
			return (int)ScalarQuery(countRequests, username, null);
		}

		public override bool SendContact(string from, string to, string message)
		{
			int result;
			using(var command = connection.CreateCommand())
			{
				command.CommandText = sendRequest;
				var param = command.CreateParameter();
				param.ParameterName = _sender;
				param.Value = from;
				param = command.CreateParameter();
				param.ParameterName = _recipient;
				param.Value = to;
				param = command.CreateParameter();
				param.ParameterName = _message;
				param.Value = message;
				param = command.CreateParameter();
				param.ParameterName = _time;
				param.Value = DateTime.UtcNow;
				result = command.ExecuteNonQuery();
			}
			return (result > 0);
		}
	}
}
