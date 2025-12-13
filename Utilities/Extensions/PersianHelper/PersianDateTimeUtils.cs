using System.Globalization;
using System.Linq;

namespace HappyTools.Utilities.Extensions.PersianHelper
{
    /// <summary>
    /// Represents PersianDateTime utils.
    /// </summary>
    public static class PersianDateTimeUtils
    {
        /// <summary>
        /// تعیین اعتبار تاریخ شمسی
        /// </summary>
        /// <param name="persianYear">سال شمسی</param>
        /// <param name="persianMonth">ماه شمسی</param>
        /// <param name="persianDay">روز شمسی</param>
        public static bool IsValidPersianDate(int persianYear, int persianMonth, int persianDay)
        {
            if (persianDay > 31 || persianDay <= 0)
            {
                return false;
            }

            if (persianMonth > 12 || persianMonth <= 0)
            {
                return false;
            }

            if (persianMonth <= 6 && persianDay > 31)
            {
                return false;
            }

            if (persianMonth >= 7 && persianDay > 30)
            {
                return false;
            }

            if (persianMonth == 12)
            {
                var persianCalendar = new PersianCalendar();
                var isLeapYear = persianCalendar.IsLeapYear(persianYear);

                if (isLeapYear && persianDay > 30)
                {
                    return false;
                }

                if (!isLeapYear && persianDay > 29)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// تعیین اعتبار تاریخ و زمان رشته‌ای شمسی
        /// با قالب‌های پشتیبانی شده‌ی ۹۰/۸/۱۴ , 1395/11/3 17:30 , ۱۳۹۰/۸/۱۴ , ۹۰-۸-۱۴ , ۱۳۹۰-۸-۱۴
        /// </summary>
        /// <param name="persianDateTime">تاریخ و زمان شمسی</param>
        public static bool IsValidPersianDateTime(this string persianDateTime)
        {
            try
            {
                var dt = persianDateTime.ToGregorianDateTime();
                return dt.HasValue;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// تبدیل تاریخ و زمان رشته‌ای شمسی به میلادی
        /// با قالب‌های پشتیبانی شده‌ی ۹۰/۸/۱۴ , 1395/11/3 17:30 , ۱۳۹۰/۸/۱۴ , ۹۰-۸-۱۴ , ۱۳۹۰-۸-۱۴
        /// </summary>
        /// <param name="persianDateTime">تاریخ و زمان شمسی</param>
        /// <returns>تاریخ و زمان قمری</returns>
        public static DateTime? ToGregorianDateTime(this string persianDateTime)
        {
            if (string.IsNullOrWhiteSpace(persianDateTime))
            {
                return null;
            }

            persianDateTime = persianDateTime.Trim().ToEnglishNumbers();
            var splitedDateTime = persianDateTime.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var rawTime = splitedDateTime.FirstOrDefault(s => s.Contains(':'));
            var rawDate = splitedDateTime.FirstOrDefault(s => !s.Contains(':'));

            if (rawDate?.Contains('/') == true)
            {
                var splitedDate = rawDate.Split('/');
                if (splitedDate.Length != 3)
                {
                    return null;
                }

                var day = getDay(splitedDate[1]);
                var month = getMonth(splitedDate[0]);
                var year = getYear(splitedDate[2]);

                if (!day.HasValue || !month.HasValue || !year.HasValue)
                {
                    return null;
                }

                if (!IsValidPersianDate(year.Value, month.Value, day.Value))
                {
                    return null;
                }

                var hour = 0;
                var minute = 0;
                var second = 0;

                if (!string.IsNullOrWhiteSpace(rawTime))
                {
                    var splitedTime = rawTime.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    hour = int.Parse(splitedTime[0]);
                    minute = int.Parse(splitedTime[1]);
                    if (splitedTime.Length > 2)
                    {
                        int.TryParse(splitedTime[2], out second);
                    }
                }

                var persianCalendar = new PersianCalendar();
                return persianCalendar.ToDateTime(year.Value, month.Value, day.Value, hour, minute, second, 0);
            }

            return null;
        }



        /// <summary>
        /// تبدیل تاریخ و زمان رشته‌ای شمسی به میلادی
        /// با قالب‌های پشتیبانی شده‌ی ۹۰/۸/۱۴ , 1395/11/3 17:30 , ۱۳۹۰/۸/۱۴ , ۹۰-۸-۱۴ , ۱۳۹۰-۸-۱۴
        /// </summary>
        /// <param name="persianDateTime">تاریخ و زمان شمسی</param>
        /// <returns>تاریخ و زمان قمری</returns>
        public static DateTimeOffset? ToGregorianDateTimeOffset(this string persianDateTime)
        {
            var dateTime = persianDateTime.ToGregorianDateTime();
            if (dateTime == null)
            {
                return null;
            }

            return new DateTimeOffset(dateTime.Value, DateTimeUtils.IranStandardTime.BaseUtcOffset);
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToLongPersianDateString(this DateTime dt)
        {
            return dt.ToPersianDateTimeString(PersianCulture.Instance.DateTimeFormat.LongDatePattern);
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToLongPersianDateString(this DateTime? dt)
        {
            return dt == null ? string.Empty : dt.Value.ToLongPersianDateString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        public static string ToLongPersianDateString(this DateTimeOffset? dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt == null ? string.Empty : dt.Value.GetDateTimeOffsetPart(dateTimeOffsetPart).ToLongPersianDateString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        public static string ToLongPersianDateString(this DateTimeOffset dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt.GetDateTimeOffsetPart(dateTimeOffsetPart).ToLongPersianDateString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395، 10:20:02 ق.ظ
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToLongPersianDateTimeString(this DateTime dt)
        {
            return dt.ToPersianDateTimeString(
                $"{PersianCulture.Instance.DateTimeFormat.LongDatePattern}، {PersianCulture.Instance.DateTimeFormat.LongTimePattern}");
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395، 10:20:02 ق.ظ
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToLongPersianDateTimeString(this DateTime? dt)
        {
            return dt == null ? string.Empty : dt.Value.ToLongPersianDateTimeString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395، 10:20:02 ق.ظ
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        public static string ToLongPersianDateTimeString(this DateTimeOffset? dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt == null ? string.Empty : dt.Value.GetDateTimeOffsetPart(dateTimeOffsetPart).ToLongPersianDateTimeString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 21 دی 1395، 10:20:02 ق.ظ
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        public static string ToLongPersianDateTimeString(this DateTimeOffset dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt.GetDateTimeOffsetPart(dateTimeOffsetPart).ToLongPersianDateTimeString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToPersianDateTimeString(this DateTime dateTime, string format)
        {
            return dateTime.ToString(format, PersianCulture.Instance);
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی و دریافت اجزای سال، ماه و روز نتیجه‌ی حاصل‌
        /// </summary>
        /// <param name="gregorianDate">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        public static Tuple<int, int, int> ToPersianYearMonthDay(this DateTimeOffset? gregorianDate, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return gregorianDate == null ? null : gregorianDate.Value.GetDateTimeOffsetPart(dateTimeOffsetPart).ToPersianYearMonthDay();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی و دریافت اجزای سال، ماه و روز نتیجه‌ی حاصل‌
        /// </summary>
        public static Tuple<int, int, int> ToPersianYearMonthDay(this DateTime? gregorianDate)
        {
            return gregorianDate == null ? null : gregorianDate.Value.ToPersianYearMonthDay();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی و دریافت اجزای سال، ماه و روز نتیجه‌ی حاصل‌
        /// </summary>
        /// <param name="gregorianDate">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        public static Tuple<int, int, int> ToPersianYearMonthDay(this DateTimeOffset gregorianDate, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return gregorianDate.GetDateTimeOffsetPart(dateTimeOffsetPart).ToPersianYearMonthDay();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی و دریافت اجزای سال، ماه و روز نتیجه‌ی حاصل‌
        /// </summary>
        public static Tuple<int, int, int> ToPersianYearMonthDay(this DateTime gregorianDate)
        {
            var persianCalendar = new PersianCalendar();
            var persianYear = persianCalendar.GetYear(gregorianDate);
            var persianMonth = persianCalendar.GetMonth(gregorianDate);
            var persianDay = persianCalendar.GetDayOfMonth(gregorianDate);
            return new Tuple<int, int, int>(persianYear, persianMonth, persianDay);
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21
        /// </summary>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateString(this DateTimeOffset? dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt == null ? string.Empty : dt.Value.GetDateTimeOffsetPart(dateTimeOffsetPart).ToShortPersianDateString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21
        /// </summary>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateString(this DateTimeOffset dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt.GetDateTimeOffsetPart(dateTimeOffsetPart).ToShortPersianDateString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateString(this DateTime dt)
        {
            return dt.ToPersianDateTimeString(PersianCulture.Instance.DateTimeFormat.ShortDatePattern);
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateString(this DateTime? dt)
        {
            return dt == null ? string.Empty : dt.Value.ToShortPersianDateString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21 10:20
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateTimeString(this DateTime dt)
        {

            return dt.ToPersianDateTimeString(
                $"{PersianCulture.Instance.DateTimeFormat.ShortTimePattern} {PersianCulture.Instance.DateTimeFormat.ShortDatePattern}");
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21 10:20
        /// </summary>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateTimeString(this DateTime? dt)
        {
            return dt == null ? string.Empty : dt.Value.ToShortPersianDateTimeString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21 10:20
        /// </summary>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateTimeString(this DateTimeOffset? dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt == null ? string.Empty : dt.Value.GetDateTimeOffsetPart(dateTimeOffsetPart).ToShortPersianDateTimeString();
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// با قالبی مانند 1395/10/21 10:20
        /// </summary>
        /// <param name="dt">تاریخ و زمان</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        /// <returns>تاریخ شمسی</returns>
        public static string ToShortPersianDateTimeString(this DateTimeOffset dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return dt.GetDateTimeOffsetPart(dateTimeOffsetPart).ToShortPersianDateTimeString();
        }

        private static int? getDay(string part)
        {
            var day = part.toNumber();
            if (!day.Item1) return null;
            var pDay = day.Item2;
            if (pDay == 0 || pDay > 31) return null;
            return pDay;
        }

        private static int? getMonth(string part)
        {
            var month = part.toNumber();
            if (!month.Item1) return null;
            var pMonth = month.Item2;
            if (pMonth == 0 || pMonth > 12) return null;
            return pMonth;
        }

        private static int? getYear(string part)
        {
            var year = part.toNumber();
            if (!year.Item1) return null;
            var pYear = year.Item2;
            if (part.Length == 2) pYear += 1300;
            return pYear;
        }

        private static Tuple<bool, int> toNumber(this string data)
        {
            int number;
            var result = int.TryParse(data, out number);
            return new Tuple<bool, int>(result, number);
        }
    }
}