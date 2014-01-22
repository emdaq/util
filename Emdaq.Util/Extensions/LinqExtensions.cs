using System;
using System.Collections.Generic;
using System.Linq;

namespace Emdaq.Util.Extensions
{
    public static class LinqExtensions
    {
        //alden wins the game... damnit i just lost the game... yeah that game
        public static IEnumerable<IList<T>> Chunk<T>(this IEnumerable<T> input, int chunkSize)
        {
            if (chunkSize == 0 || input == null)
            {
                throw new ArgumentException();
            }
        
            var list = new List<T>(chunkSize);
            
            foreach (var item in input)
            {
                list.Add(item);
                
                if (list.Count == chunkSize)
                {
                    yield return list;
                    list = new List<T>(chunkSize);
                }
            }

            if (list.Count > 0)
            {
                yield return list;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> input)
        {
            return new HashSet<T>(input);
        } 

        public static HashSet<TKey> ToHashSet<T, TKey>(this IEnumerable<T> input, Func<T, TKey> keySelector)
        {
            return new HashSet<TKey>(input.Select(keySelector));
        } 

        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> input, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            foreach (var kvp in input.Where(predicate))
            {
                input.Remove(kvp.Key);
            }
        }

        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            return dict.TryGetValue(key, out value) ? value : default(TValue);
        }

        public static IEnumerable<T> DistinctBy<T, TProp>(this IEnumerable<T> input, Func<T, TProp> propSelector)
        {
            var keys = new HashSet<TProp>();
            return input.Where(x => keys.Add(propSelector(x)));
        }

        public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList();
        }
    }
}
