﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Iiots.Pages
{
    /// <summary>
    /// TabAdam4150.xaml 的交互逻辑
    /// </summary>
    public partial class TabAdam4150SeriesTcpView : UserControl
    {
        public TabAdam4150SeriesTcpView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }
    }
}
