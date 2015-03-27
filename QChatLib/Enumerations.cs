using System;
using System.Collections.Generic;
using System.Text;

namespace QChatLib
{
	public enum RequestType : byte
	{
		KeepAlive,
		Close,
		Login,
		Logout,
		GetIP,
		GetContacts,
		SendContact,
		AcceptContact,
		RejectContact,
	}

	public enum ResponseType : byte
	{
		Success,
		Fail,
		InvalidRequest,
		AuthenticationError,
		NotPermitted,
		IncomingIP,
		IncomingContacts,
		Timeout,
	}

	public enum NotificationType : byte
	{
		ContactRequest,
		ContactRequestAccepted,
		ContactRequestRejected,
	}

	public enum ContactState
	{
		Sent,
		Received,
		Accepted,
	}
}
