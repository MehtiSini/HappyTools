namespace HappyTools.Utilities.Extensions
{
    /// <summary>
    /// 	Extension methods for the array data type
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>Performs the specified action on each element of the specified array.</summary>
        /// <param name="array">The one-dimensional, zero-based <see cref="T:System.Array" /> on whose elements the action is to be performed.</param>
        /// <param name="action">The <see cref="T:System.Action`1" /> to perform on each element of <paramref name="array" />.</param>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.-or-
        /// <paramref name="action" /> is <see langword="null" />.</exception>
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            for (var index = 0; index < array.Length; ++index)
                action(array[index]);
        }

        /////<summary>
        /////	Check if the array is null or empty
        /////</summary>
        /////<param name = "source"></param>
        /////<returns></returns>
        //public static bool IsNullOrEmpty(this Array source)
        //{
        //    return source != null ? source.Length <= 0 : false;
        //}

        ///<summary>
        ///	Check if the index is within the array
        ///</summary>
        ///<param name = "source"></param>
        ///<param name = "index"></param>
        ///<returns></returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static bool WithinIndex(this Array source, int index)
        {
            return source != null && index >= 0 && index < source.Length;
        }

        ///<summary>
        ///	Check if the index is within the array
        ///</summary>
        ///<param name = "source"></param>
        ///<param name = "index"></param>
        ///<param name="dimension"></param>
        ///<returns></returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static bool WithinIndex(this Array source, int index, int dimension = 0)
        {
            return source != null && index >= source.GetLowerBound(dimension) && index <= source.GetUpperBound(dimension);
        }


        /// <summary>
        /// Combine two arrays into one.
        /// </summary>
        /// <typeparam name="T">Type of Array</typeparam>
        /// <param name="combineWith">Base array in which arrayToCombine will add.</param>
        /// <param name="arrayToCombine">Array to combine with Base array.</param>
        /// <returns></returns>
        /// <example>
        /// 	<code>
        /// 		int[] arrayOne = new[] { 1, 2, 3, 4 };
        /// 		int[] arrayTwo = new[] { 5, 6, 7, 8 };
        /// 		Array combinedArray = arrayOne.CombineArray<int>(arrayTwo);
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static T[] CombineArray<T>(this T[] combineWith, T[] arrayToCombine)
        {
            if (combineWith != default(T[]) && arrayToCombine != default(T[]))
            {
                var initialSize = combineWith.Length;
                Array.Resize(ref combineWith, initialSize + arrayToCombine.Length);
                Array.Copy(arrayToCombine, arrayToCombine.GetLowerBound(0), combineWith, initialSize, arrayToCombine.Length);
            }
            return combineWith;
        }

        /// <summary>
        /// To clear the contents of the array.
        /// </summary>
        /// <param name="clear"> The array to clear</param>
        /// <returns>Cleared array</returns>
        /// <example>
        ///     <code>
        ///         Array array = Array.CreateInstance(typeof(string), 2);
        ///         array.SetValue("One", 0); array.SetValue("Two", 1);
        ///         Array arrayToClear = array.ClearAll();
        ///     </code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static Array ClearAll(this Array clear)
        {
            if (clear != null)
                Array.Clear(clear, 0, clear.Length);
            return clear;
        }

        /// <summary>
        /// To clear the contents of the array.
        /// </summary>
        /// <typeparam name="T">The type of array</typeparam>
        /// <param name="clear"> The array to clear</param>
        /// <returns>Cleared array</returns>
        /// <example>
        ///     <code>
        ///         int[] result = new[] { 1, 2, 3, 4 }.ClearAll<int>();
        ///     </code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static T[] ClearAll<T>(this T[] arrayToClear)
        {
            if (arrayToClear != null)
                for (var i = arrayToClear.GetLowerBound(0); i <= arrayToClear.GetUpperBound(0); ++i)
                    arrayToClear[i] = default;
            return arrayToClear;
        }

        /// <summary>
        /// To clear a specific item in the array.
        /// </summary>
        /// <param name="arrayToClear">The array in where to clean the item.</param>
        /// <param name="at">Which element to clear.</param>
        /// <returns></returns>
        /// <example>
        ///     <code>
        ///         Array array = Array.CreateInstance(typeof(string), 2);
        ///         array.SetValue("One", 0); array.SetValue("Two", 1);
        ///         Array result = array.ClearAt(2);
        ///     </code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static Array ClearAt(this Array arrayToClear, int at)
        {
            if (arrayToClear != null)
            {
                var arrayIndex = at.GetArrayIndex();
                if (arrayIndex.IsIndexInArray(arrayToClear))
                    Array.Clear(arrayToClear, arrayIndex, 1);
            }
            return arrayToClear;
        }

        /// <summary>
        /// To clear a specific item in the array.
        /// </summary>
        /// <typeparam name="T">The type of array</typeparam>
        /// <param name="arrayToClear">Array to clear.</param>
        /// <param name="at">Which element to clear.</param>
        /// <returns></returns>
        /// <example>
        ///     <code>
        ///           string[] clearString = new[] { "A" }.ClearAt<string>(0);
        ///     </code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static T[] ClearAt<T>(this T[] arrayToClear, int at)
        {
            if (arrayToClear != null)
            {
                var arrayIndex = at.GetArrayIndex();
                if (arrayIndex.IsIndexInArray(arrayToClear))
                    arrayToClear[arrayIndex] = default;
            }
            return arrayToClear;
        }

        /// <summary>
        /// Tests if the array is empty.
        /// </summary>
        /// <param name="array">The array to test.</param>
        /// <returns>True if the array is empty.</returns>
        public static bool IsEmpty(this Array array)
        {
            array.ExceptionIfNullOrEmpty(
                "The array cannot be null.",
                "array");

            return array.Length == 0;
        }

        /// <summary>
        /// Convert a value to an array with length of 1 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] ToSingleItemArray<T>(this T value)
        {
            var res = new T[] { value };
            return res;
        }
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            var walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (var i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (var i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (var j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
