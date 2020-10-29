using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using NLE.Device.ADAM;
using NLE.Device.ZigBee;
using SerialPortHelper.Model;
using Stylet;
using StyletIoC;
using Utilities.Memory;
using Utilities.Wpf.Component;

namespace Iiots.Pages
{
    public class TabZigbeeViewModel : Screen, IDisposable
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;


        #region 数据源
        /// <summary>
        /// IP地址
        /// </summary>
        public IPAddress IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int? Port { get; set; } = null;

        /// <summary>
        /// 数据验证结果（true：不可连接，false：可连接）
        /// </summary>
        public bool ConnError { get; set; } = true;

        /// <summary>
        /// Zigbee的TCP包装实例
        /// </summary>
        public ZigBeeSeriesTcp Zigbee { get; set; }

        public ZigbeeDataViewModel DatSrcTemperatureAndHumidity { get; set; }
        public ZigbeeDataViewModel DatSrcBodyInfrared { get; set; }
        public ZigbeeDataViewModel DatSrcLight { get; set; }
        public ZigbeeDataViewModel DatSrcAirQuality { get; set; }
        public ZigbeeDataViewModel DatSrcCombustibleGas { get; set; }
        public ZigbeeDataViewModel DatSrcFire { get; set; }
        public ZigbeeDataViewModel DatSrcFourChannel { get; set; }

        /// <summary>
        /// 配置视图使能
        /// </summary>
        public bool VIEW_EN { get; set; } = true;

        /// <summary>
        /// 后台任务处理状态
        /// </summary>
        public bool IsLoading => !VIEW_EN;

        /// <summary>
        /// Zigbee采集数据可视化集合
        /// </summary>
        public ObservableCollection<ZigbeeDataViewModel> ZigbeeDatas { get; set; } = null;

        /// <summary>
        /// 双联继电器序列号
        /// </summary>
        public int? DoubleDelaySerialNum { get; set; } = null;

        /// <summary>
        /// 视图中当前选中的双联继电器联号
        /// </summary>
        public ZigbeeCfgSrc.UnitNumModel ZigbeeUnitNum { get; set; }
        #endregion

