using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace QChatLib
{
	public class InitializeAttribute : Attribute
	{
		static HashSet<Assembly> cache = new HashSet<Assembly>();

		public static void Init()
		{
			const BindingFlags bf = BindingFlags.Static |
				BindingFlags.Public | BindingFlags.NonPublic;
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				if(cache.Add(assembly))
					foreach (var type in assembly.GetTypes())
						foreach (var method in type.GetMethods(bf))
							if (method.GetCustomAttributes(typeof(InitializeAttribute), false).Length > 0)
								method.Invoke(null, null);
		}
	}
}
