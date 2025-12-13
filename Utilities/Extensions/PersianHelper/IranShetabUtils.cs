using System.Text.RegularExpressions;
using HappyTools.Utilities.Extensions;

namespace HappyTools.Utilities.Extensions.PersianHelper
{
    public class Bank
    {
        public string BankNameFa { get; set; }
        public string BankNameEn { get; set; }
        public int[] Perfixes { get; set; }
    }
    /// <summary>
    /// Credit and Debit Card (Shetab) validation
    /// </summary>
    public static class IranShetabUtils
    {
        public static ISet<Bank> Banks = new HashSet<Bank>(new List<Bank>()
        {
            new Bank()
            {
                BankNameEn ="Eghtesad Novin",
                BankNameFa = "بانک اقتصاد نوین",
                Perfixes = 627412.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Ansar",
                BankNameFa = "بانک انصار",
                Perfixes = 627381.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Iran Zamin",
                BankNameFa = "بانک ایران زمین",
                Perfixes = 505785.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Parsian",
                BankNameFa = "بانک پارسیان",
                Perfixes =new int[]{ 639194 , 627884, 622106 }
            },
            new Bank()
            {
                BankNameEn ="Pasargard",
                BankNameFa = "بانک پاسارگاد",
                Perfixes =new int[]{ 639347, 502229 }
            },
            new Bank()
            {
                BankNameEn ="Taat",
                BankNameFa = "بانک تات",
                Perfixes =636214.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Tejarat",
                BankNameFa = "بانک تجارت",
                Perfixes =627353.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Tose`e Ta`avon",
                BankNameFa = "بانک توسعه تعاون",
                Perfixes =502908.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Tose`e Saderat Iran",
                BankNameFa = "بانک توسعه صادرات ایران",
                Perfixes =new int[]{ 627648, 207177 }
            },
            new Bank()
            {
                BankNameEn ="Hekmat Iranian",
                BankNameFa = "بانک حکمت ایرانیان",
                Perfixes =636949.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Day",
                BankNameFa = "بانک دی",
                Perfixes =502938.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Rafah",
                BankNameFa = "بانک رفاه کارگران",
                Perfixes =589463.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="",
                BankNameFa = "بانک سامان",
                Perfixes =new int[]{ 621986 }
            },
            new Bank()
            {
                BankNameEn ="Sepah",
                BankNameFa = "بانک سپه",
                Perfixes =589210.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Sarmayeh",
                BankNameFa = "بانک سرمایه",
                Perfixes =639607.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Sina",
                BankNameFa = "بانک سینا",
                Perfixes =639346.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="City Bank",
                BankNameFa = "بانک شهر",
                Perfixes =502806.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Saderat",
                BankNameFa = "بانک صادرات ایران",
                Perfixes =603769.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Bank of Industry and Mine",
                BankNameFa = "بانک صنعت و معدن",
                Perfixes =627961.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Iran's Mehr Bank",
                BankNameFa = "بانک قرض الحسنه مهر ایران",
                Perfixes =606373.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Ghavamin",
                BankNameFa = "بانک قوامین",
                Perfixes =639599.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Karafarin",
                BankNameFa = "بانک کارآفرین",
                Perfixes =new int[]{ 627488, 502910 }
            },
            new Bank()
            {
                BankNameEn ="Keshavarzi",
                BankNameFa = "بانک کشاورزی",
                Perfixes =new int[]{ 603770, 639217 }
            },
            new Bank()
            {
                BankNameEn ="Tourism Bank",
                BankNameFa = "بانک گردشگری",
                Perfixes =505416.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Markazi",
                BankNameFa = "بانک مرکزی",
                Perfixes =636795.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Maskan",
                BankNameFa = "بانک مسکن",
                Perfixes =628023.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Mellat Bank",
                BankNameFa = "بانک ملت",
                Perfixes =new int[]{ 610433, 991975 }
            },
            new Bank()
            {
                BankNameEn ="Melli",
                BankNameFa = "بانک ملی ایران",
                Perfixes =603799.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Mehr Eghtesad",
                BankNameFa = "بانک مهر اقتصاد",
                Perfixes =639370.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Post Bank",
                BankNameFa = "پست بانک ایران",
                Perfixes =627760.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Tosse Institute",
                BankNameFa = "موسسه اعتباری توسعه",
                Perfixes =628157.ToSingleItemArray()
            },
            new Bank()
            {
                BankNameEn ="Kosar FCI",
                BankNameFa = "موسسه اعتباری کوثر",
                Perfixes =505801.ToSingleItemArray()
            },
        });
        private static readonly Regex _matchIranShetab = new Regex(@"[0-9]{16}", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// validate Shetab card numbers
        /// </summary>
        /// <param name="creditCardNumber">Shetab card number</param>
        public static bool IsValidIranShetabNumber(this string creditCardNumber)
        {
            if (string.IsNullOrEmpty(creditCardNumber))
            {
                return false;
            }

            creditCardNumber = creditCardNumber.Replace("-", string.Empty);

            if (creditCardNumber.Length != 16)
            {
                return false;
            }

            if (!_matchIranShetab.IsMatch(creditCardNumber))
            {
                return false;
            }

            var sumOfDigits = creditCardNumber.Where(e => e >= '0' && e <= '9')
                .Reverse()
                .Select((e, i) => (e - 48) * (i % 2 == 0 ? 1 : 2))
                .Sum(e => e / 10 + e % 10);

            return sumOfDigits % 10 == 0;
        }
        /// <summary>
        /// دریافت بانک از روی 6 شماره اول کارت بانکی
        /// </summary>
        /// <param name="creditCardNumberPerfix"></param>
        /// <returns></returns>
        public static Bank GetBankByPerfix(this string creditCardNumberPerfix)
        {
            Bank res = null;
            var perfix = int.Parse(creditCardNumberPerfix.RemoveAllSpecialCharacters().Replace("-", string.Empty).Substring(0, 6));
            Banks.ForEach(b =>
            {
                b.Perfixes.ForEach(p =>
                {
                    if (p == perfix)
                        res = b;
                });
            });
            return res;

        }
        /// <summary>
        /// دریافت بانک از روی شماره کارت بانکی
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        public static Bank GetBank(this string creditCardNumber)
        {
            Bank res = null;
            if (!creditCardNumber.IsValidIranShetabNumber())
                return res;
            var cardNum = long.Parse(creditCardNumber.RemoveAllSpecialCharacters().Replace("-", string.Empty)).ToString();
            var perfix = int.Parse(cardNum.Substring(0, 6));
            Banks.ForEach(b =>
            {
                b.Perfixes.ForEach(p =>
                {
                    if (p == perfix)
                        res = b;
                });
            });
            return res;

        }
    }
}