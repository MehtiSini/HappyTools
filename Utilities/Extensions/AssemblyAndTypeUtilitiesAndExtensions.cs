using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HappyTools.Utilities.Extensions;
using HappyTools.Utilities.Extensions.Utilities;

namespace HappyTools.Utilities.Extensions
{
    /// <summary>
    /// 	Extension methods for the reflection meta data typeInfo "Type"
    /// </summary>
    public static class AssemblyAndTypeUtilitiesAndExtensions
    {
        ///// <summary>
        ///// 	Creates and returns an instance of the desired typeInfo
        ///// </summary>
        ///// <param name = "typeInfo">The typeInfo to be instanciated.</param>
        ///// <param name = "constructorParameters">Optional constructor parameters</param>
        ///// <returns>The instanciated object</returns>
        ///// <example>
        ///// 	<code>
        ///// 		var typeInfo = Type.GetType(".NET full qualified class Type")
        ///// 		var instance = typeInfo.CreateInstance();
        ///// 	</code>
        ///// </example>
        //public static object CreateInstance(this Type typeInfo, params object[] constructorParameters)
        //{
        //    return CreateInstance<object>(typeInfo, constructorParameters);
        //}

        ///// <summary>
        ///// 	Creates and returns an instance of the desired typeInfo casted to the generic parameter typeInfo T
        ///// </summary>
        ///// <typeparam name = "T">The data typeInfo the instance is casted to.</typeparam>
        ///// <param name = "typeInfo">The typeInfo to be instanciated.</param>
        ///// <param name = "constructorParameters">Optional constructor parameters</param>
        ///// <returns>The instanciated object</returns>
        ///// <example>
        ///// 	<code>
        ///// 		var typeInfo = Type.GetType(".NET full qualified class Type")
        ///// 		var instance = typeInfo.CreateInstance&lt;IDataType&gt;();
        ///// 	</code>
        ///// </example>
        //public static T CreateInstance<T>(this Type typeInfo, params object[] constructorParameters)
        //{
        //    var instance = Activator.CreateInstance(typeInfo, constructorParameters);
        //    return (T)instance;
        //}

        ///<summary>
        ///	Check if this is a base typeInfo
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
        ///	Check if this is accessible 
        ///</summary>
        ///<param name = "type"></param>
        ///<returns></returns>
        public static bool IsAccessible(this Type type)
        {
            return type.GetTypeInfo().IsVisible && type.GetTypeInfo().IsPublic;
        }

        ///<summary>
        ///	Check if this is a sub class generic typeInfo
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
        /// Closes the passed generic typeInfo with the provided typeInfo arguments and returns an instance of the newly constructed typeInfo.
        /// </summary>
        /// <typeparam name="T">The typed typeInfo to be returned.</typeparam>
        /// <param name="genericType">The open generic typeInfo.</param>
        /// <param name="typeArguments">The typeInfo arguments to close the generic typeInfo.</param>
        /// <returns>An instance of the constructed typeInfo casted to T.</returns>
        public static T CreateGenericTypeInstance<T>(this Type genericType, params Type[] typeArguments) where T : class
        {
            var constructedType = genericType.MakeGenericType(typeArguments);
            var instance = Activator.CreateInstance(constructedType);
            return instance as T;
        }


        /// <summary>
        /// Get all classes contains attribute of typeInfo <typeparamref name="T"/> in given assembly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<TypeInfo, T> GetClassesContainsAttribute<T>(this Assembly assembly) where T : Attribute
        {
            var res =
                (from type in assembly.DefinedTypes
                 where type.IsClass && type.GetCustomAttributes(typeof(T), true).Any()
                 select new
                 {
                     attr = type.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T,
                     classType = type
                 })
                .ToDictionary(k => k.classType, v => v.attr);
            return res;
        }

        /// <summary>
        /// Get <typeparamref name="T"/> value on a class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetClassAttribute<T>(this Type type) where T : Attribute
        {
            var attribute = type.GetTypeInfo().GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
            return attribute;
        }

