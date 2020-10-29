using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using NLE.Device.ADAM;
using NLECloudSDK.Model;
using Stylet;


namespace Iiots.Pages
{
    public class ZigbeeDataViewModel : Screen
    {
        public String SensorType { get; set; }
        public String Value { get; set; }
        public String Date { get; set; }

        public ZigbeeDataViewModel(string sensorType, string value, string date)
        {
            SensorType = sensorType;
            Value = value;
            Date = date;
        }
    }
}