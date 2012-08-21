using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace System
{
	public static class ListExtensions
	{

		/// <summary>
		/// How many times a specific character occured in a string
		/// </summary>
		public static int FrequencyCount(this string str, char ch)
		{
			int count = 0;
			int lastPos = -1;
			while ((lastPos = str.IndexOf(ch, lastPos + 1)) != -1)
				count++;
			return count;
		}

		/// <summary>
		/// Performs the specified action on each element of the IEnumerable.
		/// </summary>
		public static void ForEachAction<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			if (action == null || enumerable == null)
			{
				throw new ArgumentNullException();
			}

			foreach (var item in enumerable)
			{
				action.Invoke(item);
			}
		}

		/// <summary>
		/// Removes the all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		public static void RemoveFromIList<TSource>(this IList<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null || predicate == null)
			{
				throw new ArgumentNullException();
			}
			for (int i = source.Count - 1; i >= 0; i--)
				if (predicate.Invoke(source[i]))
				{
					source.RemoveAt(i);
				}
		}

		/// <summary>
		/// Adds range of items to the collection 
		/// </summary>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
		{
			foreach (var item in enumerable)
			{
				collection.Add(item);
			}
		}

		/// <summary>
		/// Convertion to ObservableCollection
		/// </summary>
		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new ObservableCollection<T>(source);
		}

		/// <summary>
		/// For a large list (say 100000 records) this method is one or two seconds faster
		/// </summary>
		public static TSource FirstOrDefaultFast<TSource>(this IList<TSource> list, Func<TSource, bool> predicate)
		{
			if (list == null || (predicate == null))
			{
				throw new ArgumentNullException();
			}
			for (int i = 0; i < list.Count; i++)
			{
				var item = list[i];
				if (predicate(item))
				{
					return item;
				}
			}
			return default(TSource);
		}

		public static int CountFast<TSource>(this IList<TSource> list, Func<TSource, bool> predicate)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				var item = list[i];
				if (predicate(item))
				{
					num++;
				}
			}
			return num;
		}


		/// <summary>
		/// Index of an item according to the specified predicate
		/// </summary>
		public static int FirstIndexOf<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (predicate.Invoke(list[i]))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Removes first item found
		/// </summary>
		public static void RemoveFirst<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (predicate.Invoke(list[i]))
				{
					list.RemoveAt(i);
					return;
				}
			}
		}

		/// <summary>
		/// Removes last item found
		/// </summary>
		public static void RemoveLast<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (predicate.Invoke(list[i]))
				{
					list.RemoveAt(i);
					return;
				}
			}
		}

		/// <summary>
		/// Finds an item which has minimum value based on the specified function. For a list.
		/// </summary>
		public static T MinValue<T>(this IList<T> list, Func<T, int> function)
		{
			if (function == null || list == null)
			{
				throw new ArgumentNullException();
			}

			int minValue = int.MaxValue;
			T result = default(T);
			for (int i = 0; i < list.Count; i++)
			{
				var item = list[i];
				var val = function.Invoke(item);
				if (val < minValue)
				{
					result = item;
					minValue = val;
				}
			}
			return result;
		}

		/// <summary>
		/// Finds an item which has minimum value based on the specified function. For enumerables. 
		/// </summary>
		public static T MinValue<T>(this IEnumerable<T> enumerable, Func<T, int> function)
		{
			if (function == null || enumerable == null)
			{
				throw new ArgumentNullException();
			}

			int minValue = int.MaxValue;
			T result = default(T);
			foreach (var item in enumerable)
			{
				var val = function.Invoke(item);
				if (val < minValue)
				{
					result = item;
					minValue = val;
				}
			}
			return result;
		}

		/// <summary>
		/// Finds an item which has maximum value based on the specified function. For enumerables. 
		/// </summary>
		public static T MaxValue<T>(this IList<T> list, Func<T, int> function)
		{
			if (function == null || list == null)
			{
				throw new ArgumentNullException();
			}

			int maxValue = int.MinValue;
			T result = default(T);
			for (int i = 0; i < list.Count; i++)
			{
				var item = list[i];

				var val = function.Invoke(item);
				if (val > maxValue)
				{
					result = item;
					maxValue = val;
				}
			}
			return result;
		}

		/// <summary>
		/// Finds an item which has maximum value based on the specified function. For enumerables. 
		/// </summary>
		public static T MaxValue<T>(this IEnumerable<T> enumerable, Func<T, int> function)
		{
			if (function == null || enumerable == null)
			{
				throw new ArgumentNullException();
			}

			int maxValue = int.MinValue;
			T result = default(T);
			foreach (var item in enumerable)
			{
				var val = function.Invoke(item);
				if (val > maxValue)
				{
					result = item;
					maxValue = val;
				}
			}
			return result;
		}

	}
}
