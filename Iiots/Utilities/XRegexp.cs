using System.Text.RegularExpressions;

namespace Utilities
{
    public static class XRegexp
    {
        public static bool IsTelephone(string str_telephone)
        {
            return Regex.IsMatch(str_telephone, @"^(\d{3,4}-)?\d{6,8}$");
        }

        public static bool IsHandset(string str_handset)
        {
            return Regex.IsMatch(str_handset, @"^1[3456789]\d{9}$");
        }

        public static bool IsIDcard(string str_idcard)
        {
            return Regex.IsMatch(str_idcard, @"(^\d{18}$)|(^\d{15}$)");
        }

        public static bool IsNumber(string str_number)
        {
            return Regex.IsMatch(str_number, @"^[0-9]*$");
        }

        public static bool IsPostalcode(string str_postalcode)
        {

            return Regex.IsMatch(str_postalcode, @"^\d{6}$");
        }

        public static bool IsIpAddress(string str_ipAddress, bool strict)
        {
            if (true == strict)
                return Regex.IsMatch(str_ipAddress, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
            return Regex.IsMatch(str_ipAddress, @"^(\d{1,3}\.){3}\d{1,3}$");
        }
    }
}