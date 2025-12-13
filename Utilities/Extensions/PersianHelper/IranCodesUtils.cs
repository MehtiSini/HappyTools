using System.Text.RegularExpressions;

namespace HappyTools.Utilities.Extensions.PersianHelper
{
    /// <summary>
    /// IranCodes Utils
    /// </summary>
    public static class IranCodesUtils
    {
        private static readonly Regex _matchIranianMobileNumber1 = new Regex(@"^(((98)|(\+98)|(0098)|0)(9){1}[0-9]{9})+$", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _matchIranianMobileNumber2 = new Regex(@"^(9){1}[0-9]{9}$", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _matchIranianPhoneNumber = new Regex("^[2-9][0-9]{7}$", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _matchIranianPostalCode = new Regex(@"^(\d{5}-?\d{5})$", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Validate Iranian mobile number
        /// </summary>
        public static bool IsValidIranianMobileNumber(this string mobileNumber)
        {
            return !string.IsNullOrWhiteSpace(mobileNumber) &&
                (_matchIranianMobileNumber1.IsMatch(mobileNumber) || _matchIranianMobileNumber2.IsMatch(mobileNumber));
        }

        /// <summary>
        /// Validate Iranian phone number
        /// </summary>
        public static bool IsValidIranianPhoneNumber(this string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && _matchIranianPhoneNumber.IsMatch(phoneNumber);
        }

        /// <summary>
        /// Validate Iranian postal code
        /// </summary>
        public static bool IsValidIranianPostalCode(this string postalCode)
        {
            return !string.IsNullOrWhiteSpace(postalCode) && _matchIranianPostalCode.IsMatch(postalCode);
        }
        /// <summary>
        /// Remove iran country code from mobile number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public static string NormalizeIranianMobileNumber(this string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return null;
            var englishMobileNumber = mobileNumber.ToEnglishNumbers();
            var isValid = englishMobileNumber.IsValidIranianMobileNumber();
            if (isValid)
            {
                if (englishMobileNumber.Length == 11)
                    return englishMobileNumber;
                if (englishMobileNumber.Length == 10)
                    return $"0{englishMobileNumber}";
                if (englishMobileNumber.Length > 10)
                {
                    return $"0{englishMobileNumber.Substring(englishMobileNumber.Length - 10)}";
                }


            }
            return null;
        }
    }
}