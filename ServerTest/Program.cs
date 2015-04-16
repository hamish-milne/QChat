using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using QChatLib;

namespace ServerTest
{
	class Program
	{

		static void Main(string[] args)
		{
			try
			{
				var client = new TcpClient("localhost", 81);
				var sslStream = new SslStream(client.GetStream(), false, Program_callback);
				sslStream.AuthenticateAsClient("localhost");
				var stream = new StreamWrapper(sslStream);
				LoginRequest.Send(stream, "user", "password");
				var response = ServerMessage.Receive(stream);
				Console.WriteLine(response.Type);
				Console.Read();
				ServerMessage.Send(stream, ServerMessageType.Close);
			} catch(Exception e)
			{
				Console.WriteLine(e);
			}


		}

		static bool Program_callback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}
}
