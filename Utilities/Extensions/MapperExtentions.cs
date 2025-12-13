using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HappyTools.Utilities.Extensions;

namespace HappyTools.Utilities.Extensions
{
    public static class MapperExtentions
    {
        public static List<string> AuditingProperties {
            get {
                return new List<string>()
                {
                    "ProjectId",
                    "Id",
                    "ConcurrencyStamp",
                    "CreatorId",
                    "CreationTime",
                    "DeleterId",
                    "DeletionTime",
                    "IsDeleted",
                    "LastModificationTime",
                    "LastModifierId"
                };
            }
        }
        /// <summary>
        /// Copy all property values to <see cref="objTarget"/>
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget">Target object</param>
        /// <param name="ignoreCase">specify value whether it should ignore case sensitivity while searching for properties</param>
        /// <param name="excludedProperties">list of properties that should be excluded from mapping</param>
        public static void SafeCopy(this object objSource, object objTarget, params string[] excludedProperties)
        {
            //Get the type of target object and create a new instance of that type
            var typeDestination = objTarget.GetType();
            //Get all the properties of target object type
            var targetProperties = typeDestination.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //Assign all source property to taget object 's properties
            foreach (var targetProperty in targetProperties)
            {
                var targetPropertyName = targetProperty.Name;
                if (excludedProperties.Contains(targetPropertyName))
                    continue;
                if (AuditingProperties.Contains(targetPropertyName))
                    continue;
                //if it is IEnumerable of a reference type except string it may be navigation type
                if (targetProperty.PropertyType.IsIEnumerable())
                {

                    var elType = targetProperty.PropertyType.GetElementType();
                    if (elType != null && elType.IsClass && elType != typeof(string))
                        continue;

                }
                PropertyInfo sourceProperty = null;

                try
                {

                    sourceProperty = objSource.GetType().GetProperty(targetPropertyName);

                    if (sourceProperty == null)
                    {
                        //now capitalize first letter and check again
                        targetPropertyName = targetProperty.Name.FirstLetterToUpperCase();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        //now capitalize first letter and check again
                        targetPropertyName = targetProperty.Name.FirstLetterToLowerCase();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        //now check for upper
                        targetPropertyName = targetProperty.Name.ToUpper();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        //now check for lower
                        targetPropertyName = targetProperty.Name.ToLower();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                //Check whether property can be written to
                if (targetProperty.CanWrite)
                {
                    //checking for null and empty value
                    var targetValue = targetProperty.GetValue(objTarget);
                    var sourceValue = sourceProperty.GetValue(objSource, null);
                    //if(sourceValue==null||(sourceValue is ICollection && ((ICollection)sourceValue).Count==0))
                    //continue;
                    if (sourceProperty.PropertyType != targetProperty.PropertyType)
                    {

                        try
                        {
                            //check weather source type can be converted to target type
                            var cValue = Convert.ChangeType(sourceProperty.GetValue(objSource, null), targetProperty.PropertyType);
                            targetProperty.SetValue(objTarget, cValue, null);
                        }
                        catch
                        {
                            continue;
                        }

                    }
                    else
                    {
                        //check whether property type is value type, enum or string type
                        if (sourceProperty.PropertyType.GetTypeInfo().IsValueType || sourceProperty.PropertyType.GetTypeInfo().IsEnum || sourceProperty.PropertyType.Equals(typeof(string)))
                        {
                            targetProperty.SetValue(objTarget, sourceProperty.GetValue(objSource, null), null);
                        }
                        //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                        else
                        {
                            var objPropertyValue = sourceProperty.GetValue(objSource, null);
                            if (objPropertyValue == null)
                            {
                                targetProperty.SetValue(objTarget, null, null);
                            }
                            else
                            {
                                targetProperty.SetValue(objTarget, objPropertyValue, null);
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// Copy all property values to <see cref="objTarget"/>
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget">Target object</param>
        public static void CopyPropertiesTo(this object objSource, object objTarget)
        {
            try
            {
                objSource.CopyPropertiesTo(objTarget, false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Copy all property values to <see cref="objTarget"/>
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget">Target object</param>
        /// <param name="excludedProperties">list of properties that should be excluded from mapping</param>
        public static void CopyPropertiesTo(this object objSource, object objTarget, params string[] excludedProperties)
        {
            objSource.CopyPropertiesTo(objTarget, false, excludedProperties);
        }
        /// <summary>
        /// Copy all property values to <see cref="objTarget"/>
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget">Target object</param>
        /// <param name="excludedPropertyselector">list of properties that should be excluded from mapping</param>

        public static void CopyPropertiesTo<TSource>(this object objSource, object objTarget, params Expression<Func<TSource, TSource>>[] excludedPropertyselector) where TSource : class, new()
        {
            objSource.CopyPropertiesTo(objTarget, false, excludedPropertyselector);
        }
        /// <summary>
        /// Copy all property values to <see cref="objTarget"/>
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget">Target object</param>
        /// <param name="ignoreCase">specify value whether it should ignore case sensitivity while searching for properties</param>
        /// <param name="excludedPropertyselector">list of properties that should be excluded from mapping</param>

        public static void CopyPropertiesTo<TSource>(this object objSource, object objTarget, bool ignoreCase, params Expression<Func<TSource, TSource>>[] excludedPropertyselector) where TSource : class, new()
        {
            var excludedProperties = new List<string>();
            foreach (var propertyExpression in excludedPropertyselector)
            {
                var expression = (MemberExpression)propertyExpression.Body;
                var name = expression.Member.Name;
                excludedProperties.Add(name);
            }
            objSource.CopyPropertiesTo(objTarget, ignoreCase, excludedProperties.ToArray());
        }
        /// <summary>
        /// Copy all property values to <see cref="objTarget"/>
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget">Target object</param>
        /// <param name="ignoreCase">specify value whether it should ignore case sensitivity while searching for properties</param>
        /// <param name="excludedProperties">list of properties that should be excluded from mapping</param>
        public static void CopyPropertiesTo(this object objSource, object objTarget, bool ignoreCase, params string[] excludedProperties)
        {
            //Get the type of target object and create a new instance of that type
            var typeDestination = objTarget.GetType();
            //Get all the properties of target object type
            var targetProperties = typeDestination.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //Assign all source property to taget object 's properties
            foreach (var targetProperty in targetProperties)
            {
                if (excludedProperties.Contains(targetProperty.Name))
                    continue;
                var targetPropertyName = targetProperty.Name;
                if (excludedProperties.Contains(targetPropertyName))
                    continue;
                //if it is IEnumerable of a reference type except string it may be navigation type
                if (targetProperty.PropertyType.IsIEnumerable())
                {

                    var elType = targetProperty.PropertyType.GetElementType();
                    if (elType != null && elType.IsClass && elType != typeof(string))
                        continue;

                }
                PropertyInfo sourceProperty = null;

                try
                {

                    sourceProperty = objSource.GetType().GetProperty(targetPropertyName);

                    if (sourceProperty == null)
                    {
                        //now capitalize first letter and check again
                        targetPropertyName = targetProperty.Name.FirstLetterToUpperCase();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        //now capitalize first letter and check again
                        targetPropertyName = targetProperty.Name.FirstLetterToLowerCase();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        //now check for upper
                        targetPropertyName = targetProperty.Name.ToUpper();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        //now check for lower
                        targetPropertyName = targetProperty.Name.ToLower();
                        sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
                    }

                    if (sourceProperty == null)
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                //Check whether property can be written to
                if (targetProperty.CanWrite)
                {

                    if (sourceProperty.PropertyType != targetProperty.PropertyType)
                    {

                        try
                        {
                            //check weather source type can be converted to target type
                            var cValue = Convert.ChangeType(sourceProperty.GetValue(objSource, null), targetProperty.PropertyType);
                            targetProperty.SetValue(objTarget, cValue, null);
                        }
                        catch
                        {
                            continue;
                        }

                    }
                    else
                    {
                        //check whether property type is value type, enum or string type
                        if (sourceProperty.PropertyType.GetTypeInfo().IsValueType || sourceProperty.PropertyType.GetTypeInfo().IsEnum || sourceProperty.PropertyType.Equals(typeof(string)))
                        {
                            targetProperty.SetValue(objTarget, sourceProperty.GetValue(objSource, null), null);
                        }
                        //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                        else
                        {
                            var objPropertyValue = sourceProperty.GetValue(objSource, null);
                            if (objPropertyValue == null)
                            {
                                targetProperty.SetValue(objTarget, null, null);
                            }
                            else
                            {
                                targetProperty.SetValue(objTarget, objPropertyValue, null);
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// Copy all property values from expando object to <see cref="objTarget"/>
        /// </summary>
        /// <param name="expandoObjectSource"></param>
        /// <param name="objTarget">Target object</param>
        /// <param name="ignoreCase">specify value whether it should ignore case sensitivity while searching for properties</param>
        /// <param name="excludedProperties">list of properties that should be excluded from mapping</param>
        public static void CopyPropertiesFromExpandoTo(this ExpandoObject expandoObjectSource, object objTarget, bool ignoreCase, params string[] excludedProperties)
        {
            if (expandoObjectSource == null) throw new ArgumentNullException(nameof(expandoObjectSource));
            //Get the type of target object and create a new instance of that type
            var typeDestination = objTarget.GetType();

            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expandoObjectSource as IDictionary<string, object>;

            //Get all the properties of target object type
            var targetProperties = typeDestination.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //Assign all source property to taget object 's properties
            foreach (var targetProperty in targetProperties)
            {
                if (excludedProperties.Contains(targetProperty.Name))
                    continue;
                object sourceProperty = null;

                try
                {
                    var targetPropertyName = targetProperty.Name;
                    sourceProperty = expandoObjectSource.GetExpandoPropertyValue(targetPropertyName);
                    if (ignoreCase)
                    {
                        if (sourceProperty == null)
                        {
                            //now capitalize first letter and check again
                            targetPropertyName = targetProperty.Name.FirstLetterToUpperCase();
                            sourceProperty = expandoObjectSource.GetExpandoPropertyValue(targetPropertyName);
                        }
                        if (sourceProperty == null)
                        {
                            //now capitalize first letter and check again
                            targetPropertyName = targetProperty.Name.FirstLetterToLowerCase();
                            sourceProperty = expandoObjectSource.GetExpandoPropertyValue(targetPropertyName);
                        }
                        if (sourceProperty == null)
                        {
                            //now check for upper
                            targetPropertyName = targetProperty.Name.ToUpper();
                            sourceProperty = expandoObjectSource.GetExpandoPropertyValue(targetPropertyName);
                        }
                        if (sourceProperty == null)
                        {
                            //now check for lower
                            targetPropertyName = targetProperty.Name.ToLower();
                            sourceProperty = expandoObjectSource.GetExpandoPropertyValue(targetPropertyName);
                        }
                    }
                    if (sourceProperty == null)
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                //Check whether property can be written to
                if (targetProperty.CanWrite)
                {

                    if (sourceProperty.GetType() != targetProperty.PropertyType)
                    {

                        try
                        {
                            //check for nullable
                            var undeltingNullableType = Nullable.GetUnderlyingType(targetProperty.PropertyType);
                            if (undeltingNullableType != null)
                            {
                                if (sourceProperty == null)
                                    targetProperty.SetValue(objTarget, null, null);
                                else
                                {
                                    var cValue = Convert.ChangeType(sourceProperty, undeltingNullableType);
                                    targetProperty.SetValue(objTarget, cValue, null);
                                }
                            }
                            else
                            {
                                //check weather source type can be converted to target type

                                var cValue = Convert.ChangeType(sourceProperty, targetProperty.PropertyType);
                                targetProperty.SetValue(objTarget, cValue, null);
                            }

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                    }
                    else
                    {
                        //check whether property type is value type, enum or string type
                        if (sourceProperty.GetType().GetTypeInfo().IsValueType || sourceProperty.GetType().GetTypeInfo().IsEnum || sourceProperty.GetType().Equals(typeof(string)))
                        {
                            targetProperty.SetValue(objTarget, sourceProperty, null);
                        }
                        //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                        else
                        {
                            var objPropertyValue = sourceProperty;
                            if (objPropertyValue == null)
                            {
                                targetProperty.SetValue(objTarget, null, null);
                            }
                            else
                            {
                                targetProperty.SetValue(objTarget, objPropertyValue, null);
                            }
                        }
                    }

                }
            }
        }
        //public static void CopyPropertiesTo(this object objSource, object objTarget, bool ignoreCase, params string[] excludedProperties)
        //{
        //    //Get the type of target object and create a new instance of that type
        //    Type typeDestination = objTarget.GetType();
        //    //Get all the properties of target object type
        //    PropertyInfo[] targetProperties = typeDestination.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    //Assign all source property to taget object 's properties
        //    foreach (PropertyInfo targetProperty in targetProperties)
        //    {
        //        if (excludedProperties.Contains(targetProperty.Name))
        //            continue;
        //        PropertyInfo sourceProperty = null;

        //        try
        //        {
        //            var targetPropertyName = targetProperty.Name;
        //            sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
        //            if (ignoreCase)
        //            {
        //                if (sourceProperty == null)
        //                {
        //                    //now capitalize first letter and check again
        //                    targetPropertyName = targetProperty.Name.FirstLetterToUpperCase();
        //                    sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
        //                }
        //                if (sourceProperty == null)
        //                {
        //                    //now capitalize first letter and check again
        //                    targetPropertyName = targetProperty.Name.FirstLetterToLowerCase();
        //                    sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
        //                }
        //                if (sourceProperty == null)
        //                {
        //                    //now check for upper
        //                    targetPropertyName = targetProperty.Name.ToUpper();
        //                    sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
        //                }
        //                if (sourceProperty == null)
        //                {
        //                    //now check for lower
        //                    targetPropertyName = targetProperty.Name.ToLower();
        //                    sourceProperty = objSource.GetType().GetProperty(targetPropertyName);
        //                }
        //            }
        //            if (sourceProperty == null)
        //            {
        //                continue;
        //            }
        //        }
        //        catch
        //        {
        //            continue;
        //        }

        //        CopyPropertiesTo(objSource, sourceProperty, objTarget, targetProperty, ignoreCase,
        //            excludedProperties);
        //    }
        //}

        /// <summary>
        /// Copy all property values to <see cref="objTarget"/>
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget">Target object</param>
        /// <param name="ignoreCase">specify value whether it should ignore case sensitivity while searching for properties</param>
        /// <param name="excludedProperties">list of properties that should be excluded from mapping</param>

        internal static void CopyPropertiesTo(object sourceObj, PropertyInfo sourceProperty,
            object targetObj, PropertyInfo targetProperty, bool ignoreCase, params string[] excludedProperties)
        {
            //Check whether property can be written to
            if (targetProperty.CanWrite)
            {

                if (sourceProperty.PropertyType != targetProperty.PropertyType)
                {
                    //check whether property type is value type, enum or string type
                    if (sourceProperty.PropertyType.GetTypeInfo().IsValueType ||
                        sourceProperty.PropertyType.GetTypeInfo().IsEnum ||
                        sourceProperty.PropertyType.Equals(typeof(string)))
                    {
                        //check weather source type can be converted to target type
                        var cValue = Convert.ChangeType(sourceProperty.GetValue(sourceProperty, null),
                            targetProperty.PropertyType);
                        targetProperty.SetValue(targetObj, cValue, null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    else
                    {
                        var objPropertyValue = sourceProperty.GetValue(sourceObj, null);
                        if (objPropertyValue == null)
                        {
                            targetProperty.SetValue(targetObj, null, null);
                        }
                        else
                        {

                            CopyPropertiesTo(objPropertyValue, sourceProperty, targetObj, targetProperty, ignoreCase,
                                excludedProperties);
                        }

                    }

                }
                else
                {
                    //check whether property type is value type, enum or string type
                    if (sourceProperty.PropertyType.GetTypeInfo().IsValueType ||
                        sourceProperty.PropertyType.GetTypeInfo().IsEnum ||
                        sourceProperty.PropertyType.Equals(typeof(string)))
                    {
                        targetProperty.SetValue(targetObj, sourceProperty.GetValue(sourceObj, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    else
                    {
                        var objPropertyValue = sourceProperty.GetValue(sourceObj, null);
                        if (objPropertyValue == null)
                        {
                            targetProperty.SetValue(targetObj, null, null);
                        }
                        else
                        {
                            CopyPropertiesTo(objPropertyValue, sourceProperty, targetObj, targetProperty, ignoreCase,
                                excludedProperties);
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Create a deep copy of an object
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static object CloneObject(this object objSource)
        {
            //Get the type of source object and create a new instance of that type
            var typeSource = objSource.GetType();
            var objTarget = Activator.CreateInstance(typeSource);

            //Get all the properties of source object type
            var propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //Assign all source property to taget object 's properties
            foreach (var property in propertyInfo)
            {
                //Check whether property can be written to
                if (property.CanWrite)
                {
                    //check whether property type is value type, enum or string type
                    if (property.PropertyType.GetTypeInfo().IsValueType || property.PropertyType.GetTypeInfo().IsEnum || property.PropertyType.Equals(typeof(string)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    else
                    {
                        var objPropertyValue = property.GetValue(objSource, null);
                        if (objPropertyValue == null)
                        {
                            property.SetValue(objTarget, null, null);
                        }
                        else
                        {
                            property.SetValue(objTarget, objPropertyValue.CloneObject(), null);
                        }
                    }
                }
            }
            return objTarget;
        }

    }
}