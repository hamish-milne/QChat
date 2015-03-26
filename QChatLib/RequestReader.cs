using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
	public static class RequestReader
	{
		static string ReadString(Stream stream)
		{
			var len = stream.ReadByte();
			if (len < 0)
				throw new IOException("End of stream");
			var buf = new byte[len];
			stream.Read(buf, 0, len);
			return Encoding.UTF8.GetString(buf);
		}

		public static Request Read(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			var type = (RequestType)stream.ReadByte();
			switch(type)
			{
				case RequestType.KeepAlive:
				case RequestType.Close:
				case RequestType.GetNotification:
				case RequestType.Logout:
					return new SingleRequest(type);
				case RequestType.GetIP:
				case RequestType.SendContact:
				case RequestType.AcceptContact:
				case RequestType.RejectContact:
					return new SingleUsernameRequest(type, ReadString(stream));
				case RequestType.Login:
					return new LoginRequest(ReadString(stream), ReadString(stream));
				default:
					throw new IOException("End of stream");
			}
		}
	}
}
