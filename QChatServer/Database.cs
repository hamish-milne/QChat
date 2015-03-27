using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
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

	public class Database
	{
		IDbConnection connection;
		HashAlgorithm hash;
		byte[] buffer = new byte[0x200];

		const string stringType = "VARCHAR(255) NOT NULL";
		const string userTable = "Users";
		const string _username = "Username";
		const string _password = "Password";
		const string _permissions = "Permissions";

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
			contactTable + " WHERE (" + _sender + "@=" + _username + " OR " +
			_recipient + "@=" + _username + ") AND " + _message + " IS NULL)";
		
		
		public Database(IDbConnection connection)
		{
			this.connection = connection;
			hash = SHA256.Create();
		}

		public string GetPasswordHash(string username, string password)
		{
			hash.Clear();
			var numUser = Util.GetBytes(username, buffer, 0);
			var numPass = Util.GetBytes(password, buffer, numUser);
			var hashBytes = hash.ComputeHash(buffer, 0, numUser + numPass);
			return Convert.ToBase64String(hashBytes);
		}

		object ScalarQuery(string query, string username, string password)
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

		public bool AuthenticateUser(string username, string password)
		{
			return (int)ScalarQuery(checkPassword, username,
				GetPasswordHash(username, password)) > 0;
		}

		bool AddUpdate(string query, string username, string password)
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

		public bool AddUser(string username, string password)
		{
			return AddUpdate(insertText, username, password);
		}

		public bool SetPassword(string username, string password)
		{
			return AddUpdate(updateText, username, password);
		}

		public int GetPermissions(string username)
		{
			var value = (int?)ScalarQuery(permissionText, username, null);
			if (!value.HasValue)
				throw new ArgumentException("Username doesn't exist");
			return value.Value;
		}

		public bool HasContact(string userA, string userB)
		{
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

		public IList<Contact> GetContacts(string username)
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
					ret.Add(new Contact(name, state, time, message));
				}
			}
			return ret;
		}

		public bool UserExists(string username)
		{
			return (ScalarQuery(permissionText, username, null) != null);
		}

		public int CountContacts(string username)
		{
			return (int)ScalarQuery(countContacts, username, null);
		}

		public int CountRequests(string username)
		{
			return (int)ScalarQuery(countRequests, username, null);
		}

		public bool SendContact(string from, string to, string message)
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
