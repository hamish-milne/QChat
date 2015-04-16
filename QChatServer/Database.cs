using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QChatLib;

namespace QChatServer
{
	public abstract class Database
	{
		public abstract bool AuthenticateUser(string username, string password);
		public abstract byte[] GetPublicKey(string username);
		public abstract byte[] GetPrivateKey(string username, string password);
		public abstract bool AddUser(string username, string password);
		public abstract bool SetPassword(string username, string password);
		public abstract int GetPermissions(string username);
		public abstract bool HasContact(string userA, string userB);
		public abstract IList<Contact> GetContacts(string username);
		public abstract bool UserExists(string username);
		public abstract int CountContacts(string username);
		public abstract int CountRequests(string username);
		public abstract bool SendContact(string from, string to, string message);
	}
}
