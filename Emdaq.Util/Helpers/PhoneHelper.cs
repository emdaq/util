using System;
using PhoneNumbers;

namespace Emdaq.Util.Helpers
{
    public class PhoneHelper
    {
        #region singleton

        private static readonly Lazy<PhoneHelper> Singleton = new Lazy<PhoneHelper>(() => new PhoneHelper());
        public static PhoneHelper I { get { return Singleton.Value; } }

        #endregion

        #region constants

        private const string UsRegionCode = "US";
        private const int UsCountryCode = 1;
        
        #endregion

        public PhoneHelper(PhoneNumberUtil util = null, string defaultRegionCode = null)
        {
            _defaultRegionCode = defaultRegionCode ?? UsRegionCode;
            _util = util ?? PhoneNumberUtil.GetInstance();
        }

        private readonly PhoneNumberUtil _util;
        private readonly string _defaultRegionCode;

        private static bool NumbersMatch(string input1, string input2)
        {
            input1 = input1 ?? string.Empty;
            input2 = input2 ?? string.Empty;
            return PhoneNumberUtil.NormalizeDigitsOnly(input1) == PhoneNumberUtil.NormalizeDigitsOnly(input2);
        }

        private static bool IsUsPhone(PhoneNumber phone)
        {
            return phone != null && phone.CountryCode == UsCountryCode;
        }

        private PhoneNumber ParsePhone(string phone)
        {
            try
            {
                return _util.Parse(phone, _defaultRegionCode);
            }
            catch
            {
                return null;
            }
        }

        public bool IsValidPhone(string phone)
        {
            return ParsePhone(phone) != null;
        }

        public bool IsUsPhone(string phone)
        {
            var parsedPhone = ParsePhone(phone);
            return IsUsPhone(parsedPhone);
        }

        public bool ArePhonesEquivalent(string phone1, string phone2)
        {
            phone1 = phone1 ?? string.Empty;
            phone2 = phone2 ?? string.Empty;

            var parsedPhone1 = ParsePhone(phone1);
            var parsedPhone2 = ParsePhone(phone2);

            if (parsedPhone1 == null || parsedPhone2 == null)
            {
                return NumbersMatch(phone1, phone2);
            }

            var match = _util.IsNumberMatch(parsedPhone1, parsedPhone2);
            return match != PhoneNumberUtil.MatchType.NO_MATCH;
        }


        /// <summary>
        /// Format US: "(603) 303-7377 x1234", non-US: "+1 603-303-7377 x1234"
        /// </summary>
        public string FormatForDisplay(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return string.Empty;
            }

            var parsedPhone = ParsePhone(phone);

            if (parsedPhone == null)
            {
                return phone;
            }

            if (IsUsPhone(parsedPhone))
            {
                var tel = _util.Format(parsedPhone, PhoneNumberFormat.NATIONAL);

                return parsedPhone.HasExtension
                           ? tel.Replace("ext. " + parsedPhone.Extension, "x" + parsedPhone.Extension)
                           : tel;
            }
            else
            {
                var tel = _util.Format(parsedPhone, PhoneNumberFormat.INTERNATIONAL);

                return parsedPhone.HasExtension
                           ? tel.Replace("ext. " + parsedPhone.Extension, "x" + parsedPhone.Extension)
                           : tel;
            }
        }

        /// <summary>
        /// Format: "+16033037377x1234"
        /// </summary>
        public string FormatForDatabase(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }

            var parsedPhone = ParsePhone(phone);

            if (parsedPhone == null)
            {
                throw new NumberParseException(ErrorType.NOT_A_NUMBER, "Invalid number, cannot format for storage.");
            }

            var tel = _util.Format(parsedPhone, PhoneNumberFormat.E164);

            return parsedPhone.HasExtension 
                       ? tel + "x" + parsedPhone.Extension 
                       : tel;
        }

        // ********************
        // PHONE NUMBER FORMATS
        // ********************
        // E164            +16033037377 
        // NATIONAL        (603) 303-7377  ext. 1234
        // INTERNATIONAL   +1 603-303-7377 ext. 1234
        // RFC3966         tel:+1-603-303-7377;ext=1234
    }
}
