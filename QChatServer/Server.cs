using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;
using QChatLib;

namespace QChatServer
{
    public class Server
    {
		X509Certificate serverCertificate;
		IPAddress ip;
		int port;
		int timeout;
		readonly List<Thread> threads = new List<Thread>();
		ServerSettings s;
		bool running;

		public Server(ServerSettings s, X509Certificate cert, int timeout)
			: this(s, cert, 100, IPAddress.Any, 0)
		{
		}

		public Server(ServerSettings s, X509Certificate cert, int timeout, IPAddress ip, int port)
		{
			this.s = s;
			this.serverCertificate = cert;
			this.timeout = timeout;
			this.ip = ip;
			this.port = port;
		}

		public void Stop()
		{
			running = false;
		}

		public void Run()
		{
			running = true;
			var listener = new TcpListener(ip, port);
			listener.Start();
			var ep = (IPEndPoint)listener.LocalEndpoint;
			s.Log.Log("Started server on " + ep.Address + ":" + ep.Port, LogLevel.Info);
			while(running)
			{
				var client = listener.AcceptTcpClient();
				var sslStream = new SslStream(client.GetStream(), false);
				Thread thread = null;
				try
				{
					sslStream.AuthenticateAsServer(serverCertificate, false,
						SslProtocols.Tls, false);
					sslStream.WriteTimeout = timeout;
					thread = new Thread(new ServerThread(sslStream,
						((IPEndPoint)client.Client.RemoteEndPoint).Address, s).Run);
					threads.Add(thread);
					thread.Start();
				} catch(Exception e)
				{
					s.Log.Log(e.ToString(), LogLevel.Error);
					if (thread != null)
					{
						thread.Abort();
						threads.Remove(thread);
					}
					sslStream.Close();
					client.Close();
				}
			}
		}
    }
}
