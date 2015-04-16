using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UdpCommand
{
	class Program
	{
		static void ReceiveThread(Socket socket)
		{
			var buf = new byte[0];
			var ep = new IPEndPoint(IPAddress.Any, 0);
			while(true)
			{
				try
				{
					ep.Address = IPAddress.Any;
					ep.Port = 0;
					EndPoint refEP = ep;
					lock (socket)
						socket.ReceiveFrom(buf, ref refEP);
					ep = (IPEndPoint)refEP;
					Console.WriteLine("Received: " + ep.Address + ":" + ep.Port);
				} catch(SocketException e)
				{
					if (e.SocketErrorCode != SocketError.TimedOut)
						Console.WriteLine(e);
				}
			}
		}

		static void Main(string[] args)
		{
			Thread receiveThread = null;
			try
			{
				var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socket.ReceiveTimeout = 100;
				socket.Bind(new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1])));
				receiveThread = new Thread(() => ReceiveThread(socket));
				receiveThread.Start();
				while (true)
				{
					var line = Console.ReadLine();
					var tokens = line.Split(' ');
					lock (socket)
						socket.SendTo(new byte[0], new IPEndPoint(IPAddress.Parse(tokens[0]), int.Parse(tokens[1])));
				}
			} catch(Exception e)
			{
				Console.WriteLine(e);
			} finally
			{
				if (receiveThread != null)
					receiveThread.Abort();
			}
		}
	}
}
