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
    public class ZigbeeSerialNumValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                var str_port = (string)value;
                int port;


                if (String.IsNullOrEmpty(str_port))
                    return new ValidationResult(false, "序列号不可为空");

                if(!XRegexp.IsNumber(str_port))
                    return new ValidationResult(false, "序列号格式不正确");

                if(!int.TryParse(str_port,out port))
                    return new ValidationResult(false, "序列号无效");

                if(!(port >= 0x0000 && port <= 0xFFFF))
                    return new ValidationResult(false, "序列号需在0xFFFF内");

                return new ValidationResult(true, null);
            }

            if (value == null)
                return new ValidationResult(false, "序列号不可为空");

            return new ValidationResult(true, null);
        }
    }
}
