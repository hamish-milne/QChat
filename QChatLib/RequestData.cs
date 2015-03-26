using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
    public abstract class Request
    {
		public abstract RequestType RequestType { get; }

		public virtual void Send(StreamWriter stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.Write((byte)RequestType);
		}
    }

	public class SingleRequest : Request
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

	public class KeepAlive : Request
	{
		public override RequestType RequestType
		{
			get { return RequestType.KeepAlive; }
		}
	}

	public class Close : Request
	{
		public override RequestType RequestType
		{
			get { return RequestType.Close; }
		}
	}

	public class Logout : Request
	{
		public override RequestType RequestType
		{
			get { return RequestType.Logout; }
		}
	}

	public class GetNotification : Request
	{
		public override RequestType RequestType
		{
			get { return RequestType.GetNotification; }
		}
	}

	public abstract class UsernameRequest : Request
	{
		string username;

		public string Username
		{
			get { return username; }
		}

		public override void Send(StreamWriter stream)
		{
			base.Send(stream);
			Util.WriteShortString(stream, Username);
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

		public override void Send(StreamWriter stream)
		{
			base.Send(stream);
			Util.WriteShortString(stream, Password);
		}

		public LoginRequest(string username, string password)
			: base(username)
		{
			this.password = password;
		}
	}

	public class GetIP : UsernameRequest
	{
		public override RequestType RequestType
		{
			get { return RequestType.GetIP; }
		}

		public GetIP(string username)
			: base(username)
		{
		}
	}

	public class SendContact : UsernameRequest
	{
		public override RequestType RequestType
		{
			get { return RequestType.SendContact; }
		}

		public SendContact(string username)
			: base(username)
		{
		}
	}

	public class AcceptContact : UsernameRequest
	{
		public override RequestType RequestType
		{
			get { return RequestType.AcceptContact; }
		}

		public AcceptContact(string username)
			: base(username)
		{
		}
	}

	public class RejectContact : UsernameRequest
	{
		public override RequestType RequestType
		{
			get { return RequestType.RejectContact; }
		}

		public RejectContact(string username)
			: base(username)
		{
		}
	}
}
