using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Utilities.Wpf
{
    public class IpAddressValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                var str_ip = (string)value;
                IPAddress ip;


                if (String.IsNullOrEmpty(str_ip))
                    return new ValidationResult(false, "ip地址不可为空");

                if (!XRegexp.IsIpAddress(str_ip, false))
                    return new ValidationResult(false, "ip地址格式不正确");

                if (!IPAddress.TryParse(str_ip, out ip))
                    return new ValidationResult(false, "ip地址不在允许的范围内");

                return new ValidationResult(true, null);
            }

            if (value == null)
                return new ValidationResult(false, "ip地址不可为空");

            return new ValidationResult(true, null);
        }
    }
}
