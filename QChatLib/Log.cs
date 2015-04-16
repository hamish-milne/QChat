using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QChatLib
{
	public enum LogLevel
	{
		Debug,
		Info,
		Warning,
		Error,
	}

	public interface ILog
	{
		void Log(string message, LogLevel level);
	}

	public class ConsoleLog : ILog
	{
		StreamWriter stream;

		public void Log(string message, LogLevel level)
		{
			lock (stream)
			{
				stream.WriteLine(message);
				stream.Flush();
			}
		}

		public ConsoleLog()
		{
			stream = new StreamWriter(Console.OpenStandardOutput());
		}
	}
}
