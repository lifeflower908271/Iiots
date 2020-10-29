using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using Utilities;

namespace SerialPortHelper.Model
{
    public sealed class SerialPortConfigurationSource
    {

        #region 波特率
        public static BaudRateConverter BaudRateTo => Singleton<BaudRateConverter>.GetInstance();
        [ValueConversion(typeof(BaudRateModel), typeof(String))]
        public sealed class BaudRateConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((BaudRateModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public sealed class BaudRateModel
        {

            public int Value { get; set; }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        private static ObservableCollection<BaudRateModel> _baudRate;

        public static ObservableCollection<BaudRateModel> BaudRate
        {
            get
            {
                if (_baudRate == null)
                {
                    Interlocked.CompareExchange(ref _baudRate, new ObservableCollection<BaudRateModel>(), null);
                }
                return _baudRate;
            }
        }

        private static void BaudRateCollectionInitialize()
        {
            BaudRate.Add(new BaudRateModel() { Value = 110 });
            BaudRate.Add(new BaudRateModel() { Value = 300 });
            BaudRate.Add(new BaudRateModel() { Value = 600 });
            BaudRate.Add(new BaudRateModel() { Value = 1200 });
            BaudRate.Add(new BaudRateModel() { Value = 2400 });
            BaudRate.Add(new BaudRateModel() { Value = 4800 });
            BaudRate.Add(new BaudRateModel() { Value = 9600 });
            BaudRate.Add(new BaudRateModel() { Value = 14400 });
            BaudRate.Add(new BaudRateModel() { Value = 19200 });
            BaudRate.Add(new BaudRateModel() { Value = 38400 });
            BaudRate.Add(new BaudRateModel() { Value = 57600 });
            BaudRate.Add(new BaudRateModel() { Value = 115200 });
            BaudRate.Add(new BaudRateModel() { Value = 128000 });
            BaudRate.Add(new BaudRateModel() { Value = 256000 });
        }
        #endregion

        #region 校验位
        public static ParityConverter ParityTo => Singleton<ParityConverter>.GetInstance();
        [ValueConversion(typeof(ParityModel), typeof(String))]
        public sealed class ParityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((ParityModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public sealed class ParityModel
        {

            public Parity Value { get; set; }

            public override string ToString()
            {
                return Enum.GetName(typeof(Parity), Value);
            }
        }

        private static ObservableCollection<ParityModel> _parity;

        public static ObservableCollection<ParityModel> Parity
        {
            get
            {
                if (_parity == null)
                {
                    Interlocked.CompareExchange(ref _parity, new ObservableCollection<ParityModel>(), null);
                }
                return _parity;
            }
        }

        private static void ParityCollectionInitialize()
        {
            Parity.Add(new ParityModel() { Value = System.IO.Ports.Parity.None });
            Parity.Add(new ParityModel() { Value = System.IO.Ports.Parity.Odd });
            Parity.Add(new ParityModel() { Value = System.IO.Ports.Parity.Even });
            Parity.Add(new ParityModel() { Value = System.IO.Ports.Parity.Space });
            Parity.Add(new ParityModel() { Value = System.IO.Ports.Parity.Mark });
        }
        #endregion

        #region 停止位
        public static StopBitsConverter StopBitsTo => Singleton<StopBitsConverter>.GetInstance();
        [ValueConversion(typeof(StopBitsModel), typeof(String))]
        public sealed class StopBitsConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((StopBitsModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public sealed class StopBitsModel
        {

            public StopBits Value { get; set; }

            public override string ToString()
            {
                switch (Value)
                {
                    case System.IO.Ports.StopBits.None:
                        return Enum.GetName(typeof(StopBits), Value);
                    case System.IO.Ports.StopBits.One:
                        return "1";
                    case System.IO.Ports.StopBits.Two:
                        return "2";
                    case System.IO.Ports.StopBits.OnePointFive:
                        return "1.5";
                    default:
                        break;
                }
                return null;
            }
        }

        private static ObservableCollection<StopBitsModel> _stopBits;

        public static ObservableCollection<StopBitsModel> StopBits
        {
            get
            {
                if (_stopBits == null)
                {
                    Interlocked.CompareExchange(ref _stopBits, new ObservableCollection<StopBitsModel>(), null);
                }
                return _stopBits;
            }
        }

        private static void StopBitsCollectionInitialize()
        {
            StopBits.Add(new StopBitsModel() { Value = System.IO.Ports.StopBits.None });
            StopBits.Add(new StopBitsModel() { Value = System.IO.Ports.StopBits.One });
            StopBits.Add(new StopBitsModel() { Value = System.IO.Ports.StopBits.OnePointFive });
            StopBits.Add(new StopBitsModel() { Value = System.IO.Ports.StopBits.Two });
        }
        #endregion

        #region 数据位
        public static DataBitsConverter DataBitsTo => Singleton<DataBitsConverter>.GetInstance();
        [ValueConversion(typeof(DataBitsModel), typeof(String))]
        public sealed class DataBitsConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((DataBitsModel)value).ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public sealed class DataBitsModel
        {

            public int Value { get; set; }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        private static ObservableCollection<DataBitsModel> _dataBits;

        public static ObservableCollection<DataBitsModel> DataBits
        {
            get
            {
                if (_dataBits == null)
                {
                    Interlocked.CompareExchange(ref _dataBits, new ObservableCollection<DataBitsModel>(), null);
                }
                return _dataBits;
            }
        }

        private static void DataBitsCollectionInitialize()
        {
            DataBits.Add(new DataBitsModel() { Value = 5 });
            DataBits.Add(new DataBitsModel() { Value = 6 });
            DataBits.Add(new DataBitsModel() { Value = 7 });
            DataBits.Add(new DataBitsModel() { Value = 8 });
        }
        #endregion

        static SerialPortConfigurationSource()
        {
            BaudRateCollectionInitialize();
            ParityCollectionInitialize();
            StopBitsCollectionInitialize();
            DataBitsCollectionInitialize();

        }
    }
}
