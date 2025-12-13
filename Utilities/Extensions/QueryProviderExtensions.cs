using System.Collections.Generic;
using System.Text;

namespace HappyTools.Utilities.Extensions
{
    public static class QueryProviderExtensions
    {
        public static bool IsEntityFrameworkProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName == "System.Data.Objects.ELinq.ObjectQueryProvider";
        }

        public static bool IsLinqToObjectsProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName.Contains("EnumerableQuery");
        }

        public static bool IsOpenAccessProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName.Contains("Telerik.OpenAccess");
        }
    }
}