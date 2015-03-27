using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QChatServer
{
	public class IPManager
	{
		Dictionary<string, List<IPAddress>> dict;
		IPAddress[] empty = new IPAddress[0];

		public virtual bool AddIP(string username, IPAddress ip, int maxConcurrent)
		{
			if (username == null)
				throw new ArgumentNullException("username");
			if(ip == null)
				throw new ArgumentNullException("ip");
			if (dict == null)
				dict = new Dictionary<string, List<IPAddress>>();
			lock(dict)
			{
				List<IPAddress> list;
				dict.TryGetValue(username, out list);
				if (list == null)
				{
					if (maxConcurrent < 1)
						return false;
					list = new List<IPAddress>();
					dict.Add(username, list);
				}
				if (maxConcurrent <= list.Count)
					return false;
				list.Add(ip);
			}
			return true;
		}

		public virtual void RemoveIP(string username, IPAddress ip)
		{
			if (username == null)
				throw new ArgumentNullException("username");
			if (ip == null)
				throw new ArgumentNullException("ip");
			if (dict == null)
				return;
			lock(dict)
			{
				List<IPAddress> list;
				dict.TryGetValue(username, out list);
				if (list != null)
					list.Remove(ip);
			}
		}

		public virtual IList<IPAddress> GetIPs(string username)
		{
			if (username == null)
				throw new ArgumentNullException("username");
			if (dict == null)
				return null;
			IPAddress[] ret;
			lock(dict)
			{
				List<IPAddress> list;
				dict.TryGetValue(username, out list);
				if (list == null)
					ret = empty;
				else
					ret = list.ToArray();
			}
			return ret;
		}
	}
}
