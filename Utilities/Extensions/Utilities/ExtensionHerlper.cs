using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Utilities.Extensions.Utilities
{
    public static class ExtensionHerlper
    {
        public static bool IsNullOrHasNegativeCount(this object obj)
        {
            if (obj == null)
                return true;

            if (obj is ICollection collection)
                return collection.Count <= 0;

            return false;
        }


        public static bool IsNotNullOrHasPositiveCount(this object obj)
        {
            if (obj == null)
                return false;

            if (obj is ICollection collection)
                return collection.Count > 0;

            return true;
        }

   
    }
}
