using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using SerialPortHelper.Model;
using Stylet;
using StyletIoC;
using Utilities.Memory;

namespace Iiots.Pages
{
    public class TabAdam4150SeriesViewModel : Screen, IDisposable
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;

        #region 数据源
        /// <summary>
        /// 视图中当前选中的串口配置_波特率Model
        /// </summary>
        public SerialPortConfigurationSource.BaudRateModel CmbBaudRate { get; set; }

        /// <summary>
        /// 该源被绑定到串口集合视图
        /// </summary>
        public ObservableCollection<string> DataSourcePortNames { get; set; } = new ObservableCollection<string>(SerialPort.GetPortNames().ToList());

        /// <summary>
        /// 视图中当前选中的串口配置_串口名称
        /// </summary>
        public string CmbPortNames { get; set; }
        #endregion

        public TabAdam4150SeriesViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;
            this.DisplayName = @"串口模式";
        }

        protected override void OnViewLoaded()
        {
            HwndSource hwndSource = PresentationSource.FromVisual(this.View) as HwndSource; // 获取窗口过程
            if (hwndSource != null)
                hwndSource.AddHook(new HwndSourceHook(WndProc)); // 为窗口过程添加挂钩
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

        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WndMsg.WM_DEVICECHANGE) // 检测到设备发生变更
            {
                var portNames = SerialPort.GetPortNames();
                switch (wParam.ToInt32())
                {

                    case WndMsg.DBT_DEVICEARRIVAL: // 设备插入
                        foreach (var portName in portNames)
                        {
                            if (!DataSourcePortNames.Contains<string>(portName))
                                DataSourcePortNames.Add(portName);
                        }
                        handled = true;
                        break;

                    case WndMsg.DBT_DEVICEREMOVECOMPLETE: // 设备拔出
                        foreach (var portName in DataSourcePortNames.ToArray())
                        {
                            if (!portNames.Contains<string>(portName))
                                DataSourcePortNames.Remove(portName);
                        }
                        handled = true;
                        break;
                    default:
                        break;
                }
            }
            return IntPtr.Zero;
        }
    }
}
