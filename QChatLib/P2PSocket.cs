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
		Socket socket;
		IPEndPoint host;
		int attempts = 5;
		int waitTime = 100;
		readonly byte[] buf = new byte[0x10000];

		void Run()
		{

		}

		public P2PSocket(bool ipv6)
		{
			host = new IPEndPoint(ipv6 ? IPAddress.IPv6Any : IPAddress.Any, 0);
			socket = new Socket(ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Bind(host);
			socket.ReceiveTimeout = waitTime;
		}

		public virtual void HolePunch(EndPoint remoteEP, EndPoint server, ulong sessionKey)
		{
			lock (buf)
			{
				for (int i = 0; i < sizeof(ulong); i++)
					buf[i] = (byte)(sessionKey >> (sizeof(ulong) - 1 - i));
				var attempts = this.attempts;
				while (attempts > 0)
				{
					try
					{
						socket.SendTo(buf, server);
						if (socket.ReceiveFrom(buf, ref server) == 0)
							return;
					} catch (SocketException e)
					{
						if (e.SocketErrorCode != SocketError.TimedOut)
							throw;
					}
					attempts--;
				}
			}
			throw new Exception("Unable to perform hole punching");
		}
	}
}
