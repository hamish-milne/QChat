using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace QChatLib
{
	class ClientConnection
	{
		Socket socket;

		public ClientConnection()
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		}

		
	}
}
