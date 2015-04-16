using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
	public abstract class Notification : Message<NotificationType>
	{
		
	}

	public class ContactRequestBase : Notification
	{
		string username;

		public string Username
		{
			get { return username; }
		}

		protected static void Send(StreamWrapper stream, NotificationType type, string username)
		{
			Send(stream, (byte)type);
			stream.WriteString(username, 1);
		}

		public ContactRequestBase(string username)
		{
			this.username = username;
		}
	}

	public class ContactRequest : ContactRequestBase
	{
		public const NotificationType Code = NotificationType.ContactRequest;

		public static void Send(StreamWrapper stream, string username)
		{
			Send(stream, Code, username);
		}

		public ContactRequest(string username)
			: base(username)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) =>
				new ContactRequest(stream.ReadString(1)));
		}
	}

	public class ContactRequestAccepted : ContactRequestBase
	{
		public const NotificationType Code = NotificationType.ContactRequestAccepted;

		public static void Send(StreamWrapper stream, string username)
		{
			Send(stream, Code, username);
		}

		public ContactRequestAccepted(string username)
			: base(username)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) =>
				new ContactRequestAccepted(stream.ReadString(1)));
		}
	}

	public class ContactRequestRejected : ContactRequestBase
	{
		public const NotificationType Code = NotificationType.ContactRequestRejected;

		public static void Send(StreamWrapper stream, string username)
		{
			Send(stream, Code, username);
		}

		public ContactRequestRejected(string username)
			: base(username)
		{
		}

		[Initialize]
		static void Init()
		{
			AssignReader((byte)Code, (stream) =>
				new ContactRequestRejected(stream.ReadString(1)));
		}
	}
}
