using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QChatLib;

namespace QChatServer
{
	public class MemoryDb : Database
	{
		class User
		{
			public string password;
			public int permissions;
			public byte[] privateKey, publicKey;

			public User(string password)
			{
				this.password = password;
			}
		}

		class ContactStore : IEquatable<ContactStore>
		{
			public string a, b;
			public ContactState state;
			public string message;
			public DateTime time;

			public bool Equals(ContactStore other)
			{
				if (other == null)
					return false;
				return (a == other.a && b == other.b)
					|| (a == other.b && b == other.a);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as ContactStore);
			}

			public override int GetHashCode()
			{
				int a = this.a == null ? 0 : this.a.GetHashCode();
				int b = this.b == null ? 0 : this.b.GetHashCode();
				return a ^ b;
			}

			public bool Contains(string user)
			{
				int hash = user.GetHashCode();
				return (a.GetHashCode() == hash || b.GetHashCode() == hash);
			}
		}

		Dictionary<string, User> users = new Dictionary<string, User>();

		HashSet<ContactStore> contacts = new HashSet<ContactStore>();

		public override bool AddUser(string username, string password)
		{
			if (users.ContainsKey(username))
				return false;
			users.Add(username, new User(password));
			return true;
		}

		public override bool AuthenticateUser(string username, string password)
		{
			User user;
			users.TryGetValue(username, out user);
			return (user != null && user.password == password);
		}

		public override int CountContacts(string username)
		{
			int count = 0;
			foreach (var pair in contacts)
				if (pair.Contains(username))
					count++;
			return count;
		}

		public override int CountRequests(string username)
		{
			throw new NotImplementedException();
		}

		public override IList<Contact> GetContacts(string username)
		{
			var ret = new List<Contact>();
			foreach (var item in contacts)
			{
				if(!item.Contains(username))
					continue;
				var other = (item.a == username) ? item.b : item.a;
				ret.Add(new Contact(item.state, other, item.time, item.message));
			}
			return ret;
		}

		public override int GetPermissions(string username)
		{
			return users[username].permissions;
		}

		public override byte[] GetPrivateKey(string username, string password)
		{
			User user;
			users.TryGetValue(username, out user);
			if (user == null || user.password != password)
				return null;
			return user.privateKey;
		}

		public override byte[] GetPublicKey(string username)
		{
			User user;
			users.TryGetValue(username, out user);
			if (user == null)
				return null;
			return user.privateKey;
		}

		static readonly ContactStore check = new ContactStore();
		public override bool HasContact(string userA, string userB)
		{
			check.a = userA;
			check.b = userB;
			return contacts.Contains(check);
		}

		public override bool SendContact(string from, string to, string message)
		{
			var toAdd = new ContactStore();
			toAdd.a = from;
			toAdd.b = to;
			toAdd.message = message;
			toAdd.time = DateTime.UtcNow;
			return contacts.Add(toAdd);
		}

		public override bool SetPassword(string username, string password)
		{
			if (password == null)
				throw new ArgumentNullException("password");
			users[username].password = password;
			return true;
		}

		public override bool UserExists(string username)
		{
			return users.ContainsKey(username);
		}
	}
}
