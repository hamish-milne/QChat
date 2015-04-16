using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QChatLib;

namespace QChatServer
{
	class DERParser
	{
		public enum ClassType
		{
			Universal = 0,
			Application = 1,
			ContextSpecific = 2,
			Private = 3,
		}

		public struct Record
		{
			public ulong ID;
			public ClassType ClassType;
			public bool Constructed;
			public byte[] Data;
		}

		static int ReadByte(Stream stream)
		{
			var Byte = stream.ReadByte();
			if (Byte < 0)
				throw new IOException("End of stream");
			return Byte;
		}

		public Record ParseIdentifier(Stream stream)
		{
			var Byte = ReadByte(stream);
			var ret = default(Record);
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

		public bool ParseLength(Stream stream, out ulong ret)
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
			while(length-- > 0)
				ret = (ret << 8) + (ulong)ReadByte(stream);
			return true;
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
