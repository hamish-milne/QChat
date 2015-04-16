using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace QChatServer
{
	public class Config
	{
		readonly Dictionary<string, string> data
			= new Dictionary<string, string>();

		static class Serializers<T>
		{
			public static Func<string, T> Deserialize;
		}

		public T Get<T>(string key)
		{
			return Serializers<T>.Deserialize(data[key]);
		}

		public void Read(string file)
		{
			using(var stream = new FileStream(file, FileMode.OpenOrCreate))
			{
				var reader = new StreamReader(stream);
				string line;
				while((line = reader.ReadLine()) != null)
				{
					var tokens = line.Split(new char[] { '=' }, 2);
					if (tokens.Length < 2)
						continue;
					var key = tokens[0].Trim();
					var value = tokens[1].Trim();
					data[key] = value;
				}
			}
		}

		static Config()
		{
			Serializers<IPAddress>.Deserialize = IPAddress.Parse;
			Serializers<bool>.Deserialize = (str) => str.ToLower() == "true";
			Serializers<int>.Deserialize = int.Parse;
			Serializers<Type>.Deserialize = Type.GetType;
		}
	}
}
