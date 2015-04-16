using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using QChatLib;

namespace QChatServer
{
	public delegate void MessageHandler(ServerThread obj, ServerMessage message);

	public class ServerThread
	{
		static readonly MessageHandler[] handlers = new MessageHandler[byte.MaxValue + 1];

		protected static void AssignHandler(ServerMessageType type, MessageHandler handler)
		{
			handlers[(byte)type] = handler;
		}

		protected void HandleMessage(ServerMessage message)
		{
			MessageHandler handle;
			lock(handlers)
				handle = handlers[(byte)message.Type];
			if (handle == null)
				SingleResponse.Send(clientStream, ServerMessageType.Fail,
								"Unknown command");
			else
				handle(this, message);
		}

		static ServerThread()
		{
			AssignHandler(ServerMessageType.Login,
				(obj, message) => obj.Login(message));
			AssignHandler(ServerMessageType.GetIP,
				(obj, message) => obj.GetIP(message));
			AssignHandler(ServerMessageType.Logout,
				(obj, message) => obj.Logout());
			AssignHandler(ServerMessageType.GetContacts,
				(obj, message) => obj.GetContacts(message));
			AssignHandler(ServerMessageType.SendContact,
				(obj, message) => obj.SendContact(message));
			AssignHandler(ServerMessageType.GetPublicKey,
				(obj, message) => obj.GetPublicKey(message));
			AssignHandler(ServerMessageType.SendContact,
				(obj, message) => obj.GetPrivateKey(message));
		}

		SslStream client;
		StreamWrapper clientStream;
		IPAddress ip;
		string loggedIn;
		int currentPermissions;
		ServerSettings s;
		ulong sessionKey;

		bool CheckLogin(StreamWrapper clientStream)
		{
			if (loggedIn == null)
			{
				SingleResponse.Send(clientStream, ServerMessageType.AuthenticationError,
					"You aren't logged in");
				return false;
			}
			return true;
		}

		void Login(ServerMessage request)
		{
			if (loggedIn != null)
			{
				SingleResponse.Send(clientStream, ServerMessageType.Fail,
					"You're already logged in");
				return;
			}
			var login = (LoginRequest)request;
			if (!s.Db.AuthenticateUser(login.Username, login.Password))
			{
				SingleResponse.Send(clientStream, ServerMessageType.AuthenticationError,
					"The username or password provided was incorrect");
				return;
			}
			loggedIn = login.Username;
			currentPermissions = s.Db.GetPermissions(loggedIn);
			var maxIPcount = s.Permissions.MaxConcurrentIPs(currentPermissions);
			if (!s.IpManager.AddIP(loggedIn, ip, maxIPcount))
			{
				SingleResponse.Send(clientStream, ServerMessageType.NotPermitted,
					"You're logged into too many devices. The most your account level allows is "
					+ maxIPcount);
				return;
			}
			sessionKey = s.HolePunchServer.Add(ip);
			LoginSuccess.Send(clientStream, sessionKey);
		}

		void GetIP(ServerMessage request)
		{
			if (!CheckLogin(clientStream))
				return;
			var other = ((UsernameRequest)request).Username;
			if (!s.Db.HasContact(loggedIn, other))
			{
				SingleResponse.Send(clientStream, ServerMessageType.NotPermitted,
					"You and " + other + " don't share a contact");
				return;
			}
			var ips = s.IpManager.GetIPs(other);
			var list = new List<ClientAddress>();
			for (int i = 0; i < ips.Count; i++)
			{
				ushort port;
				if (s.HolePunchServer.GetSourcePort(ips[i], sessionKey, out port))
					list.Add(new ClientAddress(ips[i], port));
			}
			if (list.Count <= 0)
			{
				SingleResponse.Send(clientStream, ServerMessageType.Fail,
					other + " isn't logged in");
				return;
			}
			SendIPs.Send(clientStream, list);
		}

		void Logout()
		{
			if (loggedIn != null)
			{
				s.IpManager.RemoveIP(loggedIn, ip);
				loggedIn = null;
			}
			ServerMessage.Success(clientStream);
		}

		void GetContacts(ServerMessage request)
		{
			if (CheckLogin(clientStream))
				SendContacts.Send(clientStream, s.Db.GetContacts(loggedIn));
		}

		void SendContact(ServerMessage request)
		{
			if (!CheckLogin(clientStream))
				return;
			var req = (ContactRequest)request;
			if (!s.Db.SendContact(loggedIn, req.Username, req.Message))
			{
				SingleResponse.Send(clientStream, ServerMessageType.Fail,
					"The contact already exists");
				return;
			}
			ServerMessage.Success(clientStream);
		}

		void GetPublicKey(ServerMessage request)
		{
			var req = (UsernameRequest)request;
			var key = s.Db.GetPublicKey(req.Username);
			if (key == null)
				SingleResponse.Send(clientStream, ServerMessageType.Fail,
					"No key information found");
			else
				IncomingKey.Send(clientStream, key);
		}

		void GetPrivateKey(ServerMessage request)
		{
			if(!CheckLogin(clientStream))
				return;
			var req = (UsernameRequest)request;
			var key = s.Db.GetPrivateKey(loggedIn, req.Username);
			if (key == null)
				SingleResponse.Send(clientStream, ServerMessageType.Fail,
					"No key information found");
			else
				IncomingKey.Send(clientStream, key);
		}

		public void Run()
		{
			try
			{
				s.Log.Log("Connected to client " + ip, LogLevel.Info);
				while (true)
				{
					var request = ServerMessage.Receive(clientStream);
					if (request.Type == ServerMessageType.Close)
						break;
					HandleMessage(request);
					clientStream.Flush();
				}
			} catch(Exception e)
			{
				s.Log.Log(e.GetType() + ": " + e.Message, LogLevel.Error);
			} finally
			{
				client.Close();
				s.Log.Log("Disconnected from client " + ip, LogLevel.Info);
			}
		}
		
		public ServerThread(SslStream client, IPAddress ip, ServerSettings s)
		{
			this.client = client;
			this.clientStream = new StreamWrapper(client);
			this.ip = ip;
			this.s = s;
		}
	}
}
