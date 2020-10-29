using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using NLE.Device.ADAM;
using NLECloudSDK.Model;
using Stylet;


namespace Iiots.Pages
{
    public class AdamDataViewModel : Screen
    {
        public String DINumber { get; set; }
        public String Value { get; set; }

        public AdamDataViewModel(string dINumber, string value)
        {
            DINumber = dINumber;
            Value = value;
        }
    }
}