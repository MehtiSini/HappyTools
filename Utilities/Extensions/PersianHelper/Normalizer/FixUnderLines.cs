namespace HappyTools.Utilities.Extensions.PersianHelper.Normalizer
{
    public static class FixUnderLines
    {
        public static string RemoveUnderLines(this string text)
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