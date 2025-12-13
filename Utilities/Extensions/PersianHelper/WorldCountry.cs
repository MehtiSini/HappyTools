using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HappyTools.Utilities.Extensions.PersianHelper
{
    public class WorldCountry
    {
        public WorldCountry()
        {
            Name = null;
            Alpha2Code = null;
            Alpha3Code = null;
            NumericCode = null;
            Enabled = false;
        }

        public WorldCountry(string name, string alpha2Code, string alpha3Code, string numericCode, bool enabled)
        {
            Name = name;
            Alpha2Code = alpha2Code;
            Alpha3Code = alpha3Code;
            NumericCode = numericCode;
            Enabled = enabled;
        }

        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public string NumericCode { get; set; }
        public bool Enabled { get; set; }

        public override string ToString()
        {
            //Returns "USA - United States"
            return string.Format("{0} - {1}", Alpha3Code, Name);
        }
    }

    public class CountryArray
    {
        public List<WorldCountry> countries;
        public CountryArray()
        {
            countries = new List<WorldCountry>(50);
            countries.Add(new WorldCountry("Afghanistan", "AF", "AFG", "004", true));
            countries.Add(new WorldCountry("Aland Islands", "AX", "ALA", "248", true));
            countries.Add(new WorldCountry("Albania", "AL", "ALB", "008", true));
            countries.Add(new WorldCountry("Algeria", "DZ", "DZA", "012", true));
            countries.Add(new WorldCountry("American Samoa", "AS", "ASM", "016", true));
            countries.Add(new WorldCountry("Andorra", "AD", "AND", "020", true));
            countries.Add(new WorldCountry("Angola", "AO", "AGO", "024", true));
            countries.Add(new WorldCountry("Anguilla", "AI", "AIA", "660", true));
            countries.Add(new WorldCountry("Antarctica", "AQ", "ATA", "010", true));
            countries.Add(new WorldCountry("Antigua and Barbuda", "AG", "ATG", "028", true));
            countries.Add(new WorldCountry("Argentina", "AR", "ARG", "032", true));
            countries.Add(new WorldCountry("Armenia", "AM", "ARM", "051", true));
            countries.Add(new WorldCountry("Aruba", "AW", "ABW", "533", true));
            countries.Add(new WorldCountry("Australia", "AU", "AUS", "036", true));
            countries.Add(new WorldCountry("Austria", "AT", "AUT", "040", true));
            countries.Add(new WorldCountry("Azerbaijan", "AZ", "AZE", "031", true));
            countries.Add(new WorldCountry("Bahamas", "BS", "BHS", "044", true));
            countries.Add(new WorldCountry("Bahrain", "BH", "BHR", "048", true));
            countries.Add(new WorldCountry("Bangladesh", "BD", "BGD", "050", true));
            countries.Add(new WorldCountry("Barbados", "BB", "BRB", "052", true));
            countries.Add(new WorldCountry("Belarus", "BY", "BLR", "112", true));
            countries.Add(new WorldCountry("Belgium", "BE", "BEL", "056", true));
            countries.Add(new WorldCountry("Belize", "BZ", "BLZ", "084", true));
            countries.Add(new WorldCountry("Benin", "BJ", "BEN", "204", true));
            countries.Add(new WorldCountry("Bermuda", "BM", "BMU", "060", true));
            countries.Add(new WorldCountry("Bhutan", "BT", "BTN", "064", true));
            countries.Add(new WorldCountry("Bolivia, Plurinational State of", "BO", "BOL", "068", true));
            countries.Add(new WorldCountry("Bonaire, Sint Eustatius and Saba", "BQ", "BES", "535", true));
            countries.Add(new WorldCountry("Bosnia and Herzegovina", "BA", "BIH", "070", true));
            countries.Add(new WorldCountry("Botswana", "BW", "BWA", "072", true));
            countries.Add(new WorldCountry("Bouvet Island", "BV", "BVT", "074", true));
            countries.Add(new WorldCountry("Brazil", "BR", "BRA", "076", true));
            countries.Add(new WorldCountry("British Indian Ocean Territory", "IO", "IOT", "086", true));
            countries.Add(new WorldCountry("Brunei Darussalam", "BN", "BRN", "096", true));
            countries.Add(new WorldCountry("Bulgaria", "BG", "BGR", "100", true));
            countries.Add(new WorldCountry("Burkina Faso", "BF", "BFA", "854", true));
            countries.Add(new WorldCountry("Burundi", "BI", "BDI", "108", true));
            countries.Add(new WorldCountry("Cambodia", "KH", "KHM", "116", true));
            countries.Add(new WorldCountry("Cameroon", "CM", "CMR", "120", true));
            countries.Add(new WorldCountry("Canada", "CA", "CAN", "124", true));
            countries.Add(new WorldCountry("Cape Verde", "CV", "CPV", "132", true));
            countries.Add(new WorldCountry("Cayman Islands", "KY", "CYM", "136", true));
            countries.Add(new WorldCountry("Central African Republic", "CF", "CAF", "140", true));
            countries.Add(new WorldCountry("Chad", "TD", "TCD", "148", true));
            countries.Add(new WorldCountry("Chile", "CL", "CHL", "152", true));
            countries.Add(new WorldCountry("China", "CN", "CHN", "156", true));
            countries.Add(new WorldCountry("Christmas Island", "CX", "CXR", "162", true));
            countries.Add(new WorldCountry("Cocos (Keeling) Islands", "CC", "CCK", "166", true));
            countries.Add(new WorldCountry("Colombia", "CO", "COL", "170", true));
            countries.Add(new WorldCountry("Comoros", "KM", "COM", "174", true));
            countries.Add(new WorldCountry("Congo", "CG", "COG", "178", true));
            countries.Add(new WorldCountry("Congo, the Democratic Republic of the", "CD", "COD", "180", true));
            countries.Add(new WorldCountry("Cook Islands", "CK", "COK", "184", true));
            countries.Add(new WorldCountry("Costa Rica", "CR", "CRI", "188", true));
            countries.Add(new WorldCountry("Cote d'Ivoire", "CI", "CIV", "384", true));
            countries.Add(new WorldCountry("Croatia", "HR", "HRV", "191", true));
            countries.Add(new WorldCountry("Cuba", "CU", "CUB", "192", true));
            countries.Add(new WorldCountry("Curacao", "CW", "CUW", "531", true));
            countries.Add(new WorldCountry("Cyprus", "CY", "CYP", "196", true));
            countries.Add(new WorldCountry("Czech Republic", "CZ", "CZE", "203", true));
            countries.Add(new WorldCountry("Denmark", "DK", "DNK", "208", true));
            countries.Add(new WorldCountry("Djibouti", "DJ", "DJI", "262", true));
            countries.Add(new WorldCountry("Dominica", "DM", "DMA", "212", true));
            countries.Add(new WorldCountry("Dominican Republic", "DO", "DOM", "214", true));
            countries.Add(new WorldCountry("Ecuador", "EC", "ECU", "218", true));
            countries.Add(new WorldCountry("Egypt", "EG", "EGY", "818", true));
            countries.Add(new WorldCountry("El Salvador", "SV", "SLV", "222", true));
            countries.Add(new WorldCountry("Equatorial Guinea", "GQ", "GNQ", "226", true));
            countries.Add(new WorldCountry("Eritrea", "ER", "ERI", "232", true));
            countries.Add(new WorldCountry("Estonia", "EE", "EST", "233", true));
            countries.Add(new WorldCountry("Ethiopia", "ET", "ETH", "231", true));
            countries.Add(new WorldCountry("Falkland Islands (Malvinas)", "FK", "FLK", "238", true));
            countries.Add(new WorldCountry("Faroe Islands", "FO", "FRO", "234", true));
            countries.Add(new WorldCountry("Fiji", "FJ", "FJI", "242", true));
            countries.Add(new WorldCountry("Finland", "FI", "FIN", "246", true));
            countries.Add(new WorldCountry("France", "FR", "FRA", "250", true));
            countries.Add(new WorldCountry("French Guiana", "GF", "GUF", "254", true));
            countries.Add(new WorldCountry("French Polynesia", "PF", "PYF", "258", true));
            countries.Add(new WorldCountry("French Southern Territories", "TF", "ATF", "260", true));
            countries.Add(new WorldCountry("Gabon", "GA", "GAB", "266", true));
            countries.Add(new WorldCountry("Gambia", "GM", "GMB", "270", true));
            countries.Add(new WorldCountry("Georgia", "GE", "GEO", "268", true));
            countries.Add(new WorldCountry("Germany", "DE", "DEU", "276", true));
            countries.Add(new WorldCountry("Ghana", "GH", "GHA", "288", true));
            countries.Add(new WorldCountry("Gibraltar", "GI", "GIB", "292", true));
            countries.Add(new WorldCountry("Greece", "GR", "GRC", "300", true));
            countries.Add(new WorldCountry("Greenland", "GL", "GRL", "304", true));
            countries.Add(new WorldCountry("Grenada", "GD", "GRD", "308", true));
            countries.Add(new WorldCountry("Guadeloupe", "GP", "GLP", "312", true));
            countries.Add(new WorldCountry("Guam", "GU", "GUM", "316", true));
            countries.Add(new WorldCountry("Guatemala", "GT", "GTM", "320", true));
            countries.Add(new WorldCountry("Guernsey", "GG", "GGY", "831", true));
            countries.Add(new WorldCountry("Guinea", "GN", "GIN", "324", true));
            countries.Add(new WorldCountry("Guinea-Bissau", "GW", "GNB", "624", true));
            countries.Add(new WorldCountry("Guyana", "GY", "GUY", "328", true));
            countries.Add(new WorldCountry("Haiti", "HT", "HTI", "332", true));
            countries.Add(new WorldCountry("Heard Island and McDonald Islands", "HM", "HMD", "334", true));
            countries.Add(new WorldCountry("Holy See (Vatican City State)", "VA", "VAT", "336", true));
            countries.Add(new WorldCountry("Honduras", "HN", "HND", "340", true));
            countries.Add(new WorldCountry("Hong Kong", "HK", "HKG", "344", true));
            countries.Add(new WorldCountry("Hungary", "HU", "HUN", "348", true));
            countries.Add(new WorldCountry("Iceland", "IS", "ISL", "352", true));
            countries.Add(new WorldCountry("India", "IN", "IND", "356", true));
            countries.Add(new WorldCountry("Indonesia", "ID", "IDN", "360", true));
            countries.Add(new WorldCountry("Iran, Islamic Republic of", "IR", "IRN", "364", true));
            countries.Add(new WorldCountry("Iraq", "IQ", "IRQ", "368", true));
            countries.Add(new WorldCountry("Ireland", "IE", "IRL", "372", true));
            countries.Add(new WorldCountry("Isle of Man", "IM", "IMN", "833", true));
            countries.Add(new WorldCountry("Israel", "IL", "ISR", "376", true));
            countries.Add(new WorldCountry("Italy", "IT", "ITA", "380", true));
            countries.Add(new WorldCountry("Jamaica", "JM", "JAM", "388", true));
            countries.Add(new WorldCountry("Japan", "JP", "JPN", "392", true));
            countries.Add(new WorldCountry("Jersey", "JE", "JEY", "832", true));
            countries.Add(new WorldCountry("Jordan", "JO", "JOR", "400", true));
            countries.Add(new WorldCountry("Kazakhstan", "KZ", "KAZ", "398", true));
            countries.Add(new WorldCountry("Kenya", "KE", "KEN", "404", true));
            countries.Add(new WorldCountry("Kiribati", "KI", "KIR", "296", true));
            countries.Add(new WorldCountry("Korea, Democratic Admins's Republic of", "KP", "PRK", "408", true));
            countries.Add(new WorldCountry("Korea, Republic of", "KR", "KOR", "410", true));
            countries.Add(new WorldCountry("Kuwait", "KW", "KWT", "414", true));
            countries.Add(new WorldCountry("Kyrgyzstan", "KG", "KGZ", "417", true));
            countries.Add(new WorldCountry("Lao Admins's Democratic Republic", "LA", "LAO", "418", true));
            countries.Add(new WorldCountry("Latvia", "LV", "LVA", "428", true));
            countries.Add(new WorldCountry("Lebanon", "LB", "LBN", "422", true));
            countries.Add(new WorldCountry("Lesotho", "LS", "LSO", "426", true));
            countries.Add(new WorldCountry("Liberia", "LR", "LBR", "430", true));
            countries.Add(new WorldCountry("Libya", "LY", "LBY", "434", true));
            countries.Add(new WorldCountry("Liechtenstein", "LI", "LIE", "438", true));
            countries.Add(new WorldCountry("Lithuania", "LT", "LTU", "440", true));
            countries.Add(new WorldCountry("Luxembourg", "LU", "LUX", "442", true));
            countries.Add(new WorldCountry("Macao", "MO", "MAC", "446", true));
            countries.Add(new WorldCountry("Macedonia, the former Yugoslav Republic of", "MK", "MKD", "807", true));
            countries.Add(new WorldCountry("Madagascar", "MG", "MDG", "450", true));
            countries.Add(new WorldCountry("Malawi", "MW", "MWI", "454", true));
            countries.Add(new WorldCountry("Malaysia", "MY", "MYS", "458", true));
            countries.Add(new WorldCountry("Maldives", "MV", "MDV", "462", true));
            countries.Add(new WorldCountry("Mali", "ML", "MLI", "466", true));
            countries.Add(new WorldCountry("Malta", "MT", "MLT", "470", true));
            countries.Add(new WorldCountry("Marshall Islands", "MH", "MHL", "584", true));
            countries.Add(new WorldCountry("Martinique", "MQ", "MTQ", "474", true));
            countries.Add(new WorldCountry("Mauritania", "MR", "MRT", "478", true));
            countries.Add(new WorldCountry("Mauritius", "MU", "MUS", "480", true));
            countries.Add(new WorldCountry("Mayotte", "YT", "MYT", "175", true));
            countries.Add(new WorldCountry("Mexico", "MX", "MEX", "484", true));
            countries.Add(new WorldCountry("Micronesia, Federated States of", "FM", "FSM", "583", true));
            countries.Add(new WorldCountry("Moldova, Republic of", "MD", "MDA", "498", true));
            countries.Add(new WorldCountry("Monaco", "MC", "MCO", "492", true));
            countries.Add(new WorldCountry("Mongolia", "MN", "MNG", "496", true));
            countries.Add(new WorldCountry("Montenegro", "ME", "MNE", "499", true));
            countries.Add(new WorldCountry("Montserrat", "MS", "MSR", "500", true));
            countries.Add(new WorldCountry("Morocco", "MA", "MAR", "504", true));
            countries.Add(new WorldCountry("Mozambique", "MZ", "MOZ", "508", true));
            countries.Add(new WorldCountry("Myanmar", "MM", "MMR", "104", true));
            countries.Add(new WorldCountry("Namibia", "NA", "NAM", "516", true));
            countries.Add(new WorldCountry("Nauru", "NR", "NRU", "520", true));
            countries.Add(new WorldCountry("Nepal", "NP", "NPL", "524", true));
            countries.Add(new WorldCountry("Netherlands", "NL", "NLD", "528", true));
            countries.Add(new WorldCountry("New Caledonia", "NC", "NCL", "540", true));
            countries.Add(new WorldCountry("New Zealand", "NZ", "NZL", "554", true));
            countries.Add(new WorldCountry("Nicaragua", "NI", "NIC", "558", true));
            countries.Add(new WorldCountry("Niger", "NE", "NER", "562", true));
            countries.Add(new WorldCountry("Nigeria", "NG", "NGA", "566", true));
            countries.Add(new WorldCountry("Niue", "NU", "NIU", "570", true));
            countries.Add(new WorldCountry("Norfolk Island", "NF", "NFK", "574", true));
            countries.Add(new WorldCountry("Northern Mariana Islands", "MP", "MNP", "580", true));
            countries.Add(new WorldCountry("Norway", "NO", "NOR", "578", true));
            countries.Add(new WorldCountry("Oman", "OM", "OMN", "512", true));
            countries.Add(new WorldCountry("Pakistan", "PK", "PAK", "586", true));
            countries.Add(new WorldCountry("Palau", "PW", "PLW", "585", true));
            countries.Add(new WorldCountry("Palestine, State of", "PS", "PSE", "275", true));
            countries.Add(new WorldCountry("Panama", "PA", "PAN", "591", true));
            countries.Add(new WorldCountry("Papua New Guinea", "PG", "PNG", "598", true));
            countries.Add(new WorldCountry("Paraguay", "PY", "PRY", "600", true));
            countries.Add(new WorldCountry("Peru", "PE", "PER", "604", true));
            countries.Add(new WorldCountry("Philippines", "PH", "PHL", "608", true));
            countries.Add(new WorldCountry("Pitcairn", "PN", "PCN", "612", true));
            countries.Add(new WorldCountry("Poland", "PL", "POL", "616", true));
            countries.Add(new WorldCountry("Portugal", "PT", "PRT", "620", true));
            countries.Add(new WorldCountry("Puerto Rico", "PR", "PRI", "630", true));
            countries.Add(new WorldCountry("Qatar", "QA", "QAT", "634", true));
            countries.Add(new WorldCountry("Reunion", "RE", "REU", "638", true));
            countries.Add(new WorldCountry("Romania", "RO", "ROU", "642", true));
            countries.Add(new WorldCountry("Russian Federation", "RU", "RUS", "643", true));
            countries.Add(new WorldCountry("Rwanda", "RW", "RWA", "646", true));
            countries.Add(new WorldCountry("Saint BarthÃ©lemy", "BL", "BLM", "652", true));
            countries.Add(new WorldCountry("Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", "654", true));
            countries.Add(new WorldCountry("Saint Kitts and Nevis", "KN", "KNA", "659", true));
            countries.Add(new WorldCountry("Saint Lucia", "LC", "LCA", "662", true));
            countries.Add(new WorldCountry("Saint Martin (French part)", "MF", "MAF", "663", true));
            countries.Add(new WorldCountry("Saint Pierre and Miquelon", "PM", "SPM", "666", true));
            countries.Add(new WorldCountry("Saint Vincent and the Grenadines", "VC", "VCT", "670", true));
            countries.Add(new WorldCountry("Samoa", "WS", "WSM", "882", true));
            countries.Add(new WorldCountry("San Marino", "SM", "SMR", "674", true));
            countries.Add(new WorldCountry("Sao Tome and Principe", "ST", "STP", "678", true));
            countries.Add(new WorldCountry("Saudi Arabia", "SA", "SAU", "682", true));
            countries.Add(new WorldCountry("Senegal", "SN", "SEN", "686", true));
            countries.Add(new WorldCountry("Serbia", "RS", "SRB", "688", true));
            countries.Add(new WorldCountry("Seychelles", "SC", "SYC", "690", true));
            countries.Add(new WorldCountry("Sierra Leone", "SL", "SLE", "694", true));
            countries.Add(new WorldCountry("Singapore", "SG", "SGP", "702", true));
            countries.Add(new WorldCountry("Sint Maarten (Dutch part)", "SX", "SXM", "534", true));
            countries.Add(new WorldCountry("Slovakia", "SK", "SVK", "703", true));
            countries.Add(new WorldCountry("Slovenia", "SI", "SVN", "705", true));
            countries.Add(new WorldCountry("Solomon Islands", "SB", "SLB", "090", true));
            countries.Add(new WorldCountry("Somalia", "SO", "SOM", "706", true));
            countries.Add(new WorldCountry("South Africa", "ZA", "ZAF", "710", true));
            countries.Add(new WorldCountry("South Georgia and the South Sandwich Islands", "GS", "SGS", "239", true));
            countries.Add(new WorldCountry("South Sudan", "SS", "SSD", "728", true));
            countries.Add(new WorldCountry("Spain", "ES", "ESP", "724", true));
            countries.Add(new WorldCountry("Sri Lanka", "LK", "LKA", "144", true));
            countries.Add(new WorldCountry("Sudan", "SD", "SDN", "729", true));
            countries.Add(new WorldCountry("Suriname", "SR", "SUR", "740", true));
            countries.Add(new WorldCountry("Svalbard and Jan Mayen", "SJ", "SJM", "744", true));
            countries.Add(new WorldCountry("Swaziland", "SZ", "SWZ", "748", true));
            countries.Add(new WorldCountry("Sweden", "SE", "SWE", "752", true));
            countries.Add(new WorldCountry("Switzerland", "CH", "CHE", "756", true));
            countries.Add(new WorldCountry("Syrian Arab Republic", "SY", "SYR", "760", true));
            countries.Add(new WorldCountry("Taiwan, Province of China", "TW", "TWN", "158", true));
            countries.Add(new WorldCountry("Tajikistan", "TJ", "TJK", "762", true));
            countries.Add(new WorldCountry("Tanzania, United Republic of", "TZ", "TZA", "834", true));
            countries.Add(new WorldCountry("Thailand", "TH", "THA", "764", true));
            countries.Add(new WorldCountry("Timor-Leste", "TL", "TLS", "626", true));
            countries.Add(new WorldCountry("Togo", "TG", "TGO", "768", true));
            countries.Add(new WorldCountry("Tokelau", "TK", "TKL", "772", true));
            countries.Add(new WorldCountry("Tonga", "TO", "TON", "776", true));
            countries.Add(new WorldCountry("Trinidad and Tobago", "TT", "TTO", "780", true));
            countries.Add(new WorldCountry("Tunisia", "TN", "TUN", "788", true));
            countries.Add(new WorldCountry("Turkey", "TR", "TUR", "792", true));
            countries.Add(new WorldCountry("Turkmenistan", "TM", "TKM", "795", true));
            countries.Add(new WorldCountry("Turks and Caicos Islands", "TC", "TCA", "796", true));
            countries.Add(new WorldCountry("Tuvalu", "TV", "TUV", "798", true));
            countries.Add(new WorldCountry("Uganda", "UG", "UGA", "800", true));
            countries.Add(new WorldCountry("Ukraine", "UA", "UKR", "804", true));
            countries.Add(new WorldCountry("United Arab Emirates", "AE", "ARE", "784", true));
            countries.Add(new WorldCountry("United Kingdom", "GB", "GBR", "826", true));
            countries.Add(new WorldCountry("United States", "US", "USA", "840", true));
            countries.Add(new WorldCountry("United States Minor Outlying Islands", "UM", "UMI", "581", true));
            countries.Add(new WorldCountry("Uruguay", "UY", "URY", "858", true));
            countries.Add(new WorldCountry("Uzbekistan", "UZ", "UZB", "860", true));
            countries.Add(new WorldCountry("Vanuatu", "VU", "VUT", "548", true));
            countries.Add(new WorldCountry("Venezuela, Bolivarian Republic of", "VE", "VEN", "862", true));
            countries.Add(new WorldCountry("Viet Nam", "VN", "VNM", "704", true));
            countries.Add(new WorldCountry("Virgin Islands, British", "VG", "VGB", "092", true));
            countries.Add(new WorldCountry("Virgin Islands, U.S.", "VI", "VIR", "850", true));
            countries.Add(new WorldCountry("Wallis and Futuna", "WF", "WLF", "876", true));
            countries.Add(new WorldCountry("Western Sahara", "EH", "ESH", "732", true));
            countries.Add(new WorldCountry("Yemen", "YE", "YEM", "887", true));
            countries.Add(new WorldCountry("Zambia", "ZM", "ZMB", "894", true));
            countries.Add(new WorldCountry("Zimbabwe", "ZW", "ZWE", "716", true));
        }

        /// <summary>
        /// List of 3 digit abbreviated country codes
        /// </summary>
        /// <returns></returns>
        public string[] Alpha3Codes()
        {
            var abbrevList = new List<string>(countries.Count);
            foreach (var country in countries)
            {
                if (country.Enabled)
                    abbrevList.Add(country.Alpha3Code);
            }
            return abbrevList.ToArray();
        }

        /// <summary>
        /// List of 2 digit abbreviated country codes
        /// </summary>
        /// <returns></returns>
        public string[] Alpha2Codes()
        {
            var abbrevList = new List<string>(countries.Count);
            foreach (var country in countries)
            {
                if (country.Enabled)
                    abbrevList.Add(country.Alpha2Code);
            }
            return abbrevList.ToArray();
        }

        /// <summary>
        /// List of Country names
        /// </summary>
        /// <returns></returns>
        public string[] Names()
        {
            var nameList = new List<string>(countries.Count);
            foreach (var country in countries)
            {
                if (country.Enabled)
                    nameList.Add(country.Name);
            }
            return nameList.ToArray();
        }

        /// <summary>
        /// List of Countries
        /// </summary>
        /// <returns></returns>
        public WorldCountry[] Countries()
        {
            return countries.Where(c => c.Enabled == true).ToArray();
        }
    }
}
