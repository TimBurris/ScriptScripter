using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScriptScripter.DesktopApp.Controls
{
    /*
     * 
     * This control was inspired by Sam Jack's PasswordAssistant: http://blog.functionalfun.net/2008/06/wpf-passwordbox-and-data-binding.html
     * 
     */
    public class BindablePasswordBox : ContentControl
    {

        public static readonly DependencyProperty BoundPassword =
          DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(BindablePasswordBox), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(BindablePasswordBox), new PropertyMetadata(false));

        public BindablePasswordBox()
        {

            var box = new PasswordBox();
            Content = box;
            Loaded += BindablePasswordBox_Loaded;
            Unloaded += BindablePasswordBox_Unloaded;
            IsTabStop = false;
        }

        private void BindablePasswordBox_Unloaded(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)Content;
            box.PasswordChanged -= HandlePasswordChanged;
        }

        private void BindablePasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)Content;
            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = (PasswordBox)((BindablePasswordBox)d).Content;

            if (!GetUpdatingPassword(d))
            {
                box.Password = (string)e.NewValue;
            }
        }

        private void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            // set a flag to indicate that we're updating the password
            SetUpdatingPassword(this, true);
            // push the new password into the BoundPassword property
            SetBoundPassword(this, box.Password);
            SetUpdatingPassword(this, false);
        }

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
    }
}
