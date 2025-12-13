using System.Globalization;
using System.Linq;
using System.Text;

namespace HappyTools.Utilities.Extensions
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2162
    /// </summary>
    public static class UnicodeExtensions
    {


        public static string CleanUnderLines(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            const char chr1600 = (char)1600; //ـ=1600
            const char chr8204 = (char)8204; //‌=8204

            return text.Replace(chr1600.ToString(), "")
                       .Replace(chr8204.ToString(), "");
        }


    }
}