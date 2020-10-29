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
    [ValueConversion(typeof(int), typeof(String))]
    public sealed class ZigbeeSerialNumConverter : IValueConverter
    {
        public static ZigbeeSerialNumConverter Get => Singleton<ZigbeeSerialNumConverter>.GetInstance();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            if (value != null)
            {
                var serialNum = (int)value;
                return serialNum.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse((string)value);
        }
    }
}
