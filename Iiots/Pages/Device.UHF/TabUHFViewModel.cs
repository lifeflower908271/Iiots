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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;

using NLE.Device.ADAM;
using NLE.Device.UHF;
using NLE.Device.ZigBee;
using SerialPortHelper.Model;
using Stylet;
using StyletIoC;
using Utilities.Memory;
using Utilities.Wpf.Component;

namespace Iiots.Pages
{
    public class TabUHFViewModel : Screen, IDisposable
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
        public UHFReaderTcp Uhf { get; set; }

        /// <summary>
        /// 视图中反馈Epc卡号
        /// </summary>
        public String EpcStr { get; set; }

        /// <summary>
        /// 配置视图使能
        /// </summary>
        public bool VIEW_EN { get; set; } = true;

        /// <summary>
        /// 后台任务处理状态
        /// </summary>
        public bool IsLoading => !VIEW_EN;

        #endregion

        public TabUHFViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;
            this.DisplayName = @"UHF";

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
            Uhf?.Dispose();
            Uhf = null;
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
                if (Uhf != null)
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
                Uhf = new UHFReaderTcp(ip, port);
                VIEW_EN = false;
                bool isSuccess = Uhf.Connect(ip, port);
                VIEW_EN = true;

                Uhf.DataReceived += Uhf_DataReceived; ;

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

        private void Uhf_DataReceived(object sender, UHFDataEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Execute.OnUIThreadSync(() =>
                {


                });
            });
            thread.IsBackground = true;
            thread.Start();
        }



        public bool CanDisConnect
        {
            get
            {
                if (Uhf == null)
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
                Uhf.DataReceived -= Uhf_DataReceived;
                Uhf.Close();
                Execute.OnUIThreadSync(() =>
                {
                    Uhf = null;

                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    _windowManager.ShowMessageBox("断开成功！", "提示");
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public bool CanReadEpc => CanDisConnect;
        public void ReadEpc()
        {
            Thread thread = new Thread(() =>
            {
                EpcStr = string.Empty;
                var list = Uhf.ReadEpcSection();
                if (null == list)
                    return;

                foreach (var epc in list)
                {
                    EpcStr += epc + "\r\n";
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

    }
}
