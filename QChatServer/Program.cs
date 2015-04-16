using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using QChatLib;

namespace QChatServer
{
	class Program
	{
		static void Main(string[] args)
		{
			InitializeAttribute.Init();
			ServerSettings s;
			s.Db = new MemoryDb();
			s.Db.AddUser("user", "password");
			s.HolePunchServer = new HolePunchServer(2000);
			s.IpManager = new IpManager();
			s.Permissions = new AllPermissions();
			s.Log = new ConsoleLog();
			var server = new Server(s, new X509Certificate2("server.pfx", "password"), 1000, System.Net.IPAddress.IPv6Any, 81);
			server.Run();
		}
	}
}
