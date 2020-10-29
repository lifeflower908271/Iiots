using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.Wpf.Component
{
    public static class ValidationHelper
    {
        public static bool HasError(object sender, ValidationErrorEventArgs e)
        {
            var obj = (DependencyObject)sender;
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                var element = child as FrameworkElement;
                if (element == null) continue;
                if (Validation.GetHasError(element))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
