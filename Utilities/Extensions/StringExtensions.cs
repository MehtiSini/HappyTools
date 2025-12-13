using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HappyTools.Utilities.Extensions;
using Nozhan.Utility.Core.Resources;

namespace HappyTools.Utilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the input string with the first character converted to uppercase
        /// </summary>
        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length == 1) return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }
        /// <summary>
        /// Returns the input string with the first character converted to lowercase
        /// </summary>
        public static string FirstLetterToLowerCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length == 1) return s.ToUpper();
            return s.Remove(1).ToLower() + s.Substring(1);
        }

        #region Common string extensions

        /// <summary>
        /// 	Determines whether the specified string is null or empty.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        public static bool IsEmpty(this string value)
        {
            return value == null || value.Length == 0;
        }

        /// <summary>
        /// 	Determines whether the specified string is not null or empty.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        public static bool IsNotEmpty(this string value)
        {
            return value.IsEmpty() == false;
        }

        /// <summary>
        /// 	Checks whether the string is empty and returns a default value in case.
        /// </summary>
        /// <param name = "value">The string to check.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfEmpty(this string value, string defaultValue)
        {
            return value.IsNotEmpty() ? value : defaultValue;
        }

        /// <summary>
        /// 	Formats the value with the parameters using string.Format.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormatWith(this string value, params object[] parameters)
        {
            return string.Format(value, parameters);
        }

        /// <summary>
        /// 	Trims the text to a provided maximum length.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "maxLength">Maximum length.</param>
        /// <returns></returns>
        /// <remarks>
        /// 	Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength)
        {
            return value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// 	Trims the text to a provided maximum length and adds a suffix if required.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "maxLength">Maximum length.</param>
        /// <param name = "suffix">The suffix.</param>
        /// <returns></returns>
        /// <remarks>
        /// 	Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength, string suffix)
        {
            return value == null || value.Length <= maxLength ? value : string.Concat(value.Substring(0, maxLength), suffix);
        }

        /// <summary>
        /// 	Determines whether the comparison value strig is contained within the input value string
        /// </summary>
        /// <param name = "inputValue">The input value.</param>
        /// <param name = "comparisonValue">The comparison value.</param>
        /// <param name = "comparisonType">Type of the comparison to allow case sensitive or insensitive comparison.</param>
        /// <returns>
        /// 	<c>true</c> if input value contains the specified value, otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string inputValue, string comparisonValue, StringComparison comparisonType)
        {
            return inputValue.IndexOf(comparisonValue, comparisonType) != -1;
        }

        /// <summary>
        /// 	Determines whether the comparison value string is contained within the input value string without any
        ///     consideration about the case (<see cref="StringComparison.InvariantCultureIgnoreCase"/>).
        /// </summary>
        /// <param name = "inputValue">The input value.</param>
        /// <param name = "comparisonValue">The comparison value.  Case insensitive</param>
        /// <returns>
        /// 	<c>true</c> if input value contains the specified value (case insensitive), otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsEquivalenceTo(this string inputValue, string comparisonValue)
        {
            return BothStringsAreEmpty(inputValue, comparisonValue) || StringContainsEquivalence(inputValue, comparisonValue);
        }

        /// <summary>
        /// Centers a charters in this string, padding in both, left and right, by specified Unicode character,
        /// for a specified total lenght.
        /// </summary>
        /// <param name="value">Instance value.</param>
        /// <param name="width">The number of characters in the resulting string, 
        /// equal to the number of original characters plus any additional padding characters.
        /// </param>
        /// <param name="padChar">A Unicode padding character.</param>
        /// <param name="truncate">Should get only the substring of specified width if string width is 
        /// more than the specified width.</param>
        /// <returns>A new string that is equivalent to this instance, 
        /// but center-aligned with as many paddingChar characters as needed to create a 
        /// length of width paramether.</returns>
        public static string PadBoth(this string value, int width, char padChar, bool truncate = false)
        {
            var diff = width - value.Length;
            if (diff == 0 || diff < 0 && !truncate)
            {
                return value;
            }
            else if (diff < 0)
            {
                return value.Substring(0, width);
            }
            else
            {
                return value.PadLeft(width - diff / 2, padChar).PadRight(width, padChar);
            }
        }

        /// <summary>
        /// 	Loads the string into a LINQ to XML XDocument
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML document object model (XDocument)</returns>
        public static XDocument ToXDocument(this string xml)
        {
            return XDocument.Parse(xml);
        }


        /// <summary>
        ///     Loads the string into a LINQ to XML XElement
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML element object model (XElement)</returns>
        public static XElement ToXElement(this string xml)
        {
            return XElement.Parse(xml);
        }

        /// <summary>
        /// 	Reverses / mirrors a string.
        /// </summary>
        /// <param name = "value">The string to be reversed.</param>
        /// <returns>The reversed string</returns>
        public static string Reverse(this string value)
        {
            if (value.IsEmpty() || value.Length == 1)
                return value;

            var chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// 	Ensures that a string starts with a given prefix.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        /// <param name = "prefix">The prefix value to check for.</param>
        /// <returns>The string value including the prefix</returns>
        /// <example>
        /// 	<code>
        /// 		var extension = "txt";
        /// 		var fileName = string.Concat(file.Name, extension.EnsureStartsWith("."));
        /// 	</code>
        /// </example>
        public static string EnsureStartsWith(this string value, string prefix)
        {
            return value.StartsWith(prefix) ? value : string.Concat(prefix, value);
        }

        /// <summary>
        /// 	Ensures that a string ends with a given suffix.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        /// <param name = "suffix">The suffix value to check for.</param>
        /// <returns>The string value including the suffix</returns>
        /// <example>
        /// 	<code>
        /// 		var url = "http://www.pgk.de";
        /// 		url = url.EnsureEndsWith("/"));
        /// 	</code>
        /// </example>
        public static string EnsureEndsWith(this string value, string suffix)
        {
            return value.EndsWith(suffix) ? value : string.Concat(value, suffix);
        }

        /// <summary>
        /// 	Repeats the specified string value as provided by the repeat count.
        /// </summary>
        /// <param name = "value">The original string.</param>
        /// <param name = "repeatCount">The repeat count.</param>
        /// <returns>The repeated string</returns>
        public static string Repeat(this string value, int repeatCount)
        {
            if (value.Length == 1)
                return new string(value[0], repeatCount);

            var sb = new StringBuilder(repeatCount * value.Length);
            while (repeatCount-- > 0)
                sb.Append(value);
            return sb.ToString();
        }

        /// <summary>
        /// 	Tests whether the contents of a string is a numeric value
        /// </summary>
        /// <param name = "value">String to check</param>
        /// <returns>
        /// 	Boolean indicating whether or not the string contents are numeric
        /// </returns>
        /// <remarks>
        /// 	Contributed by Kenneth Scott
        /// </remarks>
        public static bool IsNumeric(this string value)
        {
            float output;
            return float.TryParse(value, out output);
        }

        /// <summary>
        /// 	Extracts all digits from a string.
        /// </summary>
        /// <param name = "value">String containing digits to extract</param>
        /// <returns>
        /// 	All digits contained within the input string
        /// </returns>
        /// <remarks>
        /// 	Contributed by Kenneth Scott
        /// </remarks>

        public static string ExtractDigits(this string value)
        {
            return value.Where(char.IsDigit).Aggregate(new StringBuilder(value.Length), (sb, c) => sb.Append(c)).ToString();
        }
        /// <summary>
        /// 	Concatenates the specified string value with the passed additional strings.
        /// </summary>
        /// <param name = "value">The original value.</param>
        /// <param name = "values">The additional string values to be concatenated.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatWith(this string value, params string[] values)
        {
            return string.Concat(value, string.Concat(values));
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuid(this string value)
        {
            return new Guid(value);
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value and returns Guid.Empty if conversion fails.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuidSave(this string value)
        {
            return value.ToGuidSave(Guid.Empty);
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value and returns the provided default value if the conversion fails.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuidSave(this string value, Guid defaultValue)
        {
            if (value.IsEmpty())
                return defaultValue;

            try
            {
                return value.ToGuid();
            }
            catch { }

            return defaultValue;
        }

        /// <summary>
        /// 	Gets the string before the given string parameter.
        /// </summary>
        /// <param name = "value">The default value.</param>
        /// <param name = "x">The given string parameter.</param>
        /// <returns></returns>
        /// <remarks>Unlike GetBetween and GetAfter, this does not Trim the result.</remarks>
        public static string GetBefore(this string value, string x)
        {
            var xPos = value.IndexOf(x);
            return xPos == -1 ? string.Empty : value.Substring(0, xPos);
        }

        /// <summary>
        /// 	Gets the string between the given string parameters.
        /// </summary>
        /// <param name = "value">The source value.</param>
        /// <param name = "x">The left string sentinel.</param>
        /// <param name = "y">The right string sentinel</param>
        /// <returns></returns>
        /// <remarks>Unlike GetBefore, this method trims the result</remarks>
        public static string GetBetween(this string value, string x, string y)
        {
            var xPos = value.IndexOf(x);
            var yPos = value.LastIndexOf(y);

            if (xPos == -1 || xPos == -1)
                return string.Empty;

            var startIndex = xPos + x.Length;
            return startIndex >= yPos ? string.Empty : value.Substring(startIndex, yPos - startIndex).Trim();
        }

        /// <summary>
        /// 	Gets the string after the given string parameter.
        /// </summary>
        /// <param name = "value">The default value.</param>
        /// <param name = "x">The given string parameter.</param>
        /// <returns></returns>
        /// <remarks>Unlike GetBefore, this method trims the result</remarks>
        public static string GetAfter(this string value, string x)
        {
            var xPos = value.LastIndexOf(x);

            if (xPos == -1)
                return string.Empty;

            var startIndex = xPos + x.Length;
            return startIndex >= value.Length ? string.Empty : value.Substring(startIndex).Trim();
        }

        /// <summary>
        /// 	A generic version of System.String.Join()
        /// </summary>
        /// <typeparam name = "T">
        /// 	The type of the array to join
        /// </typeparam>
        /// <param name = "separator">
        /// 	The separator to appear between each element
        /// </param>
        /// <param name = "value">
        /// 	An array of values
        /// </param>
        /// <returns>
        /// 	The join.
        /// </returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Join<T>(string separator, T[] value)
        {
            if (value == null || value.Length == 0)
                return string.Empty;
            if (separator == null)
                separator = string.Empty;
            var arr = value.Select(p => p.ToString());
            return string.Join(separator, arr);
        }

        /// <summary>
        /// 	Remove any instance of the given character from the current string.
        /// </summary>
        /// <param name = "value">
        /// 	The input.
        /// </param>
        /// <param name = "removeCharc">
        /// 	The remove char.
        /// </param>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params char[] removeCharc)
        {
            var result = value;
            if (!string.IsNullOrEmpty(result) && removeCharc != null)
                removeCharc.ForEach(c => result = result.Remove(c.ToString()));

            return result;

        }

        /// <summary>
        /// Remove any instance of the given string pattern from the current string.
        /// </summary>
        /// <param name="value">The input.</param>
        /// <param name="strings">The strings.</param>
        /// <returns></returns>
        /// <remarks>
        /// Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params string[] strings)
        {
            return strings.Aggregate(value, (current, c) => current.Replace(c, string.Empty));
        }

        /// <summary>Finds out if the specified string contains null, empty or consists only of white-space characters</summary>
        /// <param name = "value">The input string</param>
        public static bool IsEmptyOrWhiteSpace(this string value)
        {
            return value.IsEmpty() || value.All(t => char.IsWhiteSpace(t));
        }

        /// <summary>Determines whether the specified string is not null, empty or consists only of white-space characters</summary>
        /// <param name = "value">The string value to check</param>
        public static bool IsNotEmptyOrWhiteSpace(this string value)
        {
            return value.IsEmptyOrWhiteSpace() == false;
        }

        /// <summary>Checks whether the string is null, empty or consists only of white-space characters and returns a default value in case</summary>
        /// <param name = "value">The string to check</param>
        /// <param name = "defaultValue">The default value</param>
        /// <returns>Either the string or the default value</returns>
        public static string IfEmptyOrWhiteSpace(this string value, string defaultValue)
        {
            return value.IsEmptyOrWhiteSpace() ? defaultValue : value;
        }

        /// <summary>Uppercase First Letter</summary>
        /// <param name = "value">The string value to process</param>
        public static string ToUpperFirstLetter(this string value)
        {
            if (value.IsEmptyOrWhiteSpace()) return string.Empty;

            var valueChars = value.ToCharArray();
            valueChars[0] = char.ToUpper(valueChars[0]);

            return new string(valueChars);
        }

        /// <summary>
        /// Returns the left part of the string.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <param name="characterCount">The character count to be returned.</param>
        /// <returns>The left part</returns>
        public static string Left(this string value, int characterCount)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (characterCount >= value.Length)
                throw new ArgumentOutOfRangeException("characterCount", characterCount, "characterCount must be less than length of string");
            return value.Substring(0, characterCount);
        }

        /// <summary>
        /// Returns the Right part of the string.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <param name="characterCount">The character count to be returned.</param>
        /// <returns>The right part</returns>
        public static string Right(this string value, int characterCount)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (characterCount >= value.Length)
                throw new ArgumentOutOfRangeException("characterCount", characterCount, "characterCount must be less than length of string");
            return value.Substring(value.Length - characterCount);
        }

        /// <summary>Returns the right part of the string from index.</summary>
        /// <param name="value">The original value.</param>
        /// <param name="index">The start index for substringing.</param>
        /// <returns>The right part.</returns>
        public static string SubstringFrom(this string value, int index)
        {
            return index < 0 ? value : value.Substring(index, value.Length - index);
        }

        //todo: xml documentation requires
        //todo: unit test required
        public static byte[] GetBytes(this string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static byte[] GetBytes(this string data, Encoding encoding)
        {
            return encoding.GetBytes(data);
        }



        public static string ToPlural(this string singular)
        {
            // Multiple words in the form A of B : Apply the plural to the first word only (A)
            var index = singular.LastIndexOf(" of ");
            if (index > 0) return singular.Substring(0, index) + singular.Remove(0, index).ToPlural();

            // single Word rules
            //sibilant ending rule
            if (singular.EndsWith("sh")) return singular + "es";
            if (singular.EndsWith("ch")) return singular + "es";
            if (singular.EndsWith("us")) return singular + "es";
            if (singular.EndsWith("ss")) return singular + "es";
            //-ies rule
            if (singular.EndsWith("y")) return singular.Remove(singular.Length - 1, 1) + "ies";
            // -oes rule
            if (singular.EndsWith("o")) return singular.Remove(singular.Length - 1, 1) + "oes";
            // -s suffix rule
            return singular + "s";
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s)
        {
            return s.ToHtmlSafe(false, false);
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <param name="all">Whether to make all characters entities or just those needed.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s, bool all)
        {
            return s.ToHtmlSafe(all, false);
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <param name="all">Whether to make all characters entities or just those needed.</param>
        /// <param name="replace">Whether or not to encode spaces and line breaks.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s, bool all, bool replace)
        {
            if (s.IsEmptyOrWhiteSpace())
                return string.Empty;
            var entities = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 28, 29, 30, 31, 34, 39, 38, 60, 62, 123, 124, 125, 126, 127, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 215, 247, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 8704, 8706, 8707, 8709, 8711, 8712, 8713, 8715, 8719, 8721, 8722, 8727, 8730, 8733, 8734, 8736, 8743, 8744, 8745, 8746, 8747, 8756, 8764, 8773, 8776, 8800, 8801, 8804, 8805, 8834, 8835, 8836, 8838, 8839, 8853, 8855, 8869, 8901, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 977, 978, 982, 338, 339, 352, 353, 376, 402, 710, 732, 8194, 8195, 8201, 8204, 8205, 8206, 8207, 8211, 8212, 8216, 8217, 8218, 8220, 8221, 8222, 8224, 8225, 8226, 8230, 8240, 8242, 8243, 8249, 8250, 8254, 8364, 8482, 8592, 8593, 8594, 8595, 8596, 8629, 8968, 8969, 8970, 8971, 9674, 9824, 9827, 9829, 9830 };
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (all || entities.Contains(c))
                    sb.Append("&#" + (int)c + ";");
                else
                    sb.Append(c);
            }

            return replace ? sb.Replace("", "<br />").Replace("\n", "<br />").Replace(" ", "&nbsp;").ToString() : sb.ToString();
        }

        /// <summary>
        /// Returns true if strings are equals, without consideration to case (<see cref="StringComparison.InvariantCultureIgnoreCase"/>)
        /// </summary>
        public static bool EquivalentTo(this string s, string whateverCaseString)
        {
            return string.Equals(s, whateverCaseString, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Replace all values in string
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="oldValues">List of old values, which must be replaced</param>
        /// <param name="replacePredicate">Function for replacement old values</param>
        /// <returns>Returns new string with the replaced values</returns>
        /// <example>
        /// 	<code>
        ///         var str = "White Red Blue Green Yellow Black Gray";
        ///         var achromaticColors = new[] {"White", "Black", "Gray"};
        ///         str = str.ReplaceAll(achromaticColors, v => "[" + v + "]");
        ///         // str == "[White] Red Blue Green Yellow [Black] [Gray]"
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static string ReplaceAll(this string value, IEnumerable<string> oldValues, Func<string, string> replacePredicate)
        {
            var sbStr = new StringBuilder(value);
            foreach (var oldValue in oldValues)
            {
                var newValue = replacePredicate(oldValue);
                sbStr.Replace(oldValue, newValue);
            }

            return sbStr.ToString();
        }

        /// <summary>
        /// Replace all values in string
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="oldValues">List of old values, which must be replaced</param>
        /// <param name="newValue">New value for all old values</param>
        /// <returns>Returns new string with the replaced values</returns>
        /// <example>
        /// 	<code>
        ///         var str = "White Red Blue Green Yellow Black Gray";
        ///         var achromaticColors = new[] {"White", "Black", "Gray"};
        ///         str = str.ReplaceAll(achromaticColors, "[AchromaticColor]");
        ///         // str == "[AchromaticColor] Red Blue Green Yellow [AchromaticColor] [AchromaticColor]"
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static string ReplaceAll(this string value, IEnumerable<string> oldValues, string newValue)
        {
            var sbStr = new StringBuilder(value);
            foreach (var oldValue in oldValues)
                sbStr.Replace(oldValue, newValue);

            return sbStr.ToString();
        }

        /// <summary>
        /// Replace all values in string
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="oldValues">List of old values, which must be replaced</param>
        /// <param name="newValues">List of new values</param>
        /// <returns>Returns new string with the replaced values</returns>
        /// <example>
        /// 	<code>
        ///         var str = "White Red Blue Green Yellow Black Gray";
        ///         var achromaticColors = new[] {"White", "Black", "Gray"};
        ///         var exquisiteColors = new[] {"FloralWhite", "Bistre", "DavyGrey"};
        ///         str = str.ReplaceAll(achromaticColors, exquisiteColors);
        ///         // str == "FloralWhite Red Blue Green Yellow Bistre DavyGrey"
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks> 
        public static string ReplaceAll(this string value, IEnumerable<string> oldValues, IEnumerable<string> newValues)
        {
            var sbStr = new StringBuilder(value);
            var newValueEnum = newValues.GetEnumerator();
            foreach (var old in oldValues)
            {
                if (!newValueEnum.MoveNext())
                    throw new ArgumentOutOfRangeException("newValues", "newValues sequence is shorter than oldValues sequence");
                sbStr.Replace(old, newValueEnum.Current);
            }
            if (newValueEnum.MoveNext())
                throw new ArgumentOutOfRangeException("newValues", "newValues sequence is longer than oldValues sequence");

            return sbStr.ToString();
        }

        #endregion
        #region Regex based extension methods

        /// <summary>
        /// 	Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var isMatching = s.IsMatchingTo(@"^\d+$");
        /// 	</code>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern)
        {
            return value.IsMatchingTo(regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var isMatching = s.IsMatchingTo(@"^\d+$");
        /// 	</code>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.IsMatch(value, regexPattern, options);
        }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "replaceValue">The replacement value.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue)
        {
            return value.ReplaceWith(regexPattern, replaceValue, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "replaceValue">The replacement value.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue, RegexOptions options)
        {
            return Regex.Replace(value, regexPattern, replaceValue, options);
        }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, MatchEvaluator evaluator)
        {
            return value.ReplaceWith(regexPattern, RegexOptions.None, evaluator);
        }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <param name = "evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, RegexOptions options, MatchEvaluator evaluator)
        {
            return Regex.Replace(value, regexPattern, evaluator, options);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <returns>A collection of all matches</returns>
        public static MatchCollection GetMatches(this string value, string regexPattern)
        {
            return value.GetMatches(regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>A collection of all matches</returns>
        public static MatchCollection GetMatches(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.Matches(value, regexPattern, options);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <returns>An enumeration of matching strings</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		foreach(var number in s.GetMatchingValues(@"\d")) {
        /// 		Console.WriteLine(number);
        /// 		}
        /// 	</code>
        /// </example>
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern)
        {
            return value.GetMatchingValues(regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>An enumeration of matching strings</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		foreach(var number in s.GetMatchingValues(@"\d")) {
        /// 		Console.WriteLine(number);
        /// 		}
        /// 	</code>
        /// </example>
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern, RegexOptions options)
        {
            foreach (Match match in value.GetMatches(regexPattern, options))
            {
                if (match.Success) yield return match.Value;
            }
        }

        /// <summary>
        /// 	Uses regular expressions to split a string into parts.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <returns>The splitted string array</returns>
        public static string[] Split(this string value, string regexPattern)
        {
            return value.Split(regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to split a string into parts.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>The splitted string array</returns>
        public static string[] Split(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.Split(value, regexPattern, options);
        }

        /// <summary>
        /// 	Splits the given string into words and returns a string array.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <returns>The splitted string array</returns>
        public static string[] GetWords(this string value)
        {
            return value.Split(@"\W");
        }
        /// <summary>
        /// extract numbers from a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns>a list of extracted numbers</returns>
        public static List<long> GetNumbers(this string str)
        {
            var nums = new List<long>();
            var start = -1;
            for (var i = 0; i < str.Length; i++)
            {
                if (start < 0 && char.IsDigit(str[i]))
                {
                    start = i;
                }
                else if (start >= 0 && !char.IsDigit(str[i]))
                {
                    nums.Add(long.Parse(str.Substring(start, i - start)));
                    start = -1;
                }
            }
            if (start >= 0)
                nums.Add(long.Parse(str.Substring(start, str.Length - start)));
            return nums;
        }
        /// <summary>
        /// 	Gets the nth "word" of a given string, where "words" are substrings separated by a given separator
        /// </summary>
        /// <param name = "value">The string from which the word should be retrieved.</param>
        /// <param name = "index">Index of the word (0-based).</param>
        /// <returns>
        /// 	The word at position n of the string.
        /// 	Trying to retrieve a word at a position lower than 0 or at a position where no word exists results in an exception.
        /// </returns>
        /// <remarks>
        /// 	Originally contributed by MMathews
        /// </remarks>
        public static string GetWordByIndex(this string value, int index)
        {
            var words = value.GetWords();

            if (index < 0 || index > words.Length - 1)
                throw new IndexOutOfRangeException("The word number is out of range.");

            return words[index];
        }

        /// <summary>
        /// Removed all special characters from the string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        [Obsolete("Please use RemoveAllSpecialCharacters instead")]
        public static string AdjustInput(this string value)
        {
            return string.Join(null, Regex.Split(value, "[^a-zA-Z0-9]"));
        }

        /// <summary>
        /// Removed all special characters from the string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran, James C, http://www.noveltheory.com
        /// 	This implementation is roughly equal to the original in speed, and should be more robust, internationally.
        /// </remarks>
        public static string RemoveAllSpecialCharacters(this string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var c in value.Where(c => char.IsLetterOrDigit(c)))
                sb.Append(c);
            return sb.ToString();
        }


        /// <summary>
        /// Add space on every upper character
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string SpaceOnUpper(this string value)
        {
            return Regex.Replace(value, "([A-Z])(?=[a-z])|(?<=[a-z])([A-Z]|[0-9]+)", " $1$2").TrimStart();
        }

        #region ExtractArguments extension

        /// <summary>
        /// Options to match the template with the original string
        /// </summary>
        public enum ComparsionTemplateOptions
        {
            /// <summary>
            /// Free template comparsion
            /// </summary>
            Default,

            /// <summary>
            /// Template compared from beginning of input string
            /// </summary>
            FromStart,

            /// <summary>
            /// Template compared with the end of input string
            /// </summary>
            AtTheEnd,

            /// <summary>
            /// Template compared whole with input string
            /// </summary>
            Whole,
        }

        private const RegexOptions _defaultRegexOptions = RegexOptions.None;
        private const ComparsionTemplateOptions _defaultComparsionTemplateOptions = ComparsionTemplateOptions.Default;
        private static readonly string[] _reservedRegexOperators = new[] { @"\", "^", "$", "*", "+", "?", ".", "(", ")" };

        private static string GetRegexPattern(string template, ComparsionTemplateOptions compareTemplateOptions)
        {
            template = template.ReplaceAll(_reservedRegexOperators, v => @"\" + v);

            var comparingFromStart = compareTemplateOptions == ComparsionTemplateOptions.FromStart ||
                                      compareTemplateOptions == ComparsionTemplateOptions.Whole;
            var comparingAtTheEnd = compareTemplateOptions == ComparsionTemplateOptions.AtTheEnd ||
                                     compareTemplateOptions == ComparsionTemplateOptions.Whole;
            var pattern = new StringBuilder();

            if (comparingFromStart)
                pattern.Append("^");

            pattern.Append(Regex.Replace(template, @"\{[0-9]+\}",
                                            delegate (Match m) {
                                                var argNum = m.ToString().Replace("{", "").Replace("}", "");
                                                return string.Format("(?<{0}>.*?)", int.Parse(argNum) + 1);
                                            }
                          ));

            if (comparingAtTheEnd || template.LastOrDefault() == '}' && compareTemplateOptions == ComparsionTemplateOptions.Default)
                pattern.Append("$");

            return pattern.ToString();
        }

        /// <summary>
        /// Extract arguments from string by template
        /// </summary>
        /// <param name="value">The input string. For example, "My name is Aleksey".</param>
        /// <param name="template">Template with arguments in the format {№}. For example, "My name is {0} {1}.".</param>
        /// <param name="compareTemplateOptions">Template options for compare with input string.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <returns>Returns parsed arguments.</returns>
        /// <example>
        /// 	<code>
        /// 		var str = "My name is Aleksey Nagovitsyn. I'm from Russia.";
        /// 		var args = str.ExtractArguments(@"My name is {1} {0}. I'm from {2}.");
        ///         // args[i] is [Nagovitsyn, Aleksey, Russia]
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static IEnumerable<string> ExtractArguments(this string value, string template,
                                                           ComparsionTemplateOptions compareTemplateOptions = _defaultComparsionTemplateOptions,
                                                           RegexOptions regexOptions = _defaultRegexOptions)
        {
            return value.ExtractGroupArguments(template, compareTemplateOptions, regexOptions).Select(g => g.Value);
        }

        /// <summary>
        /// Extract arguments as regex groups from string by template
        /// </summary>
        /// <param name="value">The input string. For example, "My name is Aleksey".</param>
        /// <param name="template">Template with arguments in the format {№}. For example, "My name is {0} {1}.".</param>
        /// <param name="compareTemplateOptions">Template options for compare with input string.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <returns>Returns parsed arguments as regex groups.</returns>
        /// <example>
        /// 	<code>
        /// 		var str = "My name is Aleksey Nagovitsyn. I'm from Russia.";
        /// 		var groupArgs = str.ExtractGroupArguments(@"My name is {1} {0}. I'm from {2}.");
        ///         // groupArgs[i].Value is [Nagovitsyn, Aleksey, Russia]
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static IEnumerable<Group> ExtractGroupArguments(this string value, string template,
                                                               ComparsionTemplateOptions compareTemplateOptions = _defaultComparsionTemplateOptions,
                                                               RegexOptions regexOptions = _defaultRegexOptions)
        {
            var pattern = GetRegexPattern(template, compareTemplateOptions);
            var regex = new Regex(pattern, regexOptions);
            var match = regex.Match(value);

            return Enumerable.Cast<Group>(match.Groups).Skip(1);
        }

        #endregion ExtractArguments extension

        #endregion
        #region Bytes & Base64

        /// <summary>
        /// 	Converts the string to a byte-array using the default encoding
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <returns>The created byte array</returns>
        public static byte[] ToBytes(this string value)
        {
            return value.ToBytes(null);
        }

        /// <summary>
        /// 	Converts the string to a byte-array using the supplied encoding
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "encoding">The encoding to be used.</param>
        /// <returns>The created byte array</returns>
        /// <example>
        /// 	<code>
        /// 		var value = "Hello World";
        /// 		var ansiBytes = value.ToBytes(Encoding.GetEncoding(1252)); // 1252 = ANSI
        /// 		var utf8Bytes = value.ToBytes(Encoding.UTF8);
        /// 	</code>
        /// </example>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 	Encodes the input value to a Base64 string using the default encoding.
        /// </summary>
        /// <param name = "value">The input value.</param>
        /// <returns>The Base 64 encoded string</returns>
        public static string EncodeBase64(this string value)
        {
            return value.EncodeBase64(null);
        }

        /// <summary>
        /// 	Encodes the input value to a Base64 string using the supplied encoding.
        /// </summary>
        /// <param name = "value">The input value.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns>The Base 64 encoded string</returns>
        public static string EncodeBase64(this string value, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 	Decodes a Base 64 encoded value to a string using the default encoding.
        /// </summary>
        /// <param name = "encodedValue">The Base 64 encoded value.</param>
        /// <returns>The decoded string</returns>
        public static string DecodeBase64(this string encodedValue)
        {
            return encodedValue.DecodeBase64(null);
        }

        /// <summary>
        /// 	Decodes a Base 64 encoded value to a string using the supplied encoding.
        /// </summary>
        /// <param name = "encodedValue">The Base 64 encoded value.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns>The decoded string</returns>
        public static string DecodeBase64(this string encodedValue, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            var bytes = Convert.FromBase64String(encodedValue);
            return encoding.GetString(bytes);
        }

        #endregion

        #region String to Enum

        /// <summary>
        ///     Parse a string to a enum item if that string exists in the enum otherwise return the default enum item.
        /// </summary>
        /// <typeparam name="TEnum">The Enum type.</typeparam>
        /// <param name="dataToMatch">The data will use to convert into give enum</param>
        /// <param name="ignorecase">Whether the enum parser will ignore the given data's case or not.</param>
        /// <returns>Converted enum.</returns>
        /// <example>
        /// 	<code>
        /// 		public enum EnumTwo {  None, One,}
        /// 		object[] items = new object[] { "One".ParseStringToEnum<EnumTwo>(), "Two".ParseStringToEnum<EnumTwo>() };
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static TEnum ParseStringToEnum<TEnum>(this string dataToMatch, bool ignorecase = default)
                where TEnum : struct
        {
            return dataToMatch.IsItemInEnum<TEnum>()() ? default : (TEnum)Enum.Parse(typeof(TEnum), dataToMatch, default);
        }

        /// <summary>
        ///     To check whether the data is defined in the given enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum will use to check, the data defined.</typeparam>
        /// <param name="dataToCheck">To match against enum.</param>
        /// <returns>Anonoymous method for the condition.</returns>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static Func<bool> IsItemInEnum<TEnum>(this string dataToCheck)
            where TEnum : struct
        {
            return () => { return string.IsNullOrEmpty(dataToCheck) || !Enum.IsDefined(typeof(TEnum), dataToCheck); };
        }

        #endregion

        private static bool StringContainsEquivalence(string inputValue, string comparisonValue)
        {
            return inputValue.IsNotEmptyOrWhiteSpace() && inputValue.Contains(comparisonValue, StringComparison.OrdinalIgnoreCase);
        }

        private static bool BothStringsAreEmpty(string inputValue, string comparisonValue)
        {
            return inputValue.IsEmptyOrWhiteSpace() && comparisonValue.IsEmptyOrWhiteSpace();
        }

        /// <summary>
        /// Return the string with the leftmost number_of_characters characters removed.
        /// </summary>
        /// <param name="str">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveLeft(this string str, int number_of_characters)
        {
            if (str.Length <= number_of_characters) return "";
            return str.Substring(number_of_characters);
        }

        /// <summary>
        /// Return the string with the rightmost number_of_characters characters removed.
        /// </summary>
        /// <param name="str">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveRight(this string str, int number_of_characters)
        {
            if (str.Length <= number_of_characters) return "";
            return str.Substring(0, str.Length - number_of_characters);
        }





        /// <summary>
        /// Use the password to generate key bytes and an initialization vector with Rfc2898DeriveBytes.
        /// </summary>
        /// <param name="password">The input password to use in generating the bytes.</param>
        /// <param name="salt">The input salt bytes to use in generating the bytes.</param>
        /// <param name="key_size_bits">The input size of the key to generate.</param>
        /// <param name="block_size_bits">The input block size used by the crypto provider.</param>
        /// <param name="key">The output key bytes to generate.</param>
        /// <param name="iv">The output initialization vector to generate.</param>
        /// <remarks></remarks>
        private static void MakeKeyAndIV(string password, byte[] salt, int key_size_bits, int block_size_bits,
                                         ref byte[] key, ref byte[] iv)
        {
            var derive_bytes =
                new Rfc2898DeriveBytes(password, salt, 1234);

            key = derive_bytes.GetBytes(key_size_bits / 8);
            iv = derive_bytes.GetBytes(block_size_bits / 8);
        }

        /// <summary>
        /// Convert a byte array into a hexadecimal string representation.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string BytesToHexString(this byte[] bytes)
        {
            var result = "";
            foreach (var b in bytes)
            {
                result += " " + b.ToString("X").PadLeft(2, '0');
            }
            if (result.Length > 0) result = result.Substring(1);
            return result;
        }

        /// <summary>
        /// Convert this string containing hexadecimal into a byte array.
        /// </summary>
        /// <param name="str">The hexadecimal string to convert.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] HexStringToBytes(this string str)
        {
            str = str.Replace(" ", "");
            var max_byte = str.Length / 2 - 1;
            var bytes = new byte[max_byte + 1];
            for (var i = 0; i <= max_byte; i++)
            {
                bytes[i] = byte.Parse(str.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);
            }

            return bytes;
        }

        /// <summary>
        /// Returns a default value if the string is null or empty.
        /// </summary>
        /// <param name="s">Original String</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string DefaultIfNullOrEmpty(this string s, string defaultValue)
        {
            return string.IsNullOrEmpty(s) ? defaultValue : s;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the string value is empty.
        /// </summary>
        /// <param name="obj">The value to test.</param>
        /// <param name="message">The message to display if the value is null.</param>
        /// <param name="name">The name of the parameter being tested.</param>
        public static string ExceptionIfNullOrEmpty(this string obj, string message, string name)
        {
            if (string.IsNullOrEmpty(obj))
                throw new ArgumentException(message, name);
            return obj;
        }

        /// <summary>
        /// Joins  the values of a string array if the values are not null or empty.
        /// </summary>
        /// <param name="objs">The string array used for joining.</param>
        /// <param name="separator">The separator to use in the joined string.</param>
        /// <returns></returns>
        public static string JoinNotNullOrEmpty(this string[] objs, string separator)
        {
            var items = new List<string>();
            foreach (var s in objs)
            {
                if (!string.IsNullOrEmpty(s))
                    items.Add(s);
            }
            return string.Join(separator, items.ToArray());
        }

        /// <summary>
        /// Parses the commandline params.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A StringDictionary type object of command line parameters.</returns>
        public static Dictionary<string, string> ParseCommandlineParams(this string[] value)
        {
            var parameters = new Dictionary<string, string>();
            var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string parameter = null;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
            foreach (var txt in value)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                var Parts = spliter.Split(txt, 3);
                switch (Parts.Length)
                {
                    // Found a value (for the last parameter found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!parameters.ContainsKey(parameter))
                            {
                                Parts[0] = remover.Replace(Parts[0], "$1");
                                parameters.Add(parameter, Parts[0]);
                            }
                            parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;
                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");
                        }
                        parameter = Parts[1];
                        break;
                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");
                        }
                        parameter = Parts[1];
                        // Remove possible enclosing characters (",')
                        if (!parameters.ContainsKey(parameter))
                        {
                            Parts[2] = remover.Replace(Parts[2], "$1");
                            parameters.Add(parameter, Parts[2]);
                        }
                        parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");
            }

            return parameters;
        }

        /// <summary>
        /// Encodes the email address so that the link is still valid, but the email address is useless for email harvsters.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        public static string EncodeEmailAddress(this string emailAddress)
        {
            int i;
            string repl;
            var tempHtmlEncode = emailAddress;
            for (i = tempHtmlEncode.Length; i >= 1; i--)
            {
                var acode = Convert.ToInt32(tempHtmlEncode[i - 1]);
                if (acode == 32)
                {
                    repl = " ";
                }
                else if (acode == 34)
                {
                    repl = "\"";
                }
                else if (acode == 38)
                {
                    repl = "&";
                }
                else if (acode == 60)
                {
                    repl = "<";
                }
                else if (acode == 62)
                {
                    repl = ">";
                }
                else if (acode >= 32 && acode <= 127)
                {
                    repl = "&#" + Convert.ToString(acode) + ";";
                }
                else
                {
                    repl = "&#" + Convert.ToString(acode) + ";";
                }
                if (repl.Length > 0)
                {
                    tempHtmlEncode = tempHtmlEncode.Substring(0, i - 1) +
                                     repl + tempHtmlEncode.Substring(i);
                }
            }
            return tempHtmlEncode;
        }

        /// <summary>
        /// Hash the string using MD5 algorithm
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static string MD5Hash(this string input)
        {
            using (var provider = MD5.Create())
            {
                var builder = new StringBuilder();

                foreach (var b in provider.ComputeHash(Encoding.UTF8.GetBytes(input)))
                    builder.Append(b.ToString("x2").ToLower());

                return builder.ToString();
            }

        }
        public static string HashWithSalt(this string input, string salt)
        {
            var bytes = Encoding.UTF8.GetBytes(input + salt);
            var sHA256ManagedString = new SHA256Managed();
            var hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        /// <summary>
        /// Generates a hash for the given plain text value and returns a
        /// base64-encoded result. Before the hash is computed, a random salt
        /// is generated and appended to the plain text. This salt is stored at
        /// the end of the hash value, so it can be used later for hash
        /// verification.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be hashed. The function does not check whether
        /// this parameter is null.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Name of the hash algorithm. Allowed values are: "MD5", "SHA1",
        /// "SHA256", "SHA384", and "SHA512" (if any other value is specified
        /// MD5 hashing algorithm will be used). This value is case-insensitive.
        /// </param>
        /// <param name="saltBytes">
        /// Salt bytes. This parameter can be null, in which case a random salt
        /// value will be generated.
        /// </param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        public static string ComputeHash(this string plainText,
                                         string hashAlgorithm,
                                         byte[] saltBytes)
        {
            // If salt is not specified, generate it on the fly.
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                var minSaltSize = 4;
                var maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                var random = new Random();
                var saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                var rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }

            // Convert plain text into a byte array.
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            var plainTextWithSaltBytes =
                    new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (var i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (var i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;

            // Make sure hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hash = new SHA1Managed();
                    break;

                case "SHA256":
                    hash = new SHA256Managed();
                    break;

                case "SHA384":
                    hash = new SHA384Managed();
                    break;

                case "SHA512":
                    hash = new SHA512Managed();
                    break;

                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            // Compute hash value of our plain text with appended salt.
            var hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            var hashWithSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (var i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (var i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            var hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }

        /// <summary>
        /// Compares a hash of the specified plain text value to a given hash
        /// value. Plain text is hashed with the same salt value as the original
        /// hash.
        /// </summary>
        /// <param name="plainText">
        /// Plain text to be verified against the specified hash. The function
        /// does not check whether this parameter is null.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Name of the hash algorithm. Allowed values are: "MD5", "SHA1", 
        /// "SHA256", "SHA384", and "SHA512" (if any other value is specified,
        /// MD5 hashing algorithm will be used). This value is case-insensitive.
        /// </param>
        /// <param name="hashValue">
        /// Base64-encoded hash value produced by ComputeHash function. This value
        /// includes the original salt appended to it.
        /// </param>
        /// <returns>
        /// If computed hash mathes the specified hash the function the return
        /// value is true; otherwise, the function returns false.
        /// </returns>
        public static bool VerifyHash(this string plainText,
                                      string hashAlgorithm,
                                      string hashValue)
        {
            // Convert base64-encoded hash value into a byte array.
            var hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;

            // Make sure that hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hashSizeInBits = 160;
                    break;

                case "SHA256":
                    hashSizeInBits = 256;
                    break;

                case "SHA384":
                    hashSizeInBits = 384;
                    break;

                case "SHA512":
                    hashSizeInBits = 512;
                    break;

                default: // Must be MD5
                    hashSizeInBits = 128;
                    break;
            }

            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            // Allocate array to hold original salt bytes retrieved from hash.
            var saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (var i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            // Compute a new hash string.
            var expectedHashString =
                        plainText.ComputeHash(hashAlgorithm, saltBytes);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return hashValue == expectedHashString;
        }
        /// <summary>
        /// Encrypt the given string using the specified key.
        /// </summary>
        /// <param name="strToEncrypt">The string to be encrypted.</param>
        /// <param name="strKey">The encryption key.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(this string strToEncrypt, string strKey)
        {
            try
            {
                var objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                var objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                var strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Encoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                    TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        /// <summary>
        /// Decrypt the given string using the specified key.
        /// </summary>
        /// <param name="strEncrypted">The string to be decrypted.</param>
        /// <param name="strKey">The decryption key.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(this string strEncrypted, string strKey)
        {
            try
            {
                var objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                var objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                var strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(strEncrypted);
                var strDecrypted = Encoding.ASCII.GetString
                (objDESCrypto.CreateDecryptor().TransformFinalBlock
                (byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        /// <summary>
        /// Convert fraction to its equivalent double value
        /// </summary>
        /// <param name="fraction"></param>
        /// <returns></returns>
        public static double FractionToDouble(this string fraction)
        {
            double result;

            if (double.TryParse(fraction, out result))
            {
                return result;
            }

            var split = fraction.Split(new char[] { ' ', '/' });

            if (split.Length == 2 || split.Length == 3)
            {
                int a, b;

                if (int.TryParse(split[0], out a) && int.TryParse(split[1], out b))
                {
                    if (split.Length == 2)
                    {
                        return (double)a / b;
                    }

                    int c;

                    if (int.TryParse(split[2], out c))
                    {
                        return a + (double)b / c;
                    }
                }
            }

            throw new Exception(string.Format(FrameworkValidationMessages.DataTypeFormatMismatchError, fraction, "Size"));
        }
        /// <summary>
        /// convert a double value to its equivalent fraction
        /// </summary>
        /// <param name="number"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static string DoubleToFractions(this double number, int precision = 4)
        {
            int w, n, d;
            RoundToMixedFraction(number, precision, out w, out n, out d);
            var ret = $"{w}";
            if (w > 0)
            {
                if (n > 0)
                    ret = $"{w} {n}/{d}";
            }
            else
            {
                if (n > 0)
                    ret = $"{n}/{d}";
            }
            return ret;
        }

        static void RoundToMixedFraction(double input, int accuracy, out int whole, out int numerator, out int denominator)
        {
            var dblAccuracy = (double)accuracy;
            whole = (int)Math.Truncate(input);
            var fraction = Math.Abs(input - whole);
            if (fraction == 0)
            {
                numerator = 0;
                denominator = 1;
                return;
            }
            var n = Enumerable.Range(0, accuracy + 1).SkipWhile(e => e / dblAccuracy < fraction).First();
            var hi = n / dblAccuracy;
            var lo = (n - 1) / dblAccuracy;
            if (fraction - lo < hi - fraction) n--;
            if (n == accuracy)
            {
                whole++;
                numerator = 0;
                denominator = 1;
                return;
            }
            var gcd = GCD(n, accuracy);
            numerator = n / gcd;
            denominator = accuracy / gcd;
        }

        static int GCD(int a, int b)
        {
            if (b == 0) return a;
            else return GCD(b, a % b);
        }

        /// <summary>
        /// Extract Numbers from string
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns><see cref="List{T}"/> of numbers found in input string</returns>
        public static List<long> ExtractNumbers(this string str)
        {
            var nums = new List<long>();
            var start = -1;
            for (var i = 0; i < str.Length; i++)
            {
                if (start < 0 && char.IsDigit(str[i]))
                {
                    start = i;
                }
                else if (start >= 0 && !char.IsDigit(str[i]))
                {
                    nums.Add(long.Parse(str.Substring(start, i - start)));
                    start = -1;
                }
            }
            if (start >= 0)
                nums.Add(long.Parse(str.Substring(start, str.Length - start)));
            return nums;
        }
        /// <summary>
        /// Extract Number from string
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>number found in input string</returns>
        public static long ExtractNumber(this string str)
        {
            var nums = new List<string>();
            var start = -1;
            for (var i = 0; i < str.Length; i++)
            {
                if (start < 0 && char.IsDigit(str[i]))
                {
                    start = i;
                }
                else if (start >= 0 && !char.IsDigit(str[i]))
                {
                    nums.Add(str.Substring(start, i - start));
                    start = -1;
                }
            }
            if (start >= 0)
                nums.Add(str.Substring(start, str.Length - start));
            var joined = string.Join("", nums);
            return long.Parse(joined);
        }
        /// <summary>
        /// Remove non numeric chars from string
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>number found in input string</returns>
        public static string RemoveNonNumericChars(this string str)
        {
            var nums = new List<string>();
            var start = -1;
            for (var i = 0; i < str.Length; i++)
            {
                if (start < 0 && char.IsDigit(str[i]))
                {
                    start = i;
                }
                else if (start >= 0 && !char.IsDigit(str[i]))
                {
                    nums.Add(str.Substring(start, i - start));
                    start = -1;
                }
            }
            if (start >= 0)
                nums.Add(str.Substring(start, str.Length - start));
            var joined = string.Join("", nums);
            return joined;
        }

        public static string ConvertPersianToEnglishNumbers(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] persianDigits = { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };
            char[] englishDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < persianDigits.Length; i++)
            {
                input = input.Replace(persianDigits[i], englishDigits[i]);
            }

            return input;
        }


    }
}
