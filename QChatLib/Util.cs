using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
	public static class Util
	{
		public static Encoding TEncoding
		{
			get { return Encoding.UTF8; }
		}

		public static unsafe int GetBytes(string str, byte[] buffer, int offset)
		{
			fixed(byte* ptr = buffer)
			fixed(char* cptr = str)
			{
				return TEncoding.GetBytes(cptr, str.Length,
					ptr + offset, buffer.Length - offset);
			}
		}

		public static string GetString(byte[] buffer, int length)
		{
			return TEncoding.GetString(buffer, 0, length);
		}

		public static void WriteShortString(BinaryWriter stream, string str)
		{
			var count = str == null ? 0 : TEncoding.GetByteCount(str);
			if (count > byte.MaxValue)
				throw new ArgumentException("String is too long");
			stream.Write((byte)count);
			if (str != null)
				stream.Write(str);
		}

		public static void WriteString(BinaryWriter stream, string str)
		{
			var count = str == null ? 0 : TEncoding.GetByteCount(str);
			if (count > ushort.MaxValue)
				throw new ArgumentException("String is too long");
			stream.Write((ushort)count);
			if (str != null)
				stream.Write(str);
		}

		public static string ReadShortString(BinaryReader stream, byte[] buf)
		{
			int len = stream.ReadByte();
			if (len < 0)
				throw new IOException("End of stream");
			if (buf == null)
				buf = new byte[len];
			len = stream.Read(buf, 0, len);
			return GetString(buf, len);
		}

		public static string ReadString(BinaryReader stream, byte[] buf)
		{
			int len = stream.ReadUInt16();
			if (len < 0)
				throw new IOException("End of stream");
			if (buf == null)
				buf = new byte[len];
			len = stream.Read(buf, 0, len);
			return GetString(buf, len);
		}
	}
}
