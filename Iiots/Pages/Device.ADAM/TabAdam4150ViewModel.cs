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
using SerialPortHelper.Model;
using Stylet;
using StyletIoC;
using Utilities.Memory;
using Utilities.Wpf.Component;

namespace Iiots.Pages
{
    public class TabAdam4150ViewModel : Screen, IDisposable
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;

        private System.Timers.Timer _dataTimer;
        public System.Timers.Timer DataTimer
        {
            get
            {
                if (_dataTimer == null)
                {
                    Interlocked.CompareExchange(ref _dataTimer, new System.Timers.Timer(500), null);
                    _dataTimer.Elapsed += (sender, e) =>
                    {
                        ADAM4150?.ReadADAM4150Data();
                    };
                }
                return _dataTimer;
            }
        }

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
        /// ADAM4150的TCP包装实例
        /// </summary>
        public ADAMSeriesTcp ADAM4150 { get; set; }

        public AdamDataViewModel DatSrcDI0 { get; set; }
        public AdamDataViewModel DatSrcDI1 { get; set; }
        public AdamDataViewModel DatSrcDI2 { get; set; }
        public AdamDataViewModel DatSrcDI3 { get; set; }
        public AdamDataViewModel DatSrcDI4 { get; set; }
        public AdamDataViewModel DatSrcDI5 { get; set; }
        public AdamDataViewModel DatSrcDI6 { get; set; }

        /// <summary>
        /// 配置视图使能
        /// </summary>
        public bool VIEW_EN { get; set; } = true;

        /// <summary>
        /// 后台任务处理状态
        /// </summary>
        public bool IsLoading => !VIEW_EN;

        /// <summary>
        /// adam开关可视化集合
        /// </summary>
        public ObservableCollection<Adam4150TcpSwitchViewModel> AdamSwitchs { get; set; } = null;


        /// <summary>
        /// adam采集数据可视化集合
        /// </summary>
        public ObservableCollection<AdamDataViewModel> AdamDatas { get; set; } = null;
        #endregion

        public TabAdam4150ViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;
            this.DisplayName = @"ADAM4150";

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
            ADAM4150?.Dispose();
            ADAM4150 = null;
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
                if (ADAM4150 != null)
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
                ADAM4150 = new ADAMSeriesTcp(ip, port);
                VIEW_EN = false;
                bool isSuccess = ADAM4150.Connect(ip, port);
                VIEW_EN = true;
                if (AdamSwitchs == null)
                {
                    AdamSwitchs = new ObservableCollection<Adam4150TcpSwitchViewModel>();
                }
                else
                {
                    AdamSwitchs.Clear();
                }

                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                    "DO0",
                    new WeakReference<ADAMSeriesTcp>(ADAM4150),
                    Switchs.OnDO0,
                    Switchs.OffDO0
                 ));
                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                     "DO1",
                     new WeakReference<ADAMSeriesTcp>(ADAM4150),
                     Switchs.OnDO1,
                     Switchs.OffDO1
                  ));
                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                     "DO2",
                     new WeakReference<ADAMSeriesTcp>(ADAM4150),
                     Switchs.OnDO2,
                     Switchs.OffDO2
                  ));
                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                     "DO3",
                     new WeakReference<ADAMSeriesTcp>(ADAM4150),
                     Switchs.OnDO3,
                     Switchs.OffDO3
                  ));
                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                     "DO4",
                     new WeakReference<ADAMSeriesTcp>(ADAM4150),
                     Switchs.OnDO4,
                     Switchs.OffDO4
                  ));
                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                     "DO5",
                     new WeakReference<ADAMSeriesTcp>(ADAM4150),
                     Switchs.OnDO5,
                     Switchs.OffDO5
                  ));
                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                     "DO6",
                     new WeakReference<ADAMSeriesTcp>(ADAM4150),
                     Switchs.OnDO6,
                     Switchs.OffDO6
                  ));
                AdamSwitchs.Add(new Adam4150TcpSwitchViewModel(
                     "DO7",
                     new WeakReference<ADAMSeriesTcp>(ADAM4150),
                     Switchs.OnDO7,
                     Switchs.OffDO7
                  ));

                if (AdamDatas == null)
                    AdamDatas = new ObservableCollection<AdamDataViewModel>();
                else
                    AdamDatas.Clear();

                DatSrcDI0 = new AdamDataViewModel("DI0", String.Empty);
                DatSrcDI1 = new AdamDataViewModel("DI1", String.Empty);
                DatSrcDI2 = new AdamDataViewModel("DI2", String.Empty);
                DatSrcDI3 = new AdamDataViewModel("DI3", String.Empty);
                DatSrcDI4 = new AdamDataViewModel("DI4", String.Empty);
                DatSrcDI5 = new AdamDataViewModel("DI5", String.Empty);
                DatSrcDI6 = new AdamDataViewModel("DI6", String.Empty);
                AdamDatas.Add(DatSrcDI0);
                AdamDatas.Add(DatSrcDI1);
                AdamDatas.Add(DatSrcDI2);
                AdamDatas.Add(DatSrcDI3);
                AdamDatas.Add(DatSrcDI4);
                AdamDatas.Add(DatSrcDI5);
                AdamDatas.Add(DatSrcDI6);

                DataTimer.Start();
                ADAM4150.ADAM4150DataReceived += ADAM4150_ADAM4150DataReceived;
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


        public bool CanDisConnect
        {
            get
            {
                if (ADAM4150 == null)
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
                DataTimer.Stop();
                ADAM4150.ADAM4150DataReceived -= ADAM4150_ADAM4150DataReceived;
                ADAM4150.Close();
                Execute.OnUIThreadSync(() =>
                {
                    ADAM4150 = null;
                    AdamSwitchs?.Clear();
                    AdamSwitchs = null;
                    AdamDatas?.Clear();
                    AdamDatas = null;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    _windowManager.ShowMessageBox("断开成功！", "提示");
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void ADAM4150_ADAM4150DataReceived(object sender, ADAM4150EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Execute.OnUIThreadSync(() =>
                {
                    DatSrcDI0.Value = e.Data.DI0 ? "1" : "0";
                    DatSrcDI1.Value = e.Data.DI1 ? "1" : "0";
                    DatSrcDI2.Value = e.Data.DI2 ? "1" : "0";
                    DatSrcDI3.Value = e.Data.DI3 ? "1" : "0";
                    DatSrcDI4.Value = e.Data.DI4 ? "1" : "0";
                    DatSrcDI5.Value = e.Data.DI5 ? "1" : "0";
                    DatSrcDI6.Value = e.Data.DI6 ? "1" : "0";
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
