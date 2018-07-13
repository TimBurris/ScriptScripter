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
    public class BoundViewBase : System.Windows.Controls.UserControl
    {
        public BoundViewBase()
        {
            // setup a binding that will cause the viewmodel to know when the control is doing it's bindings
            BindingOperations.SetBinding(this, ViewBoundProperty, new Binding(nameof(ViewModels.ScriptScripterViewModelBase.ViewBound)));
            Unloaded += BoundViewBase_Unloaded;

            this.Loaded += BoundViewBase_Loaded;
        }

        private void BoundViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BoundViewBase_Loaded;
            if (InitialDialogHeight == 0 && InitialDialogWidth == 0)
                return;

            var w = Window.GetWindow(this) as DialogWindow;
            if (w == null)
                return;

            if (w.DataContext == this.DataContext)
            {

                w.Closing += window_Closing;

                bool setHeight = InitialDialogHeight > 0;
                bool setWidth = InitialDialogWidth > 0;

                if (setHeight)
                    w.Height = InitialDialogHeight;
                if (setWidth)
                    w.Width = InitialDialogWidth;

                if (setHeight && setWidth)
                    w.SizeToContent = SizeToContent.Manual;
                else if (setHeight)
                    w.SizeToContent = SizeToContent.Width;
                else
                    w.SizeToContent = SizeToContent.Height;

                RecenterParentDialog(w);
            }
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ClosingCommand?.Execute(e);
        }

        private void RecenterParentDialog(DialogWindow w)
        {
            //since we just re did the dialog's size, we have to recenter it
            double nonWPFOwnerLeft = w.Owner.Left;
            double nonWPFOwnerWidth = w.Owner.Width;
            double nonWPFOwnerTop = w.Owner.Top;
            double nonWPFOwnerHeight = w.Owner.Height;

            w.Left = nonWPFOwnerLeft + (nonWPFOwnerWidth - w.Width) / 2;

            w.Top = nonWPFOwnerTop + (nonWPFOwnerHeight - w.Height) / 2;
        }

        private void BoundViewBase_Unloaded(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this) as DialogWindow;

            if (w != null && w.DataContext == this.DataContext)
            {
                w.Closing -= window_Closing;
            }

            DataContext = null;
            Unloaded -= BoundViewBase_Unloaded;

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
            DependencyProperty.RegisterAttached("ViewBound", typeof(object), typeof(BoundViewBase), new PropertyMetadata(0));

        #endregion

        #region InitialDialogWidth DP
        public Double InitialDialogWidth
        {
            get { return (Double)GetValue(InitialDialogWidthProperty); }
            set { SetValue(InitialDialogWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitialDialogWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitialDialogWidthProperty =
            DependencyProperty.Register("InitialDialogWidth", typeof(Double), typeof(BoundViewBase));
        #endregion

        #region InitialDialogHeight DP
        public Double InitialDialogHeight
        {
            get { return (Double)GetValue(InitialDialogHeightProperty); }
            set { SetValue(InitialDialogHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitialDialogHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitialDialogHeightProperty =
            DependencyProperty.Register("InitialDialogHeight", typeof(Double), typeof(BoundViewBase));
        #endregion

        #region ClosingCommand DP

        public RelayCommand<System.ComponentModel.CancelEventArgs> ClosingCommand
        {
            get { return (RelayCommand<System.ComponentModel.CancelEventArgs>)GetValue(ClosingCommandProperty); }
            set { SetValue(ClosingCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClosingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClosingCommandProperty =
            DependencyProperty.Register("ClosingCommand", typeof(RelayCommand<System.ComponentModel.CancelEventArgs>), typeof(BoundViewBase));

        #endregion

    }
}
