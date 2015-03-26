using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
	public abstract class Notification
	{
		public abstract NotificationType NotificationType { get; }

		public virtual void Send(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.WriteByte((byte)NotificationType);
		}
	}

	public abstract class ContactNotification : Notification
	{
		string username;

		public override void Send(Stream stream)
		{
			var userBytes = Encoding.UTF8.GetBytes(username);
			if (userBytes.Length > 0xFF)
				throw new ArgumentException("Username too long");
			base.Send(stream);
			stream.Write(userBytes, 0, userBytes.Length);
		}
	}


}
