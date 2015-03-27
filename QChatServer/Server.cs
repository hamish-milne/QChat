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

namespace QChatServer
{
    public class Server
    {
		X509Certificate serverCertificate;
		IPAddress ip;
		int port;
		int timeout;
		Stream log;
		StreamWriter writer;

		public void Log(string message)
		{
			lock(log)
			{
				if (writer == null)
					writer = new StreamWriter(log);
				writer.Write(message);
			}
		}

		public void Run()
		{
			var listener = new TcpListener(ip, port);
			listener.Start();
			while(true)
			{
				var client = listener.AcceptTcpClient();
				var sslStream = new SslStream(client.GetStream(), false);
				try
				{
					sslStream.AuthenticateAsServer(serverCertificate, false,
						SslProtocols.Tls, false);
					sslStream.ReadTimeout = timeout;
					sslStream.WriteTimeout = timeout;
					new Thread(new ServerThread(sslStream, this,
						((IPEndPoint)client.Client.RemoteEndPoint).Address).Run);
				} catch(Exception e)
				{

				}
			}
		}
    }
}
