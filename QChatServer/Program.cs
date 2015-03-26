using System;
using System.Collections.Generic;
using System.Text;

namespace QChatServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var bytes = new byte[0x100];
			Console.WriteLine(Convert.ToBase64String(bytes).Length);
			for (int i = 0; i < bytes.Length; i++)
				bytes[i] = (byte)i;
			Console.WriteLine(Convert.ToBase64String(bytes).Length);
			Console.Read();
		}
	}
}