        public TabZigbeeViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;
            this.DisplayName = @"Zigbee";

        }

        protected override void OnViewLoaded()
        {

        }

        protected override void OnActivate()
        {
            base.OnActivate();


        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }


        public void Dispose()
        {
            Zigbee?.Dispose();
            Zigbee = null;
        }

        protected override void OnClose()
        {
            this.Dispose();
        }

        public void validationError(object sender, ValidationErrorEventArgs e)
        {
            if (ValidationHelper.HasError(sender, e))
                ConnError = true;
            else
                ConnError = false;
        }

        public bool CanConnect
        {
            get
            {
                if (Zigbee != null)
                    return false;

                if (IpAddress == null || Port == null)
                    return false;

                if (String.IsNullOrEmpty(IpAddress.ToString()))
                    return false;

                if (String.IsNullOrEmpty(Port.ToString()))
                    return false;

                if (ConnError)
                    return false;

                return true;
            }
        }
        public void Connect()
        {
            string ip = IpAddress.ToString();
            int port = (int)Port;
            Thread thread = new Thread(() =>
            {
                Zigbee = new ZigBeeSeriesTcp(ip, port);
                VIEW_EN = false;
                bool isSuccess = Zigbee.Connect(ip, port);
                VIEW_EN = true;

                if (ZigbeeDatas == null)
                    ZigbeeDatas = new ObservableCollection<ZigbeeDataViewModel>();
                else
                    ZigbeeDatas.Clear();

                DatSrcTemperatureAndHumidity = new ZigbeeDataViewModel("温湿度", string.Empty, string.Empty);
                DatSrcBodyInfrared = new ZigbeeDataViewModel("人体红外", string.Empty, string.Empty);
                DatSrcLight = new ZigbeeDataViewModel("光照", string.Empty, string.Empty);
                DatSrcAirQuality = new ZigbeeDataViewModel("空气质量", string.Empty, string.Empty);
                DatSrcCombustibleGas = new ZigbeeDataViewModel("可燃气", string.Empty, string.Empty);
                DatSrcFire = new ZigbeeDataViewModel("火焰", string.Empty, string.Empty);
                DatSrcFourChannel = new ZigbeeDataViewModel("四通道模拟", string.Empty, string.Empty);
                ZigbeeDatas.Add(DatSrcTemperatureAndHumidity);
                ZigbeeDatas.Add(DatSrcBodyInfrared);
                ZigbeeDatas.Add(DatSrcLight);
                ZigbeeDatas.Add(DatSrcAirQuality);
                ZigbeeDatas.Add(DatSrcCombustibleGas);
                ZigbeeDatas.Add(DatSrcFire);
                ZigbeeDatas.Add(DatSrcFourChannel);

                Zigbee.DataReceived += Zigbee_DataReceived;

                Execute.OnUIThreadSync(() =>
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    if (isSuccess)
                    {
                        _windowManager.ShowMessageBox("连接成功！", "提示");
                    }
                    else
                    {
                        _windowManager.ShowMessageBox("连接失败！", "提示");
                    }
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void Zigbee_DataReceived(object sender, ZigBeeDataEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Execute.OnUIThreadSync(() =>
                {
                    var data = e.Data;
                    switch (data.Type)
                    {
                        case SensorType.TemperatureAndHumidity:
                            DatSrcTemperatureAndHumidity.Value = $@"{data.Value1}°C  {data.Value2}%RH";
                            DatSrcTemperatureAndHumidity.Date = DateTime.Now.ToString();
                            break;
                        case SensorType.BodyInfrared:
                            DatSrcBodyInfrared.Value = $@"{data.Value1}";
                            DatSrcBodyInfrared.Date = DateTime.Now.ToString();
                            break;
                        case SensorType.Light:
                            DatSrcLight.Value = $@"{data.Value1}Lux";
                            DatSrcLight.Date = DateTime.Now.ToString();
                            break;
                        case SensorType.AirQuality:
                            DatSrcAirQuality.Value = $@"{data.Value1}";
                            DatSrcAirQuality.Date = DateTime.Now.ToString();
                            break;
                        case SensorType.CombustibleGas:
                            DatSrcCombustibleGas.Value = $@"{data.Value1}PPM";
                            DatSrcCombustibleGas.Date = DateTime.Now.ToString();
                            break;
                        case SensorType.Fire:
                            DatSrcFire.Value = $@"{data.Value1}";
                            DatSrcFire.Date = DateTime.Now.ToString();
                            break;
                        case SensorType.FourChannel:
                            DatSrcFourChannel.Value = $@"V1:{data.Value1}  V2:{data.Value2}  V3:{data.Value3}  V4:{data.Value4}";
                            DatSrcFourChannel.Date = DateTime.Now.ToString();
                            break;
                        default:
                            break;
                    }
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public bool CanDisConnect
        {
            get
            {
                if (Zigbee == null)
                    return false;

                if (CanConnect)
                    return false;

                return true;
            }
        }
        public void DisConnect()
        {
            Thread thread = new Thread(() =>
            {
                Zigbee.DataReceived -= Zigbee_DataReceived;
                Zigbee.Close();
                Execute.OnUIThreadSync(() =>
                {
                    Zigbee = null;
                    ZigbeeDatas?.Clear();
                    ZigbeeDatas = null;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    _windowManager.ShowMessageBox("断开成功！", "提示");
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }


        public bool CanDoubleDelayOn => CanDisConnect;
        public void DoubleDelayOn()
        {
            Thread thread = new Thread(() =>
            {
                if(DoubleDelaySerialNum != null)
                {
                    var serialNum = (int)DoubleDelaySerialNum;
                    Zigbee.DoubleRelay(serialNum, ZigbeeUnitNum.Value, Relays.On);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public bool CanDoubleDelayOff => CanDisConnect;
        public void DoubleDelayOff()
        {
            Thread thread = new Thread(() =>
            {
                if (DoubleDelaySerialNum != null)
                {
                    var serialNum = (int)DoubleDelaySerialNum;
                    Zigbee.DoubleRelay(serialNum, ZigbeeUnitNum.Value, Relays.Off);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
