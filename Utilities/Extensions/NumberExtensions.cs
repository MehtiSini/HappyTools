using HappyTools.Utilities.Extensions;

namespace HappyTools.Utilities.Extensions
{
    public static class NumericExtensions
    {
        public static string ToFileSizeDisplay(this int i)
        {
            return ((long)i).ToFileSizeDisplay(2);
        }

        public static string ToFileSizeDisplay(this int i, int decimals)
        {
            return ((long)i).ToFileSizeDisplay(decimals);
        }

        public static string ToFileSizeDisplay(this long i)
        {
            return i.ToFileSizeDisplay(2);
        }

        public static string ToFileSizeDisplay(this long i, int decimals)
        {
            if (i < 1024 * 1024 * 1024) // 1 GB
            {
                var value = Math.Round(i / 1024m / 1024m, decimals).ToString("N" + decimals);
                if (decimals > 0 && value.EndsWith(new string('0', decimals)))
                    value = value.Substring(0, value.Length - decimals - 1);

                return string.Concat(value, " MB");
            }
            else
            {
                var value = Math.Round(i / 1024m / 1024m / 1024m, decimals).ToString("N" + decimals);
                if (decimals > 0 && value.EndsWith(new string('0', decimals)))
                    value = value.Substring(0, value.Length - decimals - 1);

                return string.Concat(value, " GB");
            }
        }

        public static string ToOrdinal(this int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num.ToString("#,###0") + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num.ToString("#,###0") + "st";
                case 2:
                    return num.ToString("#,###0") + "nd";
                case 3:
                    return num.ToString("#,###0") + "rd";
                default:
                    return num.ToString("#,###0") + "th";
            }
        }

        public static string SplitDecimalByThree(this decimal number)
        {
            var parts = number.ToString().Split('.');
            string integerPart = parts[0];
            string decimalPart = parts.Length > 1 ? parts[1] : string.Empty;

            // Format the integer part
            string formattedIntegerPart = string.Join(",", Enumerable
                .Range(0, (integerPart.Length + 2) / 3)
                .Select(i => new string(integerPart.Reverse().Skip(i * 3).Take(3).Reverse().ToArray()))
                .Where(s => !string.IsNullOrEmpty(s))
                .Reverse());

            // Combine with decimal part if exists
            return string.IsNullOrEmpty(decimalPart)
                ? formattedIntegerPart
                : $"{formattedIntegerPart}.{decimalPart}";
        }

    }
}
