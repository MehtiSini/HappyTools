using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HappyTools.Utilities.Extensions;
using HappyTools.Utilities.Extensions.Collections;

namespace HappyTools.Utilities.Extensions
{
    public class Node<T>
    {
        internal Node() { }

        public int Level { get; internal set; }
        public Node<T> Parent { get; internal set; }
        public T Item { get; internal set; }
        public IList<Node<T>> Children { get; internal set; }
    }
    public static class CollectionExtensions
    {
        public static IEnumerable<T> InsertOrMergeFirst<T>(this IEnumerable<T> obj, T value) where T : IEquatable<T>
        {
            return obj.InsertOrMerge(0, value);
        }
        public static IEnumerable<T> InsertOrMerge<T>(this IEnumerable<T> obj, int index, T value) where T : IEquatable<T>
        {
            List<T> list = null;
            if (obj == null)
                list = new List<T>();
            else
                list = obj.ToList();
            if (value == null)
                return list;
            if (!value.Equals(list.FirstOrDefault()))
                list.Insert(index, value);
            return list;
        }
        public static bool ValueEquals(this Array array1, Array array2)
        {
            if (array1 == null && array2 == null)
            {
                return true;
            }

            if (array1 == null || array2 == null)
            {
                return false;
            }

            if (array1.Length != array2.Length)
            {
                return false;
            }
            if (array1.Equals(array2))
            {
                return true;
            }
            else
            {
                for (var Index = 0; Index < array1.Length; Index++)
                {
                    if (!Equals(array1.GetValue(Index), array2.GetValue(Index)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public static IEnumerable<Node<T>> ByHierarchy<T>(
            this IEnumerable<T> source,
            Func<T, bool> startWith,
            Func<T, T, bool> connectBy)
        {
            return source.ByHierarchy(startWith, connectBy, null);
        }

        private static IEnumerable<Node<T>> ByHierarchy<T>(
            this IEnumerable<T> source,
            Func<T, bool> startWith,
            Func<T, T, bool> connectBy,
            Node<T> parent)
        {
            var level = parent == null ? 0 : parent.Level + 1;

            if (source == null)
                throw new ArgumentNullException("source");

            if (startWith == null)
                throw new ArgumentNullException("startWith");

            if (connectBy == null)
                throw new ArgumentNullException("connectBy");

            foreach (var value in from item in source
                                  where startWith(item)
                                  select item)
            {
                var children = new List<Node<T>>();
                var newNode = new Node<T>
                {
                    Level = level,
                    Parent = parent,
                    Item = value,
                    Children = children.AsReadOnly()
                };

                foreach (var subNode in source.ByHierarchy(possibleSub => connectBy(value, possibleSub),
                                                                  connectBy, newNode))
                {
                    children.Add(subNode);
                }

                yield return newNode;
            }
        }
        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> coll)
        {
            return new ObservableCollection<T>(coll);
        }
        public static void DumpHierarchy<T>(this IEnumerable<Node<T>> nodes, Func<T, string> display)
        {
            DumpHierarchy(nodes, display, 0);
        }

        private static void DumpHierarchy<T>(IEnumerable<Node<T>> nodes, Func<T, string> display, int level)
        {
            foreach (var node in nodes)
            {
                for (var i = 0; i < level; i++) Console.Write("  ");
                Console.WriteLine(display(node.Item));
                if (node.Children != null)
                    DumpHierarchy(node.Children, display, level + 1);
            }
        }
        /// <summary>
        /// Takes a LINQ GroupBy value and turns it into a set of GroupedObservableCollection objects.
        /// </summary>
        /// <returns>The grouped observable.</returns>
        /// <param name="group">Group.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static IEnumerable<GroupedObservableCollection<TKey, TValue>> ToGroupedObservable<TKey, TValue>(
           this IEnumerable<IGrouping<TKey, TValue>> group)
        {
            foreach (var item in group)
            {
                yield return new GroupedObservableCollection<TKey, TValue>(item.Key, item);
            }
        }

        /// <summary>
        /// Returns an ObservableCollection from a set of enumerable items.
        /// </summary>
        /// <returns>The observable collection.</returns>
        /// <param name="items">Items.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static OptimizedObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            return new OptimizedObservableCollection<T>(items);
        }

        /// <summary>
        /// Perform a sort of the items in a collection. This is useful
        /// if the underlying collection does not support sorting. 
        /// </summary>
        /// <param name="collection">Underlying collection to sort</param>
        /// <param name="comparer">Comparer delegate</param>
        /// <param name="reverse">True to reverse the collection</param>
        public static void BubbleSort<T>(this IList<T> collection, Func<T, T, int> comparer, bool reverse = false)
        {
            for (var index = collection.Count - 1; index >= 0; index--)
            {
                for (var child = 1; child <= index; child++)
                {
                    var d1 = collection[child - 1];
                    var d2 = collection[child];

                    var result = !reverse ? comparer(d1, d2) : comparer(d2, d1);
                    if (result > 0)
                    {
                        collection.Remove(d1);
                        collection.Insert(child, d1);
                    }
                }
            }
        }

        /// <summary>
        /// Perform a sort of the items in a collection. This is useful
        /// if the underlying collection does not support sorting. Note that
        /// the object type must be comparable.
        /// </summary>
        /// <param name="collection">Underlying collection to sort</param>
        /// <param name="comparer">Comparer interface</param>
        /// <param name="reverse">True to reverse the collection</param>
        public static void BubbleSort(this IList collection, IComparer comparer, bool reverse = false)
        {
            for (var index = collection.Count - 1; index >= 0; index--)
            {
                for (var child = 1; child <= index; child++)
                {
                    var d1 = collection[child - 1];
                    var d2 = collection[child];

                    var result = !reverse
                        ? comparer.Compare(d1, d2)
                        : comparer.Compare(d2, d1);

                    if (result > 0)
                    {
                        collection.Remove(d1);
                        collection.Insert(child, d1);
                    }
                }
            }
        }

        /// <summary>
        /// This is used to compare two collections.
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection Source</param>
        /// <param name="other">Collection to compare to</param>
        /// <param name="sameBuyRequestRequired">Require same-BuyRequest elements (exact match)</param>
        /// <returns></returns>
        public static bool Compare<T>(this ICollection<T> collection, ICollection<T> other, bool sameBuyRequestRequired = false)
        {
            if (!ReferenceEquals(collection, other))
            {
                if (other == null)
                    throw new ArgumentNullException("other");

                // Not the same number of elements.  No match
                if (collection.Count != other.Count)
                    return false;

                // Require same-BuyRequest; just defer to existing LINQ match
                if (sameBuyRequestRequired)
                    return collection.SequenceEqual(other);

                // Otherwise allow it to be any BuyRequest, but require same count of each item type.
                var comparer = EqualityComparer<T>.Default;
                return !(from item in collection
                         let thisItem = item
                         where !other.Contains(item, comparer) || collection.Count(check => comparer.Equals(thisItem, check)) != other.Count(check => comparer.Equals(thisItem, check))
                         select item).Any();
            }

            return true;
        }

        /// <summary>
        /// Add a range of IEnumerable collection to an existing Collection.
        /// </summary>
        ///<typeparam name="T">Type of collection</typeparam>
        ///<param name="collection">Collection</param>
        /// <param name="items">Items to add</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
                collection.Add(item);
        }

        /// <summary>
        /// Removes a set of items from the collection.
        /// </summary>
        /// <param name="collection">Collection to remove from</param>
        /// <param name="items">Items to remove from collection.</param>
        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
                collection.Remove(item);
        }

