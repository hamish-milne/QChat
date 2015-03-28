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
		GetPublicKey,
		GetPrivateKey,
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

	public enum MessageType : byte
	{
		Acknowledge,
		RequestPublicKey,
		MessageSend,
		MessageRequest,
		FileSend,
		FileAccept,
		FileData,
		VoiceRequest,
		VoiceAccept,
		VoiceReject,
		VoiceEnd,
		VoiceData,
	}
}
