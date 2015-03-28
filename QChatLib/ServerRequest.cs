using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
    public abstract class ServerRequest
    {
		public abstract RequestType RequestType { get; }

		public static void Send(StreamWrapper stream, RequestType type)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.WriteByte((byte)type);
		}

		public static ServerRequest Receive(StreamWrapper stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			var type = (RequestType)stream.ReadByte();
			switch (type)
			{
				default:
					return new SingleRequest(type);
				case RequestType.GetIP:
				case RequestType.AcceptContact:
				case RequestType.RejectContact:
					return new SingleUsernameRequest(type, stream.ReadString(1));
				case RequestType.Login:
					return new LoginRequest(stream.ReadString(1), stream.ReadString(1));
				case RequestType.SendContact:
					return new ContactRequest(stream.ReadString(1), stream.ReadString(2))
			}
		}
    }

	public class SingleRequest : ServerRequest
	{
		RequestType requestType;

		public override RequestType RequestType
		{
			get { return requestType; }
		}

		public SingleRequest(RequestType requestType)
		{
			this.requestType = requestType;
		}
	}

	public class ContactRequest : UsernameRequest
	{
		string message;

		public string Message
		{
			get { return message; }
		}

		public static void Send(StreamWrapper stream, string username, string message)
		{
			Send(stream, RequestType.SendContact, username);
			stream.WriteString(message, 2);
		}

		public ContactRequest(string username, string message)
			: base(username)
		{
			this.message = message;
		}
	}

	public abstract class UsernameRequest : ServerRequest
	{
		string username;

		public string Username
		{
			get { return username; }
		}

		public static void Send(StreamWrapper stream, RequestType type, string username)
		{
			Send(stream, type);
			stream.WriteString(username, 1);
		}

		public UsernameRequest(string username)
		{
			this.username = username;
		}
	}

	public class SingleUsernameRequest : UsernameRequest
	{
		RequestType requestType;

		public override RequestType RequestType
		{
			get { return requestType; }
		}

		public SingleUsernameRequest(RequestType requestType, string username)
			: base(username)
		{
			this.requestType = requestType;
		}
	}

	public class LoginRequest : UsernameRequest
	{
		string password;

		public override RequestType RequestType
		{
			get { return RequestType.Login; }
		}

		public string Password
		{
			get { return password; }
		}

		public static void Send(StreamWrapper stream, string username, string password)
		{
			Send(stream, RequestType.Login, username);
			stream.WriteString(password, 1);
		}

		public LoginRequest(string username, string password)
			: base(username)
		{
			this.password = password;
		}
	}
}
