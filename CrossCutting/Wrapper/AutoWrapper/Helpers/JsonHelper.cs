using HappyTools.CrossCutting.Wrapper.AutoWrapper.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HappyTools.CrossCutting.Wrapper.AutoWrapper.Helpers
{
    public static class JSONHelper
    {
        public static JsonSerializerSettings GetJSONSettings(bool ignoreNull = true, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore, bool useCamelCaseNaming = true)
        {
            return new CamelCaseContractResolverJsonSettings().GetJSONSettings(ignoreNull, referenceLoopHandling, useCamelCaseNaming);
        }

        public static (JsonSerializerSettings Settings, Dictionary<string, string> Mappings) GetJSONSettings<T>(bool ignoreNull = true, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore, bool useCamelCaseNaming = true)
        {
            return new CustomContractResolverJsonSettings<T>().GetJSONSettings(ignoreNull, referenceLoopHandling, useCamelCaseNaming);
        }

        public static bool HasProperty(dynamic obj, string name)
        {
            if (obj is JObject) return ((JObject)obj).ContainsKey(name);
            return obj.GetType().GetProperty(name) != null;
        }
        public static JToken RemoveEmptyChildren(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                var copy = new JObject();
                foreach (var prop in token.Children<JProperty>())
                {
                    var child = prop.Value;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }

                    if (!child.IsNullOrEmpty())
                    {
                        copy.Add(prop.Name, child);
                    }
                }
                return copy;
            }
            else if (token.Type == JTokenType.Array)
            {
                var copy = new JArray();
                foreach (var item in token.Children())
                {
                    var child = item;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }

                    if (!child.IsNullOrEmpty())
                    {
                        copy.Add(child);
                    }
                }
                return copy;
            }
            return token;
        }

        public static bool IsEmpty(JToken token)
        {
            return token.Type == JTokenType.Null;
        }
    }
}
