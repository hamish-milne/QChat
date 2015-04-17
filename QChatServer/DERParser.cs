using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QChatLib;

namespace QChatServer
{
	public class DERParser
	{
		public enum ClassType
		{
			Universal = 0,
			Application = 1,
			ContextSpecific = 2,
			Private = 3,
		}

		public class Record
		{
			public ulong ID;
			public ClassType ClassType;
			public bool Constructed;
			public object Data;
		}

		ulong streamLength = ulong.MaxValue;

		int ReadByte(Stream stream)
		{
			int Byte = -1;
			if(streamLength != ulong.MaxValue)
				Byte = stream.ReadByte();
			if (Byte < 0)
				throw new IOException("End of stream");
			return Byte;
		}

		Record ParseIdentifier(Stream stream)
		{
			var Byte = ReadByte(stream);
			var ret = new Record();
			ret.ClassType = (ClassType)(Byte >> 6);
			ret.Constructed = ((Byte >> 5) & 1) != 0;
			ret.ID = (ulong)(Byte & 0x1F);
			if(ret.ID == 0x1F)
			{
				ret.ID = 0;
				int count = 0;
				do
				{
					count++;
					if (count > 9)
						throw new IOException("Identifier is too large");
					Byte = ReadByte(stream);
					ret.ID = (ret.ID << 7) + (ulong)(Byte & 0x7F);
				} while ((Byte & 0x80) != 0);
			}
			return ret;
		}

		bool ParseLength(Stream stream, out ulong ret)
		{
			var Byte = ReadByte(stream);
			if ((Byte & 0x80) == 0)
			{
				ret = (ulong)Byte;
				return true;
			}
			ret = 0;
			var length = (Byte & 0x7F);
			if (length == 0)
				return false;
			if (length > sizeof(ulong))
				throw new IOException("Length is too large");
			while (length-- > 0)
				ret = (ret << 8) + (ulong)ReadByte(stream);
			if (ret == ulong.MaxValue)
				throw new IOException("Length is too large");
			return true;
		}

		public Record ParseObject(Stream stream)
		{
			var record = ParseIdentifier(stream);
			ulong length;
			if(ParseLength(stream, out length))
			{
				if(record.Constructed)
				{
					var storedLength = streamLength;
					streamLength = length;
					var recordList = new List<Record>();
					while (streamLength > 0)
						recordList.Add(ParseObject(stream));
				} else
				{

				}
			}

		}


		void ParseConstructed(Stream stream, bool endOnZero)
		{

		}

		public void ParseContents(Stream stream)
		{

		}

		public void Parse(Stream stream)
		{

		}

	}
}
