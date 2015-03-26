using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using QChatLib;

namespace QChatServer
{
	public class ServerThread
	{
		SslStream client;
		Server server;
		IPAddress ip;
		string loggedIn;

		public void Run()
		{
			try
			{
				while (true)
				{
					var request = RequestReader.Read(client);
					if (request.RequestType == RequestType.Close)
						break;
					switch(request.RequestType)
					{
						case RequestType.KeepAlive:
							break;
						case RequestType.GetIP:
							// Check contact
							var ip = server.GetIP(((UsernameRequest)request).Username);
							if(ip == null)
							{
								// Write response: fail
							} else
							{
								// Write response
							}
							break;
						case RequestType.Login:
							var login = (LoginRequest)request;
							server.SetIP(login.Username, this.ip);
							// Write success
							break;
							
					}
				}
				client.Close();
			} catch(Exception e)
			{

			}
		}
		
		public ServerThread(SslStream client, Server server, IPAddress ip)
		{
			this.client = client;
			this.server = server;
			this.ip = ip;
		}
	}
}
