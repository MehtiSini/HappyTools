using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HappyTools.Utilities.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Will try and parse an enum and it's default type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns>True if the enum value is defined.</returns>
        public static bool TryEnumIsDefined(Type type, object value)
        {
            if (type == null || value == null || !type.GetTypeInfo().IsEnum)
                return false;

            // Return true if the value is an enum and is a matching type.
            if (type == value.GetType())
                return true;

            if (TryEnumIsDefined<int>(type, value))
                return true;
            if (TryEnumIsDefined<string>(type, value))
                return true;
            if (TryEnumIsDefined<byte>(type, value))
                return true;
            if (TryEnumIsDefined<short>(type, value))
                return true;
            if (TryEnumIsDefined<long>(type, value))
                return true;
            if (TryEnumIsDefined<sbyte>(type, value))
                return true;
            if (TryEnumIsDefined<ushort>(type, value))
                return true;
            if (TryEnumIsDefined<uint>(type, value))
                return true;
            if (TryEnumIsDefined<ulong>(type, value))
                return true;

            return false;
        }

        public static bool TryEnumIsDefined<T>(Type type, object value)
        {
            // Catch any casting errors that can occur or if 0 is not defined as a default value.
            try
            {
                if (value is T && Enum.IsDefined(type, (T)value))
                    return true;
            }
            catch (Exception) { }

            return false;
        }

        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return ((int)(object)type & (int)(object)value) == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }


        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type | (int)(object)value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }


        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type & ~(int)(object)value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
        /// <summary>
        /// get underlying value of enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetUnderlyingValue(this Enum value)
        {
            var underlyingType = Enum.GetUnderlyingType(value.GetType());
            var res = Convert.ChangeType(value, underlyingType);
            return res;
        }
        /// <summary>
        /// Removes a flag and returns the new value
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="variable">Source enum</param>
        /// <param name="flag">Dumped flag</param>
        /// <returns>Result enum value</returns>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static T ClearFlag<T>(this Enum variable, T flag)
        {
            return variable.ClearFlags(flag);
        }

        /// <summary>
        /// Removes flags and returns the new value
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="variable">Source enum</param>
        /// <param name="flags">Dumped flags</param>
        /// <returns>Result enum value</returns>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static T ClearFlags<T>(this Enum variable, params T[] flags)
        {
            var result = Convert.ToUInt64(variable);
            foreach (var flag in flags)
                result &= ~Convert.ToUInt64(flag);
            return (T)Enum.Parse(variable.GetType(), result.ToString());
        }

        /// <summary>
        /// Includes a flag and returns the new value
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="variable">Source enum</param>
        /// <param name="flag">Established flag</param>
        /// <returns>Result enum value</returns>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static T SetFlag<T>(this Enum variable, T flag)
        {
            return variable.SetFlags(flag);
        }

        /// <summary>
        /// Includes flags and returns the new value
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="variable">Source enum</param>
        /// <param name="flags">Established flags</param>
        /// <returns>Result enum value</returns>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static T SetFlags<T>(this Enum variable, params T[] flags)
        {
            var result = Convert.ToUInt64(variable);
            foreach (var flag in flags)
                result |= Convert.ToUInt64(flag);
            return (T)Enum.Parse(variable.GetType(), result.ToString());
        }

        /// <summary>
        /// Check to see if enumeration has a specific flag
        /// </summary>
        /// <param name="variable">Enumeration to check</param>
        /// <param name="flag">Flag to check for</param>
        /// <returns>Result of check</returns>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        /*  This will never be called. Enum.HasFlag is a native method of Enum
        public static bool HasFlag<E>(this E variable, E flag)
            where E : struct, IComparable, IFormattable, IConvertible
        {
            return HasFlags(variable, flag);
        }
         */

        /// <summary>
        /// Check to see if enumeration has a specific flag set
        /// </summary>
        /// <param name="variable">Enumeration to check</param>
        /// <param name="flags">Flags to check for</param>
        /// <returns>Result of check</returns>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static bool HasFlags<E>(this E variable, params E[] flags)
            where E : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(E).GetTypeInfo().IsEnum)
                throw new ArgumentException("variable must be an Enum", "variable");

            foreach (var flag in flags)
            {
                if (!Enum.IsDefined(typeof(E), flag))
                    return false;

                var numFlag = Convert.ToUInt64(flag);
                if ((Convert.ToUInt64(variable) & numFlag) != numFlag)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// get <see cref="DescriptionAttribute"/> from enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            return value.GetType().GetField(value.ToString()).GetDescription();
        }
        /// <summary>
        /// get <see cref="DisplayAttribute"/> from enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplay(this Enum value)
        {
            return value.GetType().GetField(value.ToString()).GetDisplay();
        }
    }
}