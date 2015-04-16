using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace QChatLib.ServerResponses
{
	public class Success : Message<ServerResponse>
	{
		public const ServerResponse Code = ServerResponse.Success;

		public override ServerResponse Type
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
			AssignReader((byte)Code, (stream) => new Success());
		}
	}

	public class LoginSuccess : Message<ServerResponse>
	{
		public const ServerResponse Code = ServerResponse.LoginSuccess;

		ulong sessionKey;

		public virtual ulong SessionKey
		{
			get { return sessionKey; }
		}

		public override ServerResponse Type
		{
			get { return Code; }
		}

		public LoginSuccess(ulong sessionKey)
		{
			this.sessionKey = sessionKey;
		}

		public static void Send(StreamWrapper stream, ulong sessionKey)
		{
			Send(stream, (byte)Code);
			stream.Write(sessionKey);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)ServerResponse.LoginSuccess, (stream) => new LoginSuccess(stream.ReadULong()));
		}
	}

	public abstract class TextResponse : Message<ServerResponse>
	{
		string message;

		public virtual string Message
		{
			get { return message; }
		}

		public TextResponse(string message)
		{
			this.message = message;
		}

		protected static void Send(StreamWrapper stream, ServerResponse code, string message)
		{
			Send(stream, (byte)code);
			stream.WriteString(message, 2);
		}
	}

	public class Fail : TextResponse
	{
		public const ServerResponse Code = ServerResponse.Fail;

		public override ServerResponse Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream, string message)
		{
			Send(stream, Code, message);
		}

		public Fail(string message)
			: base(message)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new Fail(stream.ReadString(2)));
		}
	}

	public class NotPermitted : TextResponse
	{
		public const ServerResponse Code = ServerResponse.NotPermitted;

		public override ServerResponse Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream, string message)
		{
			Send(stream, Code, message);
		}

		public NotPermitted(string message)
			: base(message)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new NotPermitted(stream.ReadString(2)));
		}
	}

	public class InvalidRequest : TextResponse
	{
		public const ServerResponse Code = ServerResponse.InvalidRequest;

		public override ServerResponse Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream, string message)
		{
			Send(stream, Code, message);
		}

		public InvalidRequest(string message)
			: base(message)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new InvalidRequest(stream.ReadString(2)));
		}
	}

	public class AuthenticationError : TextResponse
	{
		public const ServerResponse Code = ServerResponse.AuthenticationError;

		public override ServerResponse Type
		{
			get { return Code; }
		}

		public static void Send(StreamWrapper stream, string message)
		{
			Send(stream, Code, message);
		}

		public AuthenticationError(string message)
			: base(message)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) => new AuthenticationError(stream.ReadString(2)));
		}
	}

	public class SendContacts : Message<ServerResponse>
	{
		public const ServerResponse Code = ServerResponse.IncomingContacts;

		IList<Contact> contacts;

		public override ServerResponse Type
		{
			get { return Code; }
		}

		public SendContacts(IList<Contact> contacts)
		{
			this.contacts = contacts;
		}

		public static void Send(StreamWrapper stream, IList<Contact> contacts)
		{
			Send(stream, (byte)Code);
			stream.Write((ushort)(contacts == null ? 0 : contacts.Count));
			if(contacts != null)
				for(int i = 0; i < contacts.Count; i++)
				{
					var c = contacts[i];
					stream.Write((byte)c.State);
					stream.WriteString(c.Name, 1);
					if(c.State != ContactState.Accepted)
					{
						stream.Write(c.RequestSent.ToFileTimeUtc());
						stream.WriteString(c.Message, 2);
					}
				}
		}

		static Message<ServerResponse> Receive(StreamWrapper stream)
		{
			var count = stream.ReadUShort();
			var list = new Contact[count];
			for (int i = 0; i < count; i++)
			{
				var state = (ContactState)stream.ReadByte();
				var name = stream.ReadString(1);
				Contact contact;
				if (state != ContactState.Accepted)
				{
					contact = new Contact(state, name,
						DateTime.FromFileTimeUtc(stream.ReadLong()),
						stream.ReadString(2));
				}
				else
				{
					contact = new Contact(state, name, default(DateTime), null);
				}
				list[i] = contact;
			}
			return new SendContacts(list);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, Receive);
		}
	}

	public class SendIPs : Message<ServerResponse>
	{
		public const ServerResponse Code = ServerResponse.IncomingIP;

		IList<ClientAddress> ips;

		public SendIPs(IList<ClientAddress> ips)
		{
			this.ips = ips;
		}

		public override ServerResponse Type
		{
			get { return ServerResponse.IncomingIP; }
		}

		public static void Send(StreamWrapper stream, IList<ClientAddress> ips)
		{
			Send(stream, (byte)Code);
			stream.Write(ips == null ? 0 : (ushort)ips.Count);
			if(ips != null)
				for(int i = 0; i < ips.Count; i++)
				{
					var ip = ips[i].Address;
					if (ip == null)
						stream.Write((byte)0);
					else
					{
						var bytes = ip.GetAddressBytes();
						var len = bytes.Length + sizeof(ushort);
						if (len > byte.MaxValue)
							throw new Exception("IP is too long");
						stream.Write((byte)len);
						stream.Write(bytes, 0, bytes.Length);
						stream.Write(ips[i].Port);
					}
				}
		}

		static Message<ServerResponse> Receive(StreamWrapper stream)
		{
			var ipCount = stream.ReadUShort();
			var ips = new ClientAddress[ipCount];
			byte[] buffer = null;
			for (int i = 0; i < ipCount; i++)
			{
				var byteCount = stream.ReadByte();
				if (byteCount <= sizeof(ushort))
				{
					while (byteCount > 0)
					{
						stream.ReadByte();
						byteCount--;
					}
					continue;
				}
				byteCount -= sizeof(ushort);
				if (buffer == null || buffer.Length != byteCount)
					buffer = new byte[byteCount];
				var ipBytes = stream.Read(buffer, 0, byteCount);
				var port = stream.ReadUShort();
				ips[i] = new ClientAddress(new IPAddress(buffer), port);
			}
			return new SendIPs(ips);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, Receive);
		}
	}

	public class IncomingKey : Message<ServerResponse>
	{
		public const ServerResponse Code = ServerResponse.IncomingKey;

		byte[] keyData;

		public virtual byte[] KeyData
		{
			get { return keyData; }
		}

		public override ServerResponse Type
		{
			get { return ServerResponse.IncomingKey; }
		}

		public IncomingKey(byte[] keyData)
		{
			this.keyData = keyData;
		}

		public static void Send(StreamWrapper stream, byte[] keyData)
		{
			Send(stream, (byte)Code);
			stream.Write((ushort)keyData.Length);
			stream.Write(keyData, 0, keyData.Length);
		}

		static Message<ServerResponse> Receive(StreamWrapper stream)
		{
			var length = stream.ReadByte();
			var keyData = new byte[length];
			stream.Read(keyData, 0, length);
			return new IncomingKey(keyData);
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, Receive);
		}
	}

}
