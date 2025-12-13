using System.Reflection;
using System.Resources;

namespace HappyTools.CrossCutting.Wrapper.Utilities
{
    public static partial class ResourceUtility
    {

        public static string GetResource(Type resourseType, string resourceKey)
        {
            var rm = new ResourceManager(resourseType);
            return rm.GetString(resourceKey);
        }
        public static string GetResource(Type resourseType, string resourceKey, System.Globalization.CultureInfo culture)
        {
            var rm = new ResourceManager(resourseType);
            return rm.GetString(resourceKey, culture);
        }
        public static string GetResource(string baseResourceName, Assembly assembly, string resourceKey)
        {
            var rm = new ResourceManager(baseResourceName, assembly);
            return rm.GetString(resourceKey);
        }
        public static string GetResource(string baseResourceName, Assembly assembly, string resourceKey, System.Globalization.CultureInfo culture)
        {
            var rm = new ResourceManager(baseResourceName, assembly);
            return rm.GetString(resourceKey, culture);
        }


    }
}
