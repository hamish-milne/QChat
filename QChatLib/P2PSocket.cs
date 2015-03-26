using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace QChatLib
{
	public delegate void ReceiveMessage(IPAddress ip, byte[] buffer);

	public class P2PSocket
	{
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		IPEndPoint host;
		int port = 8000;
		int attempts = 5;
		int waitTime = 100;

		void Run()
		{

		}

		public P2PSocket()
		{
			host = new IPEndPoint(IPAddress.Any, port);
		}
	}
}
