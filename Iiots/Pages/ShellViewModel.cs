using System;
using Stylet;
using StyletIoC;

namespace Iiots.Pages
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;

        public ShellViewModel(IWindowManager windowManager, IContainer container,
            TabAdam4150ViewModel vmAdam4150, TabLedViewModel vmLed, TabZigbeeViewModel vmZigbee, TabUHFViewModel vmUhf)
        {
            _windowManager = windowManager;
            _container = container;

   

            this.Items.Add(vmAdam4150);
            this.Items.Add(vmLed);
            this.Items.Add(vmZigbee);
            this.Items.Add(vmUhf);
            this.ActiveItem = vmAdam4150;
        }

        protected override void OnViewLoaded()
        {
            
        }
    }
}
