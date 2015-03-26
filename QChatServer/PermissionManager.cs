using System;
using System.Collections.Generic;
using System.Text;

namespace QChatServer
{
	public abstract class PermissionManager
	{
		public abstract int MaxContactRequests(int permissions);
		public abstract int MaxContacts(int permissions);
		public abstract int MaxConcurrentIPs(int permissions);
		public abstract int MaxContactMessageLength(int permissions);
		public abstract bool AllowGlobalBlock(int permissions);
		public abstract TimeSpan ContactRequestTimeout(int permissions);
	}
}
