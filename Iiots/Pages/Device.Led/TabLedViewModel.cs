using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using NLE.Device.Led;
using SerialPortHelper.Model;
using Stylet;
using StyletIoC;
using Utilities.Memory;
using Utilities.Wpf.Component;

namespace Iiots.Pages
{
    public class TabLedViewModel : Screen, IDisposable
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
        /// Led的TCP包装实例
        /// </summary>
        public LedDriverTcp Led { get; set; }

        /// <summary>
        /// 视图中当前选中的Led_播放模式
        /// </summary>
        public LedCfgSrc.PlayModeModel LedPlayMode { get; set; }

        /// <summary>
        /// 视图中当前选中的Led_播放速度
        /// </summary>
        public LedCfgSrc.SpeedModel LedSpeed { get; set; }

        /// <summary>
        /// 视图中当前输入的Led_文本内容
        /// </summary>
        public String LedText { get; set; } = String.Empty;


        /// <summary>
        /// 配置视图使能
        /// </summary>
        public bool VIEW_EN { get; set; } = true;

        /// <summary>
        /// 后台任务处理状态
        /// </summary>
        public bool IsLoading => !VIEW_EN;

        #endregion

        public TabLedViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;
            this.DisplayName = @"Led";
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
            Led?.Dispose();
            Led = null;
        }

        protected override void OnClose()
        {
            this.Dispose();
        }


        public bool CanConnect
        {
            get
            {
                if (Led != null)
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
                Led = new LedDriverTcp(ip, port);
                VIEW_EN = false;
                bool isSuccess = Led.Connect(ip, port);
                VIEW_EN = true;
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
                if (Led == null)
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
                Led.Close();
                Execute.OnUIThreadSync(() =>
                {
                    Led = null;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    _windowManager.ShowMessageBox("断开成功！", "提示");
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public bool CanSendText => CanDisConnect;

        public void SendText()
        {
            Thread thread = new Thread(() =>
            {
                bool isSuccess = Led.Send(LedText ?? String.Empty, LedPlayMode.Value, LedSpeed.Value);
                Execute.OnUIThreadSync(() =>
                {
                    if (isSuccess)
                    {
                        _windowManager.ShowMessageBox("发送成功！", "提示");
                    }
                    else
                    {
                        _windowManager.ShowMessageBox("发送失败！", "提示");
                    }
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public void validationError(object sender, ValidationErrorEventArgs e)
        {
            if (ValidationHelper.HasError(sender, e))
                ConnError = true;
            else
                ConnError = false;
        }
    }
}
