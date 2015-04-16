using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QChatLib
{
	public struct ClientAddress
	{
		public IPAddress Address;
		public ushort Port;

		public ClientAddress(IPAddress address, ushort port)
		{
			Address = address;
			Port = port;
		}
	}
}
