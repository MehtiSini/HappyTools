using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HappyTools.Utilities.Extensions.HttpClient.HttpError
{
    public class ModelError
    {

        public ModelError(Exception exception)
            : this(exception, null /* errorMessage */)
        {
        }

        public ModelError(Exception exception, string errorMessage)
            : this(errorMessage)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            Exception = exception;
        }

        public ModelError(string errorMessage)
        {
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public Exception Exception {
            get;
            private set;
        }

        public string ErrorMessage {
            get;
            private set;
        }
    }
    public class ModelErrorCollection : Collection<ModelError>
    {

        public void Add(Exception exception)
        {
            Add(new ModelError(exception));
        }

        public void Add(string errorMessage)
        {
            Add(new ModelError(errorMessage));
        }
    }
    public class ValueProviderResult
    {

        private static readonly CultureInfo _staticCulture = CultureInfo.InvariantCulture;
        private CultureInfo _instanceCulture;

        // default constructor so that subclassed types can set the properties themselves
        protected ValueProviderResult()
        {
        }

        public ValueProviderResult(object rawValue, string attemptedValue, CultureInfo culture)
        {
            RawValue = rawValue;
            AttemptedValue = attemptedValue;
            Culture = culture;
        }

        public string AttemptedValue {
            get;
            protected set;
        }

        public CultureInfo Culture {
            get {
                if (_instanceCulture == null)
                {
                    _instanceCulture = _staticCulture;
                }
                return _instanceCulture;
            }
            protected set {
                _instanceCulture = value;
            }
        }

        public object RawValue {
            get;
            protected set;
        }

        private static object ConvertSimpleType(CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType.IsInstanceOfType(value))
            {
                return value;
            }

            // if this is a user-input value but the user didn't type anything, return no value
            var valueAsString = value as string;
            if (valueAsString != null && valueAsString.Trim().Length == 0)
            {
                return null;
            }

            var converter = TypeDescriptor.GetConverter(destinationType);
            var canConvertFrom = converter.CanConvertFrom(value.GetType());
            if (!canConvertFrom)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!(canConvertFrom || converter.CanConvertTo(destinationType)))
            {
                // EnumConverter cannot convert integer, so we verify manually
                if (destinationType.GetTypeInfo().IsEnum && value is int)
                {
                    return Enum.ToObject(destinationType, (int)value);
                }

                // In case of a Nullable object, we try again with its underlying type.
                var underlyingType = Nullable.GetUnderlyingType(destinationType);
                if (underlyingType != null)
                {
                    return ConvertSimpleType(culture, value, underlyingType);
                }

                var message = string.Format(CultureInfo.CurrentCulture, "Cannot find any suitable converter!",
                    value.GetType().FullName, destinationType.FullName);
                throw new InvalidOperationException(message);
            }

            try
            {
                var convertedValue = canConvertFrom ?
                    converter.ConvertFrom(null /* context */, culture, value) :
                    converter.ConvertTo(null /* context */, culture, value, destinationType);
                return convertedValue;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, "error while converting type",
                    value.GetType().FullName, destinationType.FullName);
                throw new InvalidOperationException(message, ex);
            }
        }

        public object ConvertTo(Type type)
        {
            return ConvertTo(type, null /* culture */);
        }

        public virtual object ConvertTo(Type type, CultureInfo culture)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var cultureToUse = culture ?? Culture;
            return UnwrapPossibleArrayType(cultureToUse, RawValue, type);
        }

        private static object UnwrapPossibleArrayType(CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType.IsInstanceOfType(value))
            {
                return value;
            }

            // array conversion results in four cases, as below
            var valueAsArray = value as Array;
            if (destinationType.IsArray)
            {
                var destinationElementType = destinationType.GetElementType();
                if (valueAsArray != null)
                {
                    // case 1: both destination + source type are arrays, so convert each element
                    IList converted = Array.CreateInstance(destinationElementType, valueAsArray.Length);
                    for (var i = 0; i < valueAsArray.Length; i++)
                    {
                        converted[i] = ConvertSimpleType(culture, valueAsArray.GetValue(i), destinationElementType);
                    }
                    return converted;
                }
                else
                {
                    // case 2: destination type is array but source is single element, so wrap element in array + convert
                    var element = ConvertSimpleType(culture, value, destinationElementType);
                    IList converted = Array.CreateInstance(destinationElementType, 1);
                    converted[0] = element;
                    return converted;
                }
            }
            else if (valueAsArray != null)
            {
                // case 3: destination type is single element but source is array, so extract first element + convert
                if (valueAsArray.Length > 0)
                {
                    value = valueAsArray.GetValue(0);
                    return ConvertSimpleType(culture, value, destinationType);
                }
                else
                {
                    // case 3(a): source is empty array, so can't perform conversion
                    return null;
                }
            }
            // case 4: both destination + source type are single elements, so convert
            return ConvertSimpleType(culture, value, destinationType);
        }

    }

    public class ModelState
    {

        private ModelErrorCollection _errors = new ModelErrorCollection();

        public ValueProviderResult Value {
            get;
            set;
        }

        public ModelErrorCollection Errors {
            get {
                return _errors;
            }
        }
    }
    internal static class DictionaryHelpers
    {

        public static IEnumerable<KeyValuePair<string, TValue>> FindKeysWithPrefix<TValue>(IDictionary<string, TValue> dictionary, string prefix)
        {
            TValue exactMatchValue;
            if (dictionary.TryGetValue(prefix, out exactMatchValue))
            {
                yield return new KeyValuePair<string, TValue>(prefix, exactMatchValue);
            }

            foreach (var entry in dictionary)
            {
                var key = entry.Key;

                if (key.Length <= prefix.Length)
                {
                    continue;
                }

                if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var charAfterPrefix = key[prefix.Length];
                switch (charAfterPrefix)
                {
                    case '[':
                    case '.':
                        yield return entry;
                        break;
                }
            }
        }

        public static bool DoesAnyKeyHavePrefix<TValue>(IDictionary<string, TValue> dictionary, string prefix)
        {
            return FindKeysWithPrefix(dictionary, prefix).Any();
        }

    }

    public class ModelStateDictionary : IDictionary<string, ModelState>
    {

        private readonly Dictionary<string, ModelState> _innerDictionary = new Dictionary<string, ModelState>(StringComparer.OrdinalIgnoreCase);

        public ModelStateDictionary()
        {
        }

        public ModelStateDictionary(ModelStateDictionary dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            foreach (var entry in dictionary)
            {
                _innerDictionary.Add(entry.Key, entry.Value);
            }
        }

        public int Count {
            get {
                return _innerDictionary.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return ((IDictionary<string, ModelState>)_innerDictionary).IsReadOnly;
            }
        }

        public bool IsValid {
            get {
                return Values.All(modelState => modelState.Errors.Count == 0);
            }
        }

        public ICollection<string> Keys {
            get {
                return _innerDictionary.Keys;
            }
        }

        public ModelState this[string key] {
            get {
                ModelState value;
                _innerDictionary.TryGetValue(key, out value);
                return value;
            }
            set {
                _innerDictionary[key] = value;
            }
        }

        public ICollection<ModelState> Values {
            get {
                return _innerDictionary.Values;
            }
        }

        public void Add(KeyValuePair<string, ModelState> item)
        {
            ((IDictionary<string, ModelState>)_innerDictionary).Add(item);
        }

        public void Add(string key, ModelState value)
        {
            _innerDictionary.Add(key, value);
        }

        public void AddModelError(string key, Exception exception)
        {
            GetModelStateForKey(key).Errors.Add(exception);
        }

        public void AddModelError(string key, string errorMessage)
        {
            GetModelStateForKey(key).Errors.Add(errorMessage);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, ModelState> item)
        {
            return ((IDictionary<string, ModelState>)_innerDictionary).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, ModelState>[] array, int arrayIndex)
        {
            ((IDictionary<string, ModelState>)_innerDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, ModelState>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        private ModelState GetModelStateForKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            ModelState modelState;
            if (!TryGetValue(key, out modelState))
            {
                modelState = new ModelState();
                this[key] = modelState;
            }

            return modelState;
        }

        public bool IsValidField(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            // if the key is not found in the dictionary, we just say that it's valid (since there are no errors)
            return DictionaryHelpers.FindKeysWithPrefix(this, key).All(entry => entry.Value.Errors.Count == 0);
        }

        public void Merge(ModelStateDictionary dictionary)
        {
            if (dictionary == null)
            {
                return;
            }

            foreach (var entry in dictionary)
            {
                this[entry.Key] = entry.Value;
            }
        }

        public bool Remove(KeyValuePair<string, ModelState> item)
        {
            return ((IDictionary<string, ModelState>)_innerDictionary).Remove(item);
        }

        public bool Remove(string key)
        {
            return _innerDictionary.Remove(key);
        }

        public void SetModelValue(string key, ValueProviderResult value)
        {
            GetModelStateForKey(key).Value = value;
        }

        public bool TryGetValue(string key, out ModelState value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_innerDictionary).GetEnumerator();
        }
        #endregion

    }
}
