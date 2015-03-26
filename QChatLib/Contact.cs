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

		public Contact(string name, ContactState state, DateTime requestSent)
		{
			Name = name;
			State = state;
			RequestSent = requestSent;
			Message = null;
		}

		public Contact(string name, ContactState state, DateTime requestSent, string message)
			: this(name, state, requestSent)
		{
			Message = message;
		}
	}
}
