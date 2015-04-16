using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QChatLib
{
	public class StreamWrapper
	{
		byte[] buffer = new byte[0x10000];
		Stream stream;

		public static Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}

		public Stream BaseStream
		{
			get { return stream; }
		}

		public StreamWrapper(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			this.stream = stream;
		}

		public void WriteString(string s, int lengthBytes)
		{
			if (lengthBytes <= 0 || lengthBytes > sizeof(int))
				throw new ArgumentOutOfRangeException("lengthBytes");
			if (s == null)
				s = "";
			int len = Encoding.GetByteCount(s);
			if (len >= (0x100 << lengthBytes))
				throw new ArgumentException("String is too long");
			for(int i = lengthBytes - 1; i >= 0; i--)
			{
				var b = (byte)(len >> (8 * i));
				stream.WriteByte(b);
			}
			len = Encoding.GetBytes(s, 0, s.Length, buffer, 0);
			stream.Write(buffer, 0, len);
		}

		public string ReadString(int lengthBytes)
		{
			if (lengthBytes <= 0 || lengthBytes > sizeof(int))
				throw new ArgumentOutOfRangeException("lengthBytes");
			int len = 0;
			for(int i = 0; i < lengthBytes; i++)
			{
				var b = ReadByte();
				len <<= 8;
				len += b;
			}
			len = stream.Read(buffer, 0, len);
			return Encoding.GetString(buffer, 0, len);
		}

		public void Write(byte b)
		{
			stream.WriteByte(b);
		}

		public byte ReadByte()
		{
			var b = stream.ReadByte();
			if (b < 0)
				throw new IOException("End of stream");
			return (byte)b;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			return stream.Read(buffer, offset, count);
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}

		public void Write(ushort s)
		{
			stream.WriteByte((byte)(s >> 8));
			stream.WriteByte((byte)s);
		}

		public ushort ReadUShort()
		{
			ushort ret = (ushort)stream.ReadByte();
			ret <<= 8;
			return (ushort)(ret + stream.ReadByte());
		}

		public void Write(long l)
		{
			for (int i = sizeof(long) - 1; i >= 0; i--)
				stream.WriteByte((byte)(l >> (8 * i)));
		}

		public void Write(ulong l)
		{
			Write((long)l);
		}

		public long ReadLong()
		{
			long ret = 0;
			for(int i = 0; i < sizeof(long); i++)
			{
				ret <<= 8;
				ret += stream.ReadByte();
			}
			return ret;
		}

		public ulong ReadULong()
		{
			return (ulong)ReadLong();
		}

		public void Flush()
		{
			stream.Flush();
		}
	}
}
