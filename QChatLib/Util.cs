using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace QChatLib
{
	public enum IPv6Translation
	{
		Compatible,
		Mapped,
		Translated,
		WellKnown,
	}

	public static class Util
	{
		public const int ByteBits = 8;
		public const int DatagramSize = 0xFFFF;

		static readonly byte[] wkp = new byte[]
			{ 0, 0x64, 0xFF, 0x9B, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		static readonly byte[] translated = new byte[]
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF, 0, 0, 0, 0, 0, 0 };
		static readonly byte[] mapped = new byte[]
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF, 0, 0, 0, 0 };
		static readonly byte[] compatible = new byte[]
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

		public static IPAddress ToIPv6(IPAddress input, IPv6Translation translation)
		{
			byte[] prefix = null;
			switch(translation)
			{
				case IPv6Translation.Compatible:
					prefix = compatible;
					break;
				case IPv6Translation.Mapped:
					prefix = mapped;
					break;
				case IPv6Translation.Translated:
					prefix = translated;
					break;
				case IPv6Translation.WellKnown:
					prefix = wkp;
					break;
			}
			if (prefix == null)
				throw new ArgumentException("Invalid translation");
			return ToIPv6(input, prefix);
		}

		public static IPAddress ToIPv6(IPAddress input, byte[] prefix)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if(input.AddressFamily == AddressFamily.InterNetwork)
			{
				if (input == IPAddress.Any)
					return IPAddress.IPv6Any;
				if (input == IPAddress.None)
					return IPAddress.IPv6None;
				if (IPAddress.IsLoopback(input))
					return IPAddress.IPv6Loopback;
				var bytes = input.GetAddressBytes();
				if (bytes.Length != 4)
					throw new ArgumentException("Not an IPv4 address");
				if (prefix == null)
					throw new ArgumentNullException("prefix");
				if (prefix.Length < 16)
					throw new ArgumentException("Prefix is too short");
				for (int i = 0; i < 4; i++)
					prefix[i + 12] = bytes[i];
				return new IPAddress(prefix);
			}
			return input;
		}

	}
}