        ///<summary>
        /// This method tests an enumerable sequence and returns the index of the first item that
        /// passes the test.
        ///</summary>
        ///<typeparam name="T">Type of collection</typeparam>
        ///<param name="collection">Collection</param>
        ///<param name="test">Predicate test</param>
        ///<returns>Index (zero based) of first element that passed test, -1 if none did</returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, Predicate<T> test)
        {
            var pos = 0;
            foreach (var item in collection)
            {
                if (test(item))
                    return pos;
                pos++;
            }
            return -1;
        }

        /// <summary>
        /// Swap a value in the collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">Source collection</param>
        /// <param name="sourceIndex">Index</param>
        /// <param name="destIndex">Dest index</param>
        public static void Swap<T>(this IList<T> collection, int sourceIndex, int destIndex)
        {
            // Simple parameter checking
            if (sourceIndex < 0 || sourceIndex >= collection.Count)
                throw new ArgumentOutOfRangeException("sourceIndex");
            if (destIndex < 0 || destIndex >= collection.Count)
                throw new ArgumentOutOfRangeException("destIndex");

            // Ignore if same index
            if (sourceIndex == destIndex)
                return;

            var temp = collection[sourceIndex];
            collection[sourceIndex] = collection[destIndex];
            collection[destIndex] = temp;
        }

        /// <summary>
        /// This method moves a range of values in the collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">Source collection</param>
        /// <param name="startingIndex">Index</param>
        /// <param name="count">Count of items</param>
        /// <param name="destIndex">Dest index</param>
        public static void MoveRange<T>(this IList<T> collection, int startingIndex, int count, int destIndex)
        {
            // Simple parameter checking
            if (startingIndex < 0 || startingIndex >= collection.Count)
                throw new ArgumentOutOfRangeException("startingIndex");
            if (destIndex < 0 || destIndex >= collection.Count)
                throw new ArgumentOutOfRangeException("destIndex");
            if (startingIndex + count > collection.Count)
                throw new ArgumentOutOfRangeException("count");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            // Ignore if same index or count is zero
            if (startingIndex == destIndex || count == 0)
                return;

            // Make sure we can modify this directly
            if (collection.GetType().IsArray)
                throw new NotSupportedException("Collection is fixed-size and items cannot be efficiently moved.");

            // Go through the collection element-by-element
            var range = Enumerable.Range(0, count);
            if (startingIndex < destIndex)
                range = range.Reverse();

            foreach (var i in range)
            {
                var start = startingIndex + i;
                var dest = destIndex + i;

                var item = collection[start];
                collection.RemoveAt(start);
                collection.Insert(dest, item);
            }
        }
        /// <summary>
        /// Removes matching items from a sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        /// 
        /// <remarks>
        /// 	Renamed by James Curran, to match corresponding HashSet.RemoveWhere()
        /// 	</remarks>

        public static void RemoveWhere<T>(this ICollection<T> source, Predicate<T> predicate)
        {
            if (source == null)
                return;
            var all = source.Where(t => predicate(t)).ToList();
            all.ForEach(p => source.Remove(p));
            all = null;
        }
    }
}
