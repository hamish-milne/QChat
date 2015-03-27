using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using QChatLib;
using System.IO;

namespace QChatServer
{
	public class ServerThread
	{
		SslStream client;
		Server server;
		IPAddress ip;
		string loggedIn;
		int currentPermissions;
		Database db;
		StreamWriter writer;
		PermissionManager permissions;
		IPManager ipManager;

		bool CheckLogin()
		{
			if (loggedIn == null)
			{
				ServerResponse.Send(writer, ResponseType.AuthenticationError,
					"You aren't logged in");
				return false;
			}
			return true;
		}

		public void Run()
		{
			try
			{
				while (true)
				{
					var request = ServerRequest.Receive(client);
					if (request.RequestType == RequestType.Close)
						break;
					switch(request.RequestType)
					{
						case RequestType.KeepAlive:
							break;
						case RequestType.Login:
							var login = (LoginRequest)request;
							if(!db.AuthenticateUser(login.Username, login.Password))
							{
								ServerResponse.Send(writer, ResponseType.AuthenticationError,
									"The username or password provided was incorrect");
								break;
							}
							loggedIn = login.Username;
							currentPermissions = db.GetPermissions(loggedIn);
							var maxIPcount = permissions.MaxConcurrentIPs(currentPermissions);
							if(!ipManager.AddIP(loggedIn, ip, maxIPcount))
							{
								ServerResponse.Send(writer, ResponseType.NotPermitted,
									"You're logged into too many devices. The most your account level allows is "
									+ maxIPcount);
								break;
							}
							ServerResponse.Success(writer);
							break;
						case RequestType.GetIP:
							if(!CheckLogin())
								break;
							var other = ((UsernameRequest)request).Username;
							if(!db.HasContact(loggedIn, other))
							{
								ServerResponse.Send(writer, ResponseType.NotPermitted,
									"You and " + other + " don't share a contact");
								break;
							}
							var ips = ipManager.GetIPs(other);
							SendIPs.Send(writer, ips);
							break;
						case RequestType.Logout:
							if(loggedIn != null)
							{
								ipManager.RemoveIP(loggedIn, ip);
								loggedIn = null;
							}
							ServerResponse.Success(writer);
							break;
						case RequestType.GetContacts:
							if(CheckLogin())
								SendContacts.Send(writer, db.GetContacts(loggedIn));
							break;
						case RequestType.SendContact:
							if (!CheckLogin())
								break;
							var req = (ContactRequest)request;
							if(!db.SendContact(loggedIn, req.Username, req.Message))
							{
								ServerResponse.Send(writer, ResponseType.Fail,
									"The contact already exists");
								break;
							}
							ServerResponse.Success(writer);
							break;
						default:
							ServerResponse.Send(writer, ResponseType.Fail,
								"Unknown command");
					}
				}
			} catch(Exception e)
			{

			} finally
			{
				client.Close();
			}
		}
		
		public ServerThread(SslStream client, Server server, IPAddress ip)
		{
			this.client = client;
			this.server = server;
			this.ip = ip;
			this.writer = new StreamWriter(client);
		}
	}
}