        /// <summary>
        /// Get all members contains attribute of typeInfo <typeparamref name="T"/> in given assembly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetMembersContainsAttribute<T>(this Assembly assembly) where T : Attribute
        {
            var res = new List<T>();
            var members =
            from type in assembly.DefinedTypes
            where type.IsClass && type.GetCustomAttributes(typeof(T), true).Any()
            select new
            {
                methods = type.GetMethodsContainsAttribute<T>(),
                attr = type.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T,
                classType = type
            };
            members.ToList()
                .ForEach(p =>
                {
                    res.Add(p.attr);
                    res.AddRange(p.methods.Values);
                });
            return res;
        }
        /// <summary>
        /// Find all properties contains <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<PropertyInfo, T> GetPropertiesContainsAttribute<T>(this Type type) where T : Attribute
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T != null)
                .Select(s => new { attr = s.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T, method = s })
                .ToDictionary(k => k.method, v => v.attr);
            return properties;
        }
        /// <summary>
        /// Find all methods contains <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<MethodInfo, T> GetMethodsContainsAttribute<T>(this Type type) where T : Attribute
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T != null)
                .Select(s => new { attr = s.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T, method = s })
                .ToDictionary(k => k.method, v => v.attr);
            return methods;
        }
        /// <summary>
        /// Find all methods contains <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static Dictionary<MethodInfo, T> GetMethodsContainsAttribute<T>(this TypeInfo typeInfo) where T : Attribute
        {
            var methods = typeInfo.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T != null)
                .Select(s => new { attr = s.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T, method = s })
                .ToDictionary(k => k.method, v => v.attr);
            return methods;
        }
        /// <summary>
        /// Find all methods contains <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T FindAttribute<T>(this MemberInfo member) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
            return attribute;
        }
        /// <summary>
        /// find the derived types in given assembly
        /// </summary>
        /// <typeparam name="T">typeInfo that it should find its derived types</typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindDerivedTypes<T>(this Assembly assembly)
        {
            var type = typeof(T);
            return assembly.FindDerivedTypes(type);

        }

        /// <summary>
        /// find the derived types in given assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="baseType">typeInfo that it should find its derived types</param>
        /// <returns></returns>
        public static IEnumerable<Type> FindDerivedTypes(this Assembly assembly, Type baseType)
        {
            var baseTypeInfo = baseType.GetTypeInfo();
            bool isClass = baseTypeInfo.IsClass, isInterface = baseTypeInfo.IsInterface;

            return
                from type in assembly.DefinedTypes
                where isClass
                    ? type.IsSubclassOf(baseType)
                    : isInterface && type.ImplementedInterfaces.Contains(baseTypeInfo.AsType())
                select type.AsType();
        }

        /// <summary>
        /// Get all assembly file names in given directory path
        /// </summary>
        /// <param name="directoryPath">path of directory to search in it</param>
        /// <param name="includingSubDirectories">whether it must search in sub directories or not</param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllAssemblyFilesInDirectory(string directoryPath,
            bool includingSubDirectories)
        {
            var files = Directory.EnumerateFiles(directoryPath, "*",
                    includingSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(s => s.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                            s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));
            return files;
        }
        /// <summary>
        /// get public properties of current object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPublicProperties(this object obj)
        {
            var res = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToList();

            return res;
        }
        /// <summary>
        /// get public properties value pair of current object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<PropertyInfo, object> GetPublicPropertiesValuePair(this object obj)
        {
            var res = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new { key = p, value = p.GetValue(obj) })
                .ToDictionary(k => k.key, p => p.value);

            return res;
        }
        /// <summary>
        /// Get the value of properties that contains the attribute of typeInfo <typeparam name="TAttribute"></typeparam>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetPropertyValuesContainAttribute<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            var values = new List<object>();
            values = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<TAttribute>() != null)
                .Select(p => p.GetValue(obj))
                .ToList();

            return values;
        }
        /// <summary>
        /// Get properties that contains the attribute of typeInfo <typeparam name="TAttribute"></typeparam>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesContainAttribute<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            var props = new List<PropertyInfo>();
            props = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<TAttribute>() != null)
                .ToList();

            return props;
        }

        /// <summary>
        /// Find properties recursively that contains the attribute of typeInfo <typeparam name="TAttribute"></typeparam>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<KeyValuePair<PropertyInfo, object>> GetPropertiesContainAttributeRecursive<TAttribute>(this object obj) where TAttribute : Attribute
        {
            var result = new List<KeyValuePair<PropertyInfo, object>>();
            if (obj is null)
                return result;

            if (obj is IEnumerable)
            {
                var collection = (IList)obj;
                foreach (var val in collection)
                    result.AddRange(GetPropertiesContainAttributeRecursive<TAttribute>(val).ToArray());
                return result;
            }

            var type = obj.GetType();

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).
                         Where(x => x.GetCustomAttribute<TAttribute>() != null))
            {
                if (property.PropertyType == type)
                    continue;
                if (!property.CanRead)
                    continue;
                var val = property.GetValue(obj);
                result.Add(new KeyValuePair<PropertyInfo, object>(property, val));
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    result.AddRange(GetPropertiesContainAttributeRecursive<TAttribute>(val).ToArray());
                }
            }

            return result;
        }
        /// <summary>
        /// Get the name of properties that contains the attribute of typeInfo <typeparam name="TAttribute"></typeparam>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPropertyNamesContainAttribute<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            var props = new List<string>();
            props = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<TAttribute>() != null)
                .Select(p => p.Name)
                .ToList();

            return props;
        }
        /// <summary>
        /// Get the value of first property that contains the attribute of typeInfo <typeparam name="TAttribute"></typeparam>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetPropertyValueContainsAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        {
            object val = null;
            var property = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.GetCustomAttribute<TAttribute>() != null);
            if (property != null)
            {
                val = property.GetValue(obj);
            }
            return val;
        }
        /// <summary>
        /// check whether the property has the attribute of typeInfo <typeparam name="TAttribute"></typeparam> or not
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool HasCustomAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
        {
            return property.GetCustomAttribute<TAttribute>() != null;

        }
        /// <summary>
        /// Get a dictionary of property name and value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetAllPropertyValuePair(this object obj)
        {
            var keyValues = new Dictionary<string, object>();
            if (obj == null)
            {
                return null;
            }
            var properties = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .ToList();

            var type = obj.GetType();
            foreach (var info in properties)
            {
                var val = info.GetValue(obj, null);
                keyValues.Add(info.Name, val);
            }
            return keyValues;
        }
        ///// <summary>
        ///// Get the value of property
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //public static object GetPropertyValue(this object obj, string propertyName)
        //{
        //    if (obj == null)
        //    {
        //        return null;
        //    }

        //    Type typeInfo = obj.GetType();
        //    PropertyInfo info = typeInfo.GetProperty(propertyName);
        //    if (info == null)
        //    {
        //        return null;
        //    }

        //    obj = info.GetValue(obj, null);
        //    return obj;
        //}
        ///// <summary>
        ///// Get the <see cref="PropertyInfo"/> of given <param name="propertyName"></param>
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //public static PropertyInfo GetProperty(this object obj, string propertyName)
        //{
        //    if (obj == null||string.IsNullOrEmpty(propertyName))
        //    {
        //        return null;
        //    }

        //    Type typeInfo = obj.GetType();
        //    var info1 = obj
        //        .GetType()
        //        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
        //                       BindingFlags.Static);

        //    var info = obj
        //        .GetType()
        //        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic| BindingFlags.Static).FirstOrDefault(p => p.Name== propertyName);
        //    if (info == null)
        //    {
        //        return null;
        //    }
        //    return info;
        //}

        ///// <summary>
        ///// set the value of property
        ///// </summary>
        ///// <param name="inputObject"></param>
        ///// <param name="propertyName"></param>
        ///// <param name="propertyVal"></param>
        //public static void SetPropertyValue(this object inputObject, string propertyName, object propertyVal)
        //{
        //    //find out the typeInfo
        //    Type typeInfo = inputObject.GetType();

        //    //get the property information based on the typeInfo
        //    System.Reflection.PropertyInfo propertyInfo = typeInfo.GetProperty(propertyName);

        //    //find the property typeInfo
        //    Type propertyType = propertyInfo.PropertyType;

        //    //Convert.ChangeType does not handle conversion to nullable types
        //    //if the property typeInfo is nullable, we need to get the underlying typeInfo of the property
        //    var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

        //    ////Returns an System.Object with the specified System.Type and whose value is
        //    ////equivalent to the specified object.
        //    //propertyVal = Convert.ChangeType(propertyVal, targetType);

        //    //Set the value of the property
        //    propertyInfo.SetValue(inputObject, propertyVal, null);

        //}
        ///// <summary>
        ///// set the value of given <param name="propertyInfo"></param>
        ///// </summary>
        ///// <param name="inputObject"></param>
        ///// <param name="propertyInfo"></param>
        ///// <param name="propertyVal"></param>
        //public static void SetPropertyValue(this object inputObject, PropertyInfo propertyInfo, object propertyVal)
        //{
        //    //find out the typeInfo
        //    Type typeInfo = inputObject.GetType();


        //    //find the property typeInfo
        //    Type propertyType = propertyInfo.PropertyType;

        //    //Convert.ChangeType does not handle conversion to nullable types
        //    //if the property typeInfo is nullable, we need to get the underlying typeInfo of the property
        //    var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

        //    ////Returns an System.Object with the specified System.Type and whose value is
        //    ////equivalent to the specified object.
        //    //propertyVal = Convert.ChangeType(propertyVal, targetType);

        //    //Set the value of the property
        //    propertyInfo.SetValue(inputObject, propertyVal, null);

        //}
        private static bool IsNullableType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
        /// <summary>
        /// get <see cref="DescriptionAttribute"/> from a field
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo member)
        {
            var attributes = (DescriptionAttribute[])member.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        /// <summary>
        /// get <see cref="DescriptionAttribute"/> from a field
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDescription(this PropertyInfo property)
        {
            var attributes = (DescriptionAttribute[])property.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        /// <summary>
        /// get <see cref="DisplayAttribute"/> from a field
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetDisplay(this MemberInfo member)
        {
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false).ToList();
            if (!attributes.Any())
                return string.Empty;
            var attribute = (DisplayAttribute)attributes[0];
            if (attribute.ResourceType != null)
            {
                if (!string.IsNullOrEmpty(attribute.Name))
                {
                    var resourceValue =
                        ResourceUtility.GetResource(attribute.ResourceType,
                            attribute.Name);
                    return resourceValue;
                }

            }
            else
            {
                return attribute.Name;
            }
            return string.Empty;
        }
        /// <summary>
        /// get <see cref="DisplayAttribute"/> from a field
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDisplay(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof(DisplayAttribute), false).ToList();
            if (!attributes.Any())
                return string.Empty;
            var attribute = (DisplayAttribute)attributes[0];
            if (attribute.ResourceType != null)
            {
                if (!string.IsNullOrEmpty(attribute.Name))
                {
                    var resourceValue =
                        ResourceUtility.GetResource(attribute.ResourceType,
                            attribute.Name);
                    return resourceValue;
                }

            }
            else
            {
                return attribute.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// get <see cref="DescriptionAttribute"/> from a field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetDescription(this FieldInfo field)
        {
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        /// <summary>
        /// get <see cref="DescriptionAttribute"/> from a field
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static string GetDescription(this TypeInfo typeInfo)
        {
            var attributes = (DescriptionAttribute[])typeInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        /// <summary>
        /// get <see cref="DescriptionAttribute"/> from a field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                var type = e.GetType();
                var values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description;
        }
        /// <summary>
        /// get <see cref="DescriptionAttribute"/> from a field
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(this Type type)
        {
            var attributes = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        /// <summary>
        /// get <see cref="DisplayAttribute"/> from a field
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static string GetDisplay(this TypeInfo typeInfo)
        {
            var attributes = typeInfo.GetCustomAttributes(typeof(DisplayAttribute), false).ToList();
            if (!attributes.Any())
                return string.Empty;
            var attribute = (DisplayAttribute)attributes[0];
            if (attribute.ResourceType != null)
            {
                if (!string.IsNullOrEmpty(attribute.Name))
                {
                    var resourceValue =
                        ResourceUtility.GetResource(attribute.ResourceType,
                            attribute.Name);
                    return resourceValue;
                }

            }
            else
            {
                return attribute.Name;
            }
            return string.Empty;
        }
        /// <summary>
        /// get <see cref="DisplayAttribute"/> from a field
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDisplay(this Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(DisplayAttribute), false).ToList();
            if (!attributes.Any())
                return string.Empty;
            var attribute = (DisplayAttribute)attributes[0];
            if (attribute.ResourceType != null)
            {
                if (!string.IsNullOrEmpty(attribute.Name))
                {
                    var resourceValue =
                        ResourceUtility.GetResource(attribute.ResourceType,
                            attribute.Name);
                    return resourceValue;
                }

            }
            else
            {
                return attribute.Name;
            }
            return string.Empty;
        }
        /// <summary>
        /// get <see cref="DisplayAttribute"/> from a field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetDisplay(this FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(typeof(DisplayAttribute), false).ToList();
            if (!attributes.Any())
                return string.Empty;
            var attribute = (DisplayAttribute)attributes[0];
            if (attribute.ResourceType != null)
            {
                if (!string.IsNullOrEmpty(attribute.Name))
                {
                    var resourceValue =
                        ResourceUtility.GetResource(attribute.ResourceType,
                            attribute.Name);
                    return resourceValue;
                }

            }
            else
            {
                return attribute.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// get full typeInfo name of given <see cref="object"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFullTypeName(this object obj)
        {
            return obj.GetType().GetFullTypeName(true);
        }

        /// <summary>
        /// get typeInfo name of given <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="getFullName"></param>
        /// <returns></returns>
        public static string GetFullTypeName(this Type type, bool getFullName)
        {
            if (type.AssemblyQualifiedName != null) return type.AssemblyQualifiedName.ToString();
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// get full typeInfo name of given <see cref="object"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTypeName(this object obj)
        {
            return obj.GetType().GetTypeName(true);
        }

        /// <summary>
        /// get typeInfo name of given <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="getFullName"></param>
        /// <returns></returns>
        public static string GetTypeName(this Type type, bool getFullName)
        {
            if (type.FullName != null) return type.FullName;
            else
            {
                return type.Name;
            }
        }

        /// <summary>
        /// return true if typeInfo is implemented given <param name="interfaceType"></param> typeInfo
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool IsIEnumerable(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEnumerable)) || type.GetInterfaces().Contains(typeof(IEnumerable<>));
        }
        /// <summary>
        /// get typeInfo of T in an IEnumerable<T> typeInfo  
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetElementType(this Type type)
        {
            // Type is Array
            // short-circuit if you expect lots of arrays 
            if (type.IsArray)
                return type.GetElementType();

            // typeInfo is IEnumerable<T>;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // typeInfo implements/extends IEnumerable<T>;
            var enumType = type.GetInterfaces()
                .Where(t => t.IsGenericType &&
                            t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
            return enumType ?? type;
        }
        /// <summary>
        /// return true if typeInfo is implemented given <param name="interfaceType"></param> typeInfo
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool IsOfType(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }
        /// <summary>
        /// gets the content of embedded resource name as string
        /// </summary>
        /// <param name="assembly">assembly that contains embedded resource</param>
        /// <param name="resourceName">embedded resource name (could be like "Folder/file.txt")</param>
        /// <returns></returns>
        public static string GetEmbeddedResourceContentAsString(this Assembly assembly, string resourceName)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;

                return resourceStream.ReadToEnd();
            }
        }
        /// <summary>
        /// gets the content of embedded resource name 
        /// </summary>
        /// <param name="assembly">assembly that contains embedded resource</param>
        /// <param name="resourceName">embedded resource name (could be like "Folder/file.txt")</param>
        /// <returns></returns>
        public static byte[] GetEmbeddedResourceContent(this Assembly assembly, string resourceName)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;
                return resourceStream.ReadAllBytes();
            }
        }
        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_")
                       .Replace("\\", ".")
                       .Replace("/", ".");
        }

        /// <summary>
        /// Looks in given type for the input type.
        /// </summary>
        /// <param name="assembly">given assembly</param>
        /// <param name="typeName">type name.</param>
        /// <returns>
        /// The <see cref="Type"/> found; null if not found.
        /// </returns>
        public static Type FindType(this Assembly assembly, string typeName)
        {

            return
                assembly.DefinedTypes
                    .FirstOrDefault(t => t.Name.Equals(typeName));
        }
    } // class
}
