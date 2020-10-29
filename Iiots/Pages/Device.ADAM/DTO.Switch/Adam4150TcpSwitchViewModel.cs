using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using NLE.Device.ADAM;
using NLECloudSDK.Model;
using Stylet;


namespace Iiots.Pages
{
    public class Adam4150TcpSwitchViewModel : Screen
    {
        public readonly WeakReference<ADAMSeriesTcp> @ref;
        public readonly Switchs on;
        public readonly Switchs off;

        public string DONumber { get; set; }

        public Adam4150TcpSwitchViewModel(string dONumber, WeakReference<ADAMSeriesTcp> @ref, Switchs on, Switchs off)
        {
            this.DONumber = dONumber;
            this.@ref = @ref;
            this.on = on;
            this.off = off;
        }

        public void Control(object sender, RoutedEventArgs e)
        {
            var ui = (ToggleButton)sender;
            ADAMSeriesTcp adam;
            Switchs paras;

            if (@ref.TryGetTarget(out adam))                
            {
                if (ui.IsChecked ?? false)
                {
                    paras = on;
                }
                else
                {
                    paras = off;
                }
                Thread thread = new Thread(() =>
                {
                    adam.Switch(paras);
                });
                thread.IsBackground = true;
                thread.Start();
            }

            

        }
    }
}