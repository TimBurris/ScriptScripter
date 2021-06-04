using NinjaMvvm.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ScriptScripter.DesktopApp
{
    public class BoundControlBase : System.Windows.Controls.UserControl
    {
        public BoundControlBase()
        {
            // setup a binding that will cause the viewmodel to know when the control is doing it's bindings
            BindingOperations.SetBinding(this, ViewBoundProperty, new Binding(nameof(ViewModels.ScriptScripterViewModelBase.ViewBound)));
            Unloaded += BoundControlBase_Unloaded;
        }

        private void BoundControlBase_Unloaded(object sender, RoutedEventArgs e)
        {
            DataContext = null;
            Unloaded -= BoundControlBase_Unloaded;
        }

        #region ViewBound DP

        public static object GetViewBound(DependencyObject obj)
        {
            return (object)obj.GetValue(ViewBoundProperty);
        }

        public static void SetViewBound(DependencyObject obj, object value)
        {
            obj.SetValue(ViewBoundProperty, value);
        }

        // Using a DependencyProperty as the backing store for ViewBound.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewBoundProperty =
            DependencyProperty.RegisterAttached("ViewBound", typeof(object), typeof(BoundControlBase), new PropertyMetadata(0));

        #endregion
    }
}
