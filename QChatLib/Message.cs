using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace QChatLib
{
	public abstract class Message<T>
	{
		public delegate Message<T> Reader(StreamWrapper stream);

		static readonly Reader[] readers =
			new Reader[byte.MaxValue + 1];

		protected static void AssignReader(byte type, Reader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			lock(readers)
				readers[(byte)type] = reader;
		}

		public abstract T Type { get; }

		protected static void Send(StreamWrapper stream, byte type)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			stream.Write(type);
		}

		public static Message<T> Receive(StreamWrapper stream)
		{
			var type = stream.ReadByte();
			var reader = readers[type];
			if (reader == null)
				throw new Exception("Invalid command");
			return reader(stream);
		}
	}
}
