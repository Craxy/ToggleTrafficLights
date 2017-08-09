using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	public static class CollectionExtensions
	{
		public static IEnumerable<T> Do<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			if (sequence == null) return null;
			var enumerator = sequence.GetEnumerator();
			while (enumerator.MoveNext()) action(enumerator.Current);
			return sequence;
		}
	}
}
