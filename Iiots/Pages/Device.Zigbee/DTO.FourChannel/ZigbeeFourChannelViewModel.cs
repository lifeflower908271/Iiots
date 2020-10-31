using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using NLE.Device.ADAM;
using NLE.Device.ZigBee;
using NLECloudSDK.Model;
using Stylet;


namespace Iiots.Pages
{
    public class ZigbeeFourChannelViewModel : Screen
    {
        public ZigbeeFourChannelViewModel(string channel)
        {
            Channel = channel;
        }

        public ZigbeeCfgSrc.FourChannelModel Model{ get; set; }
        public String Channel { get; set; }
        public String Value { get; set; }
        public String Date { get; set; }
    }
}