using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace QChatServer
{
	public class HolePunchServer
	{
		struct IPKey : IEquatable<IPKey>
		{
			IPAddress IP;
			ulong SessionKey;
			
			public override int GetHashCode()
			{
 				 return (int)SessionKey ^ (23*IP.GetHashCode());
			}

			public bool Equals(IPKey other)
			{
				return (other.IP == IP) && (other.SessionKey == SessionKey);
			}

			public override bool Equals(object obj)
			{
				if (!(obj is IPKey))
					return false;
 				return Equals((IPKey)obj);
			}

			public IPKey(IPAddress ip, ulong sessionKey)
			{
				IP = ip;
				SessionKey = sessionKey;
			}
		}

		readonly Socket socket;
		readonly byte[] buffer = new byte[sizeof(ulong) + 1];
		readonly HashSet<IPKey> records;
		readonly Dictionary<IPKey, ushort> sourcePorts;
		readonly RandomNumberGenerator random;
		readonly byte[] rbuf = new byte[sizeof(ulong)];
		IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
		bool running;

		public HolePunchServer(int port)
		{
			socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram,
				ProtocolType.Udp);
			socket.Bind(new IPEndPoint(IPAddress.IPv6Any, port));
			random = RandomNumberGenerator.Create();
			records = new HashSet<IPKey>();
			sourcePorts = new Dictionary<IPKey, ushort>();
		}

		public virtual ulong Add(IPAddress sourceIP)
		{
			if (sourceIP == null)
				throw new ArgumentNullException("sourceIP");
			ulong sessionKey;
			IPKey key;
			lock (records)
			{
				do
				{
					random.GetBytes(rbuf);
					sessionKey = 0;
					for (int i = 0; i < rbuf.Length; i++)
					{
						sessionKey <<= 8;
						sessionKey += rbuf[i];
					}
					key = new IPKey(sourceIP, sessionKey);
				} while (records.Contains(key));
				records.Add(key);
			}
			return sessionKey;
		}

		public virtual bool Forget(IPAddress ip, ulong sessionKey)
		{
			if (ip == null)
				throw new ArgumentNullException("ip");
			lock(records)
				return records.Remove(new IPKey(ip, sessionKey));
		}

		public virtual bool GetSourcePort(IPAddress ip, ulong sessionKey, out ushort port)
		{
			if (ip == null)
				throw new ArgumentNullException("ip");
			lock(sourcePorts)
				return sourcePorts.TryGetValue(new IPKey(ip, sessionKey), out port);
		}

		public virtual void Run()
		{
			if (running)
				throw new InvalidOperationException("Server is running on another thread");
			var ack = new byte[0];
			while(running)
			{
				remoteEndPoint.Address = IPAddress.IPv6Any;
				remoteEndPoint.Port = 0;
				EndPoint endpoint = remoteEndPoint;
				int len = socket.ReceiveFrom(buffer, ref endpoint);
				socket.SendTo(ack, endpoint);
				if (len != sizeof(ulong))
					continue;
				var ipep = (IPEndPoint)endpoint;
				ulong sessionKey = 0;
				for(int i = 0; i < sizeof(ulong); i++)
				{
					sessionKey <<= 8;
					sessionKey += buffer[i];
				}
				var key = new IPKey(ipep.Address, sessionKey);
				bool removed;
				lock(records)
					removed = records.Remove(key);
				if(removed)
				{
					lock (sourcePorts)
						sourcePorts.Add(key, (ushort)ipep.Port);
				}
			}
		}

		public virtual void Stop()
		{
			running = false;
		}
	}
}
