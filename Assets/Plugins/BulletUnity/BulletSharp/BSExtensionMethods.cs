using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace BulletSharp
{
	public static class BSExtensionMethods
	{
		public static IntPtr Add(this IntPtr ptr, int amt)
		{
			return new IntPtr(ptr.ToInt64() + amt);
		}

		public static void Dispose(this BinaryReader reader)
		{
			var dynMethod = reader.GetType().GetMethod("Dispose",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new Type[] { typeof(bool) },
							null);
			dynMethod.Invoke(reader, new object[] { true });
		}

		public static void Dispose(this BinaryWriter writer)
		{
		}
	}
}

