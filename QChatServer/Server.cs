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
		Dictionary<string, IPAddress> addresses
			= new Dictionary<string, IPAddress>();
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

		public IPAddress GetIP(string username)
		{
			if(username == null)
				throw new ArgumentNullException("username");
			IPAddress ret = null;
			lock(addresses)
			{
				addresses.TryGetValue(username, out ret);
			}
			return ret;
		}

		public void SetIP(string username, IPAddress ip)
		{
			if (username == null)
				throw new ArgumentNullException("username");
			lock(addresses)
			{
				addresses[username] = ip;
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
