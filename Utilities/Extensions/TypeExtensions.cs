using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HappyTools.Utilities.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNumeric(this Type type)
        {
            if (type.IsArray)
                return false;

            if (type == TypeHelper.ByteType ||
                type == TypeHelper.DecimalType ||
                type == TypeHelper.DoubleType ||
                type == TypeHelper.Int16Type ||
                type == TypeHelper.Int32Type ||
                type == TypeHelper.Int64Type ||
                type == TypeHelper.SByteType ||
                type == TypeHelper.SingleType ||
                type == TypeHelper.UInt16Type ||
                type == TypeHelper.UInt32Type ||
                type == TypeHelper.UInt64Type)
                return true;
            if (type.IsNullableNumeric())
                return true;


            return false;
        }
        public static bool IsNullable(this Type type)
        {
            if (type.IsArray)
                return false;

            var t = Nullable.GetUnderlyingType(type);
            return t != null;
        }
        public static bool IsNullableNumeric(this Type type)
        {
            if (type.IsArray)
                return false;

            var t = Nullable.GetUnderlyingType(type);
            return t != null && t.IsNumeric();
        }
        public static object GetDefaultValue(this Type type)
        {
            // Validate parameters.
            if (type == null) throw new ArgumentNullException("type");

            // We want an Func<object> which returns the default.
            // Create that expression here.
            var e = Expression.Lambda<Func<object>>(
                // Have to convert to object.
                Expression.Convert(
                    // The default value, always get what the *code* tells us.
                    Expression.Default(type), typeof(object)
                )
            );

            // Compile and return the value.
            return e.Compile()();
        }
        public static T ToType<T>(this object value)
        {
            var targetType = typeof(T);
            if (value == null)
            {
                try
                {
                    return (T)Convert.ChangeType(value, targetType);
                }
                catch
                {
                    throw new ArgumentNullException(nameof(value));
                }
            }

            var converter = TypeDescriptor.GetConverter(targetType);
            var valueType = value.GetType();

            if (targetType.IsAssignableFrom(valueType))
                return (T)value;

            var targetTypeInfo = targetType.GetTypeInfo();
            if (targetTypeInfo.IsEnum && (value is string || valueType.GetTypeInfo().IsEnum))
            {
                // attempt to match enum by name.
                if (EnumExtensions.TryEnumIsDefined(targetType, value.ToString()))
                {
                    var parsedValue = Enum.Parse(targetType, value.ToString(), false);
                    return (T)parsedValue;
                }

                var message = $"The Enum value of '{value}' is not defined as a valid value for '{targetType.FullName}'.";
                throw new ArgumentException(message);
            }

            if (targetTypeInfo.IsEnum && valueType.IsNumeric())
                return (T)Enum.ToObject(targetType, value);

            if (converter.CanConvertFrom(valueType))
            {
                var convertedValue = converter.ConvertFrom(value);
                return (T)convertedValue;
            }

            if (!(value is IConvertible))
                throw new ArgumentException($"An incompatible value specified.  Target Type: {targetType.FullName} Value Type: {value.GetType().FullName}", nameof(value));
            try
            {
                var convertedValue = Convert.ChangeType(value, targetType);
                return (T)convertedValue;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"An incompatible value specified.  Target Type: {targetType.FullName} Value Type: {value.GetType().FullName}", nameof(value), e);
            }
        }
        /// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents an enumeration.</summary>
        /// <returns>
        /// <see langword="true" /> if the current <see cref="T:System.Type" /> represents an enumeration; otherwise, <see langword="false" />.</returns>
        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
        /// <summary>
        /// Returns a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivatePropertyValue<T>(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            return (T)pi.GetValue(obj, null);
        }

        /// <summary>
        /// Returns a private Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivateFieldValue<T>(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.GetTypeInfo().BaseType;
            }
            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            return (T)fi.GetValue(obj);
        }

        /// <summary>
        /// Sets a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is set</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">Value to set.</param>
        /// <returns>PropertyValue</returns>
        public static void SetPrivatePropertyValue<T>(this object obj, string propName, T val)
        {
            var t = obj.GetType();
            var prop = t.GetProperty(propName);

            prop.SetValue(obj, val, null);
        }

        /// <summary>
        /// Set a private Property Value on a given Object. Uses Reflection.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">the value to set</param>
        /// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
        public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.GetTypeInfo().BaseType;
            }
            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            fi.SetValue(obj, val);
        }
        /// <summary>
        /// 	Creates and returns an instance of the desired type
        /// </summary>
        /// <param name = "type">The type to be instanciated.</param>
        /// <param name = "constructorParameters">Optional constructor parameters</param>
        /// <returns>The instanciated object</returns>
        /// <example>
        /// 	<code>
        /// 		var type = Type.GetType(".NET full qualified class Type")
        /// 		var instance = type.CreateInstance();
        /// 	</code>
        /// </example>
        public static object CreateInstance(this Type type, params object[] constructorParameters)
        {
            return type.CreateInstance<object>(constructorParameters);
        }

        /// <summary>
        /// 	Creates and returns an instance of the desired type casted to the generic parameter type T
        /// </summary>
        /// <typeparam name = "T">The data type the instance is casted to.</typeparam>
        /// <param name = "type">The type to be instanciated.</param>
        /// <param name = "constructorParameters">Optional constructor parameters</param>
        /// <returns>The instanciated object</returns>
        /// <example>
        /// 	<code>
        /// 		var type = Type.GetType(".NET full qualified class Type")
        /// 		var instance = type.CreateInstance&lt;IDataType&gt;();
        /// 	</code>
        /// </example>
        public static T CreateInstance<T>(this Type type, params object[] constructorParameters)
        {
            var instance = Activator.CreateInstance(type, constructorParameters);
            return (T)instance;
        }

        ///<summary>
        ///	Check if this is a base type
        ///</summary>
        ///<param name = "type"></param>
        ///<param name = "checkingType"></param>
        ///<returns></returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static bool IsBaseType(this Type type, Type checkingType)
        {
            while (type != typeof(object))
            {
                if (type == null)
                    continue;

                if (type == checkingType)
                    return true;

                type = type.GetTypeInfo().BaseType;
            }
            return false;
        }

        ///<summary>
        ///	Check if this is a sub class generic type
        ///</summary>
        ///<param name = "generic"></param>
        ///<param name = "toCheck"></param>
        ///<returns></returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static bool IsSubclassOfRawGeneric(this Type generic, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                if (toCheck == null)
                    continue;

                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.GetTypeInfo().BaseType;
            }
            return false;
        }

        /// <summary>
        /// Closes the passed generic type with the provided type arguments and returns an instance of the newly constructed type.
        /// </summary>
        /// <typeparam name="T">The typed type to be returned.</typeparam>
        /// <param name="genericType">The open generic type.</param>
        /// <param name="typeArguments">The type arguments to close the generic type.</param>
        /// <returns>An instance of the constructed type casted to T.</returns>
        public static T CreateGenericTypeInstance<T>(this Type genericType, params Type[] typeArguments) where T : class
        {
            var constructedType = genericType.MakeGenericType(typeArguments);
            var instance = Activator.CreateInstance(constructedType);
            return instance as T;
        }
        /// <summary>
        /// Given a lambda expression that contains a single reference to a public property, retrieves the property's setter accessor.
        /// </summary>
        /// <typeparam name="TProperty">Data type of property</typeparam>
        /// <param name="propertyExpression">A lambda expression in the form of <code>() => PropertyName</code></param>
        /// <returns></returns>
        public static Action<object, TProperty> ExtractPropertySetter<TProperty>(this Expression<Func<TProperty>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");

            var setMethod = property.SetMethod;

            if (setMethod == null)
                throw new ArgumentException("The referenced property does not have a set method.", "propertyExpression");

            if (setMethod.IsStatic)
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");

            Action<object, TProperty> action = (obj, val) => setMethod.Invoke(obj, new object[] { val });
            return action;
        }

        /// <summary>
        /// Extracts the property name from the property expression.
        /// 
        /// Implementation borrowed from Jounce MVVM framework.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="propertyExpression">A lambda expression in the form of <code>() => PropertyName</code></param>
        /// <param name="failSilently">When 'true' causes this method to return null results instead of throwing 
        /// an exception whenever a problem occurs while probing the type information.</param>
        /// <returns>The property name</returns>
        public static string ExtractPropertyName<TObject, TProperty>(this Expression<Func<TObject, TProperty>> propertyExpression, bool failSilently = false)
        {
            if (propertyExpression == null)
            {
                if (failSilently)
                    return null;
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                if (failSilently)
                    return null;
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                if (failSilently)
                    return null;
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            var getMethod = property.GetMethod;

            if (getMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                if (failSilently)
                    return null;
                throw new ArgumentException("The referenced property does not have a get method.", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                if (failSilently)
                    return null;
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Extracts the property name from the property expression.
        /// 
        /// Implementation borrowed from Jounce MVVM framework.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="propertyExpression">A lambda expression in the form of <code>() => PropertyName</code></param>
        /// <param name="failSilently">When 'true' causes this method to return null results instead of throwing 
        /// an exception whenever a problem occurs while probing the type information.</param>
        /// <returns>The property name</returns>
        public static string ExtractPropertyName<T>(this Expression<Func<T>> propertyExpression, bool failSilently = false)
        {
            if (propertyExpression == null)
            {
                if (failSilently)
                    return null;
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                if (failSilently)
                    return null;
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                if (failSilently)
                    return null;
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            var getMethod = property.GetMethod;

            if (getMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                if (failSilently)
                    return null;
                throw new ArgumentException("The referenced property does not have a get method.", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                if (failSilently)
                    return null;
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Inspects a type to see if it defines a property with the specified name and type.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="obj">An instance of an object being inspected, or a variable of the corresponding class type (can be null).</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>Returns 'true' if the property is found.</returns>
        public static bool HasProperty<T>(this T obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;

            var type = Equals(obj, default(T)) ? typeof(T) : obj.GetType();

            // Verify that the property name matches a realinstance property on this object.
            return type.GetRuntimeProperty(propertyName) != null;
        }

    }
}