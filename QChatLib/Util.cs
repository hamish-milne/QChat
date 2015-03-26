using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
	public static class Util
	{
		public static unsafe int GetBytes(string str, byte[] buffer, int offset)
		{
			fixed(byte* ptr = buffer)
			fixed(char* cptr = str)
			{
				return Encoding.UTF8.GetBytes(cptr, str.Length,
					ptr + offset, buffer.Length - offset);
			}
		}

		public static void WriteShortString(StreamWriter stream, string str)
		{
			var count = str == null ? 0 : stream.Encoding.GetByteCount(str);
			if (count > byte.MaxValue)
				throw new ArgumentException("String is too long");
			stream.Write((byte)count);
			if (str != null)
				stream.Write(str);
		}

		public static void WriteString(StreamWriter stream, string str)
		{
			var count = str == null ? 0 : stream.Encoding.GetByteCount(str);
			if (count > ushort.MaxValue)
				throw new ArgumentException("String is too long");
			stream.Write((ushort)count);
			if (str != null)
				stream.Write(str);
		}
	}
}
