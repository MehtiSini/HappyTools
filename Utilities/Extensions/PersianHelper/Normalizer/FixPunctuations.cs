using System.Linq;

namespace HappyTools.Utilities.Extensions.PersianHelper.Normalizer
{
    public static class FixPunctuations
    {

        public static string RemovePunctuation(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ?
                string.Empty :
                new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        }
    }
}