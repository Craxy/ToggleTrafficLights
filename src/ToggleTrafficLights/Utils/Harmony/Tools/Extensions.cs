using System;
using System.Collections.Generic;

namespace Harmony
{
	internal static class CollectionExtensions
	{
		public static void Do<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			if (sequence == null)
			{
				return;
			}
			
			foreach (var e in sequence)
			{
				action(e);
			}
		}
	}
}
