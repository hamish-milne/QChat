using System;
using System.Collections.Generic;
using System.Text;

namespace QChatLib
{
	public struct Contact
	{
		public string Name;
		public ContactState State;
		public DateTime RequestSent;
		public string Message;

		public Contact(ContactState state, string name, DateTime requestSent, string message)
		{
			Name = name;
			State = state;
			RequestSent = requestSent;
			Message = message;
		}
	}
}
