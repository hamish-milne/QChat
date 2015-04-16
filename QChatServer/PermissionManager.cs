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

	public class AllPermissions : PermissionManager
	{
		public override int MaxConcurrentIPs(int permissions)
		{
			return int.MaxValue;
		}

		public override bool AllowGlobalBlock(int permissions)
		{
			return true;
		}

		public override TimeSpan ContactRequestTimeout(int permissions)
		{
			return TimeSpan.MaxValue;
		}

		public override int MaxContactMessageLength(int permissions)
		{
			return int.MaxValue;
		}

		public override int MaxContactRequests(int permissions)
		{
			return int.MaxValue;
		}

		public override int MaxContacts(int permissions)
		{
			return int.MaxValue;
		}
	}
}
