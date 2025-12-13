namespace HappyTools.Utilities.Extensions
{
    /// <summary>
    /// 	Universal conversion and parsing methods for strings.
    /// 	These methods are avaiblable throught the generic object.ConvertTo method:
    /// 	Feel free to provide additional converns for string or any other object type.
    /// </summary>
    /// <example>
    /// 	<code>
    /// 		var value = "123";
    /// 		var numeric = value.ConvertTo().ToInt32();
    /// 	</code>
    /// </example>
    public static class StringConverter
    {
        /// <summary>
        /// 	Converts a string to an Int32 value
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        /// <example>
        /// 	<code>
        /// 		var value = "123";
        /// 		var numeric = value.ConvertTo().ToInt32();
        /// 	</code>
        /// </example>
        public static int ToInt32(this IConverter<string> value)
        {
            return value.ToInt32(0, false);
        }

        /// <summary>
        /// 	Converts a string to an Int32 value
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <param name = "ignoreException">if set to <c>true</c> any parsing exception will be ignored.</param>
        /// <returns></returns>
        /// <example>
        /// 	<code>
        /// 		var value = "123";
        /// 		var numeric = value.ConvertTo().ToInt32();
        /// 	</code>
        /// </example>
        public static int ToInt32(this IConverter<string> value, int defaultValue, bool ignoreException)
        {
            if (ignoreException)
            {
                try
                {
                    return value.ToInt32(defaultValue, false);
                }
                catch
                { }
                return defaultValue;
            }

            return int.Parse(value.Value);
        }


    }
}
