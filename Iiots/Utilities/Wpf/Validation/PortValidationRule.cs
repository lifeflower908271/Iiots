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
    public class PortValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                var str_port = (string)value;
                int port;


                if (String.IsNullOrEmpty(str_port))
                    return new ValidationResult(false, "端口号不可为空");

                if(!XRegexp.IsNumber(str_port))
                    return new ValidationResult(false, "端口号格式不正确");

                if(!int.TryParse(str_port,out port))
                    return new ValidationResult(false, "端口号无效");

                return new ValidationResult(true, null);
            }

            if (value == null)
                return new ValidationResult(false, "端口号不可为空");

            return new ValidationResult(true, null);
        }
    }
}
