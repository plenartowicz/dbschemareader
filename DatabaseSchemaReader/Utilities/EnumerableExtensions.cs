using System.Collections.Generic;
using System.Linq;

namespace DatabaseSchemaReader.Utilities
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}"/>
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Checks two collection if each element is equal and it is null-check-safe
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool AreEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
		{
			if (ReferenceEquals(first, second))
			{
				return true;
			}

			if (ReferenceEquals(null, first) || ReferenceEquals(null, second))
			{
				return false;
			}

			return first.SequenceEqual(second, comparer);
		}
	}
}