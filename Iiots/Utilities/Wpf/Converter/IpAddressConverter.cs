using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Utilities.Wpf
{
    [ValueConversion(typeof(IPAddress), typeof(String))]
    public sealed class IpAddressConverter : IValueConverter
    {
        public static IpAddressConverter Get => Singleton<IpAddressConverter>.GetInstance();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            if (value != null)
            {
                var ip = (IPAddress)value;
                return ip.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IPAddress.Parse((string)value);
        }
    }
}
