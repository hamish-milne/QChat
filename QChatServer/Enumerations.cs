using System;
using System.Collections.Generic;
using System.Text;

namespace QChatServer
{
	public enum Request : byte
	{
		KeepAlive,
		Login,
		Logout,
		GetIP,
		GetNotification,
		SendContact,
		AcceptContact,
		RejectContact,
	}

	public enum Response : byte
	{
		Success,
		Fail,
		AuthenticationError,
		NotPermitted,
		InvalidArgument,
	}
}
