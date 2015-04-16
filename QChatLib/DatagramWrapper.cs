using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace QChatLib
{
	public class DatagramWrapper : MemoryStream
	{
		Socket socket;
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

		public DatagramWrapper(Socket socket)
			: base(Util.DatagramSize)
		{
			if (socket == null)
				throw new ArgumentNullException("socket");
			this.socket = socket;
		}
		
		public virtual void Send(IPAddress ip, int port)
		{
			if(Length > Util.DatagramSize)
				throw new IOException("Message is too long");
			remoteEP.Address = ip;
			remoteEP.Port = port;
			socket.SendTo(GetBuffer(), (int)Length, SocketFlags.None, remoteEP);
		}

		public virtual void Receive(out IPAddress ip, out int port)
		{
			Flush();
			EndPoint ep = remoteEP;
			socket.ReceiveFrom(GetBuffer(), ref ep);
			remoteEP = (IPEndPoint)ep;
			ip = remoteEP.Address;
			port = remoteEP.Port;
		}
	}
}
