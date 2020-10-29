using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using NLE.Device.ZigBee;
using Utilities;

namespace NLE.Device.ZigBee
{

    public sealed class ZigbeeCfgSrc
    {
        public static UnitNumConverter UnitNumTo => Singleton<UnitNumConverter>.GetInstance();

        #region 双联继电器联号
        public sealed class UnitNumModel
        {

            public int Value { get; set; }

            public override string ToString()
            {
                switch (Value)
                {
                    case 1:
                        return "第一联";
                    case 2:
                        return "第二联";
                    default:
                        return null;
                }
            }
        }

        [ValueConversion(typeof(UnitNumModel), typeof(String))]
        public sealed class UnitNumConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((UnitNumModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private static ObservableCollection<UnitNumModel> _obsUnitNum;

        public static ObservableCollection<UnitNumModel> ObsUnitNum
        {
            get
            {
                if (_obsUnitNum == null)
                {
                    Interlocked.CompareExchange(ref _obsUnitNum, new ObservableCollection<UnitNumModel>(), null);
                }
                return _obsUnitNum;
            }
        }

        private static void ObsUnitNumInitialize()
        {
            ObsUnitNum.Add(new UnitNumModel() { Value = 1});
            ObsUnitNum.Add(new UnitNumModel() { Value = 2});
        }
        #endregion


        static ZigbeeCfgSrc()
        {
            ObsUnitNumInitialize();
        }
    }
}
