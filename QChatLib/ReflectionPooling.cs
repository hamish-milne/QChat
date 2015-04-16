using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace QChatLib
{
	public class ReflectionPooling
	{
		static readonly List<Type> types = new List<Type>();

		static ReflectionPooling()
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				types.AddRange(assembly.GetTypes());
			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		}

		static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			types.AddRange(args.LoadedAssembly.GetTypes());
		}

		public static IList<Type> GetTypes()
		{
			return types;
		}

		public delegate bool TypeFilter(Type t);

		public static IList<Type> GetTypes(TypeFilter filter)
		{
			var ret = new List<Type>();
			foreach (var t in types)
				if (filter(t))
					ret.Add(t);
			return ret;
		}

		public static bool HasAttribute<T>(MemberInfo m) where T : Attribute
		{
			return (m.GetCustomAttributes(typeof(T), false).Length > 0);
		}
	}
}
