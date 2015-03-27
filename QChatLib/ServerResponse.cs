using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace QChatLib
{
	public abstract class ServerResponse
	{
		string message;

		public abstract ResponseType ResponseType { get; }
		public abstract string Message { get; }

		public static void Success(StreamWriter stream)
		{
			Send(stream, ResponseType.Success, null);
		}

		public static void Send(StreamWriter stream, ResponseType type, string message)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.Write((byte)type);
			Util.WriteString(stream, message);
		}

		public ServerResponse(string message)
		{
			this.message = message;
		}
	}

	public class SendContacts : ServerResponse
	{
		IList<Contact> contacts;

		public override ResponseType ResponseType
		{
			get { return ResponseType.IncomingContacts; }
		}

		public SendContacts(IList<Contact> contacts) : base(null)
		{
			this.contacts = contacts;
		}

		public static void Send(StreamWriter stream, IList<Contact> contacts)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.Write((byte)ResponseType.IncomingContacts);
			stream.Write(contacts == null ? 0 : (ushort)contacts.Count);
			if(contacts != null)
				for(int i = 0; i < contacts.Count; i++)
				{
					var c = contacts[i];
					stream.Write((byte)c.State);
					Util.WriteShortString(stream, c.Name);
					if(c.State != ContactState.Accepted)
					{
						stream.Write(c.RequestSent.ToFileTimeUtc());
						Util.WriteString(stream, c.Message);
					}
				}
		}
	}

	public class SendIPs : ServerResponse
	{
		IList<IPAddress> ips;

		public SendIPs(IList<IPAddress> ips) : base(null)
		{
			this.ips = ips;
		}

		public override ResponseType ResponseType
		{
			get { return ResponseType.IncomingIP; }
		}

		public static void Send(StreamWriter stream, IList<IPAddress> ips)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.Write((byte)ResponseType.IncomingIP);
			stream.Write(ips == null ? 0 : (ushort)ips.Count);
			if(ips != null)
				for(int i = 0; i < ips.Count; i++)
				{
					var ip = ips[i];
					if (ip == null)
						stream.Write((byte)0);
					else
					{
						var bytes = ip.GetAddressBytes();
						if (bytes.Length > byte.MaxValue)
							throw new Exception("IP is too long");
						stream.Write((byte)bytes.Length);
						stream.BaseStream.Write(bytes, 0, bytes.Length);
					}
				}
		}
	}

}
