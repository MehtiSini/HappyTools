namespace HappyTools.Utilities.Extensions
{
    /// <summary>
    /// Contains extension methods for the <see cref="decimal"/> class
    /// </summary>
    public static class DecimalExtenders
    {
        /// <summary>
        /// Rounds the supplied decimal to the specified amount of decimal points
        /// </summary>
        /// <param name="val">The decimal to round</param>
        /// <param name="decimalPoints">The number of decimal points to round the output value to</param>
        /// <returns>A rounded decimal</returns>
        public static decimal RoundDecimalPoints(this decimal val, int decimalPoints)
        {
            return Math.Round(val, decimalPoints);
        }

        /// <summary>
        /// Rounds the supplied decimal value to two decimal points
        /// </summary>
        /// <param name="val">The decimal to round</param>
        /// <returns>A decimal value rounded to two decimal points</returns>
        public static decimal RoundToTwoDecimalPoints(this decimal val)
        {
            return Math.Round(val, 2);
        }
    }
}