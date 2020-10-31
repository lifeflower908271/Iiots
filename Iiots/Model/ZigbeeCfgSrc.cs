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
        public static FourChannelConverter FourChannelTo => Singleton<FourChannelConverter>.GetInstance();

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
            ObsUnitNum.Add(new UnitNumModel() { Value = 1 });
            ObsUnitNum.Add(new UnitNumModel() { Value = 2 });
        }
        #endregion

        #region 四通道类型
        public sealed class FourChannelModel
        {
            public enum TYPE
            {
                CURRENT_ELECTRIC,
                SOIL_TEMPERATUER,
                WATER_TEMPERATURE,
                SOIL_MOISTURE,
                WATER_LEVEL,
                LIGHT,
                TEMPERATURE,
                HUMIDITY,
                NOICE,
                CO2
            }

            public TYPE Value { get; set; }

            public override string ToString()
            {
                switch (Value)
                {
                    case TYPE.CURRENT_ELECTRIC:
                        return "四通道电流";
                    case TYPE.SOIL_TEMPERATUER:
                        return "土壤温度";
                    case TYPE.WATER_TEMPERATURE:
                        return "水温";
                    case TYPE.SOIL_MOISTURE:
                        return "土壤水分";
                    case TYPE.WATER_LEVEL:
                        return "水位";
                    case TYPE.LIGHT:
                        return "光照";
                    case TYPE.TEMPERATURE:
                        return "温度";
                    case TYPE.HUMIDITY:
                        return "湿度";
                    case TYPE.NOICE:
                        return "噪音";
                    case TYPE.CO2:
                        return "二氧化碳";
                    default:
                        return string.Empty;
                }
            }
        }

        [ValueConversion(typeof(FourChannelModel), typeof(String))]
        public sealed class FourChannelConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((FourChannelModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private static ObservableCollection<FourChannelModel> _obsFourChannel;

        public static ObservableCollection<FourChannelModel> ObsFourChannel
        {
            get
            {
                if (_obsFourChannel == null)
                {
                    Interlocked.CompareExchange(ref _obsFourChannel, new ObservableCollection<FourChannelModel>(), null);
                }
                return _obsFourChannel;
            }
        }

        private static void ObsFourChannelModelInitialize()
        {
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.CURRENT_ELECTRIC });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.SOIL_TEMPERATUER });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.WATER_TEMPERATURE });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.SOIL_MOISTURE });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.WATER_LEVEL });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.LIGHT });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.TEMPERATURE });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.HUMIDITY });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.NOICE });
            ObsFourChannel.Add(new FourChannelModel() { Value = FourChannelModel.TYPE.CO2 });
        }
        #endregion

        static ZigbeeCfgSrc()
        {
            ObsUnitNumInitialize();
            ObsFourChannelModelInitialize();
        }
    }
}
