using System;
using System.Collections.Generic;
using System.Text;

namespace QChatLib
{
	public enum ServerResponse : byte
	{
		Success,
		Fail,
		InvalidRequest,
		AuthenticationError,
		NotPermitted,
		IncomingIP,
		IncomingContacts,
		IncomingKey,
		LoginSuccess,
		Timeout,
	}

	public enum ServerRequest : byte
	{
		RequestMode,
		NotifyMode,
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
		Relay,
		GroupCreate,
		GroupDelete,
		GroupAdd,
		GroupRemove,
		GroupRole,
	}

	public enum NotificationType : byte
	{
		ContactRequest,
		ContactRequestAccepted,
		ContactRequestRejected,
		GroupCreate,
		GroupDelete,
		GroupAdd,
		GroupRemove,
		GroupRole,
		Relay,
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
