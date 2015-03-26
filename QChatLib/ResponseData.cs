using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace QChatLib
{
	public abstract class ResponseData
	{
		string message;

		public abstract ResponseType ResponseType { get; }
		public abstract string Message { get; }

		public static void SendSingle(StreamWriter stream, ResponseType type, string message)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.Write((byte)type);
			Util.WriteString(stream, message);
		}

		public virtual void Send(StreamWriter stream)
		{
			SendSingle(stream, ResponseType, Message);
		}

		public ResponseData(string message)
		{
			this.message = message;
		}
	}

	public class SuccessResponse : ResponseData
	{
		public override ResponseType ResponseType
		{
			get { return ResponseType.Success; }
		}

		public SuccessResponse(string message) : base(message)
		{
		}
	}

	public class FailResponse : ResponseData
	{
		public override ResponseType ResponseType
		{
			get { return ResponseType.Fail; }
		}

		public FailResponse(string message) : base(message)
		{
		}
	}

	public class SendContacts : ResponseData
	{
		IList<Contact> contacts;

		public SendContacts(IList<Contact> contacts) : base(null)
		{
			this.contacts = contacts;
		}

		public override void Send(StreamWriter stream)
		{
			base.Send(stream);
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

	public class SendIPs : ResponseData
	{
		IList<IPAddress> ips;

		public SendIPs(IList<IPAddress> ips) : base(null)
		{
			this.ips = ips;
		}

		public override void Send(StreamWriter stream)
		{
			base.Send(stream);
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
