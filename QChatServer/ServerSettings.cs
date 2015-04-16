using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QChatLib;

namespace QChatServer
{
	public struct ServerSettings
	{
		public Database Db;
		public PermissionManager Permissions;
		public IpManager IpManager;
		public HolePunchServer HolePunchServer;
		public ILog Log;
	}
}
