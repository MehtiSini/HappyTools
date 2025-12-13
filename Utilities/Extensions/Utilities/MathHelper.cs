namespace HappyTools.Utilities.Extensions.Utilities
{
    public static class MathHelper
    {
        public static long CalculateCountWithPercent(long count, float percent)
        {
            return (long)(count * percent / 100);
        }

        public static decimal CalculatePriceWithPercent(decimal price, decimal percent)
        {
            return price + price * percent / 100;
        }

        public static decimal CalculatePriceWithPercentAndTax(decimal price, decimal percent, decimal tax)
        {
            var result = price + (long)(price * percent / 100);
            result += (long)(result * tax / 100);
            return result;
        }

        public static long CalculatePriceWithDiscountAmount(long price, long discountAmount)
        {
            return price - discountAmount;
        }

        public static long CalculatePriceWithFixedTax(long price, long taxAmount)
        {
            return price + taxAmount;
        }

        public static decimal CalculateFinalPrice(decimal price, decimal discountPercent, decimal taxPercent)
        {
            var discountedPrice = CalculatePriceWithPercent(price, -discountPercent);
            return CalculatePriceWithPercent(discountedPrice, taxPercent);
        }

        public static float CalculatePercentageDifference(long originalValue, long newValue)
        {
            if (originalValue == 0)
                return 0;
            return (float)(newValue - originalValue) / originalValue * 100;
        }

        public static long AdjustValueWithinRange(long value, long min, long max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
