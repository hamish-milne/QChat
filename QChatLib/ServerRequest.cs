using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib.ServerRequests
{
	public class RequestMode : Message<ServerRequest>
	{
		public const ServerRequest Code = ServerRequest.RequestMode;

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream)
		{
			Send(stream, (byte)Code);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new RequestMode());
		}
	}

	public class NotifyMode : Message<ServerRequest>
	{
		public const ServerRequest Code = ServerRequest.NotifyMode;

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream)
		{
			Send(stream, (byte)Code);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new NotifyMode());
		}
	}

	public class Close : Message<ServerRequest>
	{
		public const ServerRequest Code = ServerRequest.Close;

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream)
		{
			Send(stream, (byte)Code);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new Close());
		}
	}

	public class Logout : Message<ServerRequest>
	{
		public const ServerRequest Code = ServerRequest.Logout;

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream)
		{
			Send(stream, (byte)Code);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new Logout());
		}
	}

	public abstract class UsernameRequest : Message<ServerRequest>
	{
		string username;

		public string Username
		{
			get { return username; }
		}

		protected static void Send(StreamWrapper stream, ServerRequest type, string username)
		{
			stream.Write((byte)type);
			stream.WriteString(username, 1);
		}

		public UsernameRequest(string username)
		{
			this.username = username;
		}
	}

	public class ContactRequest : UsernameRequest
	{
		public const ServerRequest Code = ServerRequest.SendContact;

		string message;

		public string Message
		{
			get { return message; }
		}

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream, string username, string message)
		{
			Send(stream, Code, username);
			stream.WriteString(message, 2);
		}

		public ContactRequest(string username, string message)
			: base(username)
		{
			this.message = message;
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)ServerRequest.SendContact, (stream) =>
				new ContactRequest(stream.ReadString(1), stream.ReadString(2)));
		}
	}

	public class GetIP : UsernameRequest
	{
		public const ServerRequest Code = ServerRequest.GetIP;

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream, string username)
		{
			Send(stream, Code, username);
		}

		public GetIP(string username)
			: base(username)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new GetIP(stream.ReadString(1)));
		}
	}

	public class GetPublicKey : UsernameRequest
	{
		public const ServerRequest Code = ServerRequest.GetPublicKey;

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream, string username)
		{
			Send(stream, Code, username);
		}

		public GetPublicKey(string username)
			: base(username)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new GetPublicKey(stream.ReadString(1)));
		}
	}

	public class Login : UsernameRequest
	{
		public const ServerRequest Code = ServerRequest.Login;

		string password;

		public override ServerRequest Type
		{
			get { return Code; }
		}

		public string Password
		{
			get { return password; }
		}

		public static void Send(StreamWrapper stream, string username, string password)
		{
			Send(stream, Code, username);
			stream.WriteString(password, 1);
		}

		public Login(string username, string password)
			: base(username)
		{
			this.password = password;
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) =>
				new Login(stream.ReadString(1), stream.ReadString(1)));
		}
	}
}
