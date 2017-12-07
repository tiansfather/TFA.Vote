using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TF.Common
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list.ToList())
            {
                action(item);
            }
        }

        public static void ForEachFirst<T>(this IEnumerable<T> list, Action<T, bool> action)
        {
            bool isFirst = true;
            foreach (var item in list.ToList())
            {
                action(item, isFirst);
                isFirst = false;
            }
        }

        public static void ForEachIndex<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            int i = 0;
            foreach (var item in list.ToList())
            {
                action(item, i++);
            }
        }

        public static IEnumerable<TSource> Distinct<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> projection)
        {
            return DistinctIterator(source, projection, EqualityComparer<TResult>.Default);
        }

        public static IEnumerable<TSource> Distinct<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> projection, IEqualityComparer<TResult> equalityComparer)
        {
            return DistinctIterator(source, projection, equalityComparer);
        }

        private static IEnumerable<TSource> DistinctIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> projection, IEqualityComparer<TResult> equalityComparer)
        {
            var alreadySeenValues = new HashSet<TResult>(equalityComparer);

            foreach (var element in source)
            {
                var value = projection(element);

                if (alreadySeenValues.Contains(value))
                {
                    continue;
                }

                yield return element;
                alreadySeenValues.Add(value);
            }
        }

        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }

        public static IEnumerable<TSource> TakeSkip<TSource>(IEnumerable<TSource> source, int take, int skip)
        {
            var enumerator = source.GetEnumerator();

            while (true)
            {
                for (int i = 0; i < take; i++)
                {
                    if (!enumerator.MoveNext())
                        yield break;

                    yield return enumerator.Current;
                }

                for (int i = 0; i < skip; i++)
                {
                    if (!enumerator.MoveNext())
                        yield break;
                }
            }
        }

        /// <summary>
        /// Returns all elements of <paramref name="source"/> without <paramref name="elements"/>.
        /// Does not throw an exception if <paramref name="source"/> does not contain <paramref name="elements"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> to remove the specified elements from.</param>
        /// <param name="elements">The elements to remove.</param>
        /// <returns>
        /// All elements of <paramref name="source"/> except <paramref name="elements"/>.
        /// </returns>
        public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source, params TSource[] elements)
        {
            return Without(source, (IEnumerable<TSource>)elements);
        }

        /// <summary>
        /// Returns all elements of <paramref name="source"/> without <paramref name="elements"/>.
        /// Does not throw an exception if <paramref name="source"/> does not contain <paramref name="elements"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> to remove the specified elements from.</param>
        /// <param name="elements">The elements to remove.</param>
        /// <returns>
        /// All elements of <paramref name="source"/> except <paramref name="elements"/>.
        /// </returns>
        public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> elements)
        {
            return WithoutIterator(source, elements, EqualityComparer<TSource>.Default);
        }

        /// <summary>
        /// Returns all elements of <paramref name="source"/> without <paramref name="elements"/> using the specified equality comparer to compare values.
        /// Does not throw an exception if <paramref name="source"/> does not contain <paramref name="elements"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> to remove the specified elements from.</param>
        /// <param name="equalityComparer">The equality comparer to use.</param>
        /// <param name="elements">The elements to remove.</param>
        /// <returns>
        /// All elements of <paramref name="source"/> except <paramref name="elements"/>.
        /// </returns>
        public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source,
            IEqualityComparer<TSource> equalityComparer, params TSource[] elements)
        {
            return Without(source, equalityComparer, (IEnumerable<TSource>)elements);
        }

        /// <summary>
        /// Returns all elements of <paramref name="source"/> without <paramref name="elements"/> using the specified equality comparer to compare values.
        /// Does not throw an exception if <paramref name="source"/> does not contain <paramref name="elements"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> to remove the specified elements from.</param>
        /// <param name="equalityComparer">The equality comparer to use.</param>
        /// <param name="elements">The elements to remove.</param>
        /// <returns>
        /// All elements of <paramref name="source"/> except <paramref name="elements"/>.
        /// </returns>
        public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source,
            IEqualityComparer<TSource> equalityComparer, IEnumerable<TSource> elements)
        {
            return WithoutIterator(source, elements, equalityComparer);
        }

        private static IEnumerable<TSource> WithoutIterator<TSource>(IEnumerable<TSource> source,
            IEnumerable<TSource> elementsToRemove, IEqualityComparer<TSource> comparer)
        {
            HashSet<TSource> elementsToRemoveSet = new HashSet<TSource>(elementsToRemove, comparer);

            return source.Where(elem => !elementsToRemoveSet.Contains(elem));
        }

        /// <summary>
        /// Determines whether the specified sequence's element count is equal to or greater than <paramref name="minElementCount"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> whose elements to count.</param>
        /// <param name="minElementCount">The minimum number of elements the specified sequence is expected to contain.</param>
        /// <returns>
        ///   <c>true</c> if the element count of <paramref name="source"/> is equal to or greater than <paramref name="minElementCount"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAtLeast<TSource>(this IEnumerable<TSource> source, int minElementCount)
        {
            return HasAtLeast(source, minElementCount, _ => true);
        }

        /// <summary>
        /// Determines whether the specified sequence contains exactly <paramref name="minElementCount"/> or more elements satisfying a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> whose elements to count.</param>
        /// <param name="minElementCount">The minimum number of elements satisfying the specified condition the specified sequence is expected to contain.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        ///   <c>true</c> if the element count of satisfying elements is equal to or greater than <paramref name="minElementCount"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAtLeast<TSource>(this IEnumerable<TSource> source, int minElementCount, Func<TSource, bool> predicate)
        {
            if (minElementCount == 0)
            {
                return true;
            }

            ICollection sourceCollection = source as ICollection;

            if (sourceCollection != null && sourceCollection.Count < minElementCount)
            {
                // If the collection doesn't even contain as many elements
                // as expected to match the predicate, we can stop here
                return false;
            }

            int matches = 0;

            foreach (TSource element in source.Where(predicate))
            {
                matches++;

                if (matches >= minElementCount)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified sequence's element count is at most <paramref name="maxElementCount"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> whose elements to count.</param>
        /// <param name="maxElementCount">The maximum number of elements the specified sequence is expected to contain.</param>
        /// <returns>
        ///   <c>true</c> if the element count of <paramref name="source"/> is equal to or lower than <paramref name="maxElementCount"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAtMost<TSource>(this IEnumerable<TSource> source, int maxElementCount)
        {
            return HasAtMost(source, maxElementCount, _ => true);
        }

        /// <summary>
        /// Determines whether the specified sequence contains at most <paramref name="maxElementCount"/> elements satisfying a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> whose elements to count.</param>
        /// <param name="maxElementCount">The maximum number of elements satisfying the specified condition the specified sequence is expected to contain.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        ///   <c>true</c> if the element count of satisfying elements is equal to or less than <paramref name="maxElementCount"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAtMost<TSource>(this IEnumerable<TSource> source, int maxElementCount, Func<TSource, bool> predicate)
        {
            ICollection sourceCollection = source as ICollection;

            if (sourceCollection != null && sourceCollection.Count <= maxElementCount)
            {
                // If the collection doesn't contain more elements
                // than expected to match the predicate, we can stop here
                return true;
            }

            int matches = 0;

            foreach (TSource element in source.Where(predicate))
            {
                matches++;

                if (matches > maxElementCount)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified sequence contains exactly the specified number of elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> to count.</param>
        /// <param name="elementCount">The number of elements the specified sequence is expected to contain.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="source"/> contains exactly <paramref name="elementCount"/> elements; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasExactly<TSource>(this IEnumerable<TSource> source, int elementCount)
        {
            ICollection sourceCollection = source as ICollection;

            if (sourceCollection != null)
            {
                return sourceCollection.Count == elementCount;
            }

            return HasExactly(source, elementCount, _ => true);
        }

        /// <summary>
        /// Determines whether the specified sequence contains exactly the specified number of elements satisfying the specified condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{TSource}"/> to count satisfying elements.</param>
        /// <param name="elementCount">The number of matching elements the specified sequence is expected to contain.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="source"/> contains exactly <paramref name="elementCount"/> elements satisfying the condition; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasExactly<TSource>(this IEnumerable<TSource> source, int elementCount, Func<TSource, bool> predicate)
        {
            ICollection sourceCollection = source as ICollection;
            if (sourceCollection != null && sourceCollection.Count < elementCount)
            {
                // If the collection doesn't even contain as many elements
                // as expected to match the predicate, we can stop here
                return false;
            }

            int matches = 0;

            foreach (var element in source.Where(predicate))
            {
                ++matches;

                if (matches > elementCount)
                {
                    return false;
                }
            }

            return matches == elementCount;
        }
    }
}