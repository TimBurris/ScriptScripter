using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ScriptScripter.DesktopApp.Controls
{
    public class EnhancedDataGridControl : System.Windows.Controls.DataGrid, INotifyPropertyChanged
    {
        public static DependencyProperty LoadMoreDataCommandProperty
            = DependencyProperty.Register("LoadMoreDataCommand", typeof(ICommand), typeof(EnhancedDataGridControl));

        public static DependencyProperty LoadMoreDataCommandParameterProperty
            = DependencyProperty.Register("LoadMoreDataCommandParameter", typeof(Object), typeof(EnhancedDataGridControl));

        public static DependencyProperty CanLoadMoreDataProperty
            = DependencyProperty.Register("CanLoadMoreData", typeof(Boolean), typeof(EnhancedDataGridControl));

        public static DependencyProperty IsLoadingDataProperty
            = DependencyProperty.Register("IsLoadingData", typeof(Boolean), typeof(EnhancedDataGridControl));


        private ScrollViewer _scrollViewer;

        public event PropertyChangedEventHandler PropertyChanged;

        public EnhancedDataGridControl()
        {
            this.Style = System.Windows.Application.Current.TryFindResource("EnhancedDataGridControlStyle") as System.Windows.Style;
            var bgbrush = System.Windows.Application.Current.TryFindResource("SelectedItemBackgroundBrush");
            var fgbrush = System.Windows.Application.Current.TryFindResource("SelectedItemForegroundBrush");
            this.Resources.Add(SystemColors.HighlightBrushKey, bgbrush);
            this.Resources.Add(SystemColors.HighlightTextBrushKey, fgbrush);

            this.Resources.Add(SystemColors.InactiveSelectionHighlightBrushKey, bgbrush);
            this.Resources.Add(SystemColors.InactiveSelectionHighlightTextBrushKey, fgbrush);

            this.LoadingRow += EnhancedDataGridControl_LoadingRow;
            this.UnloadingRow += EnhancedDataGridControl_UnloadingRow;
            this.Loaded += EnhancedDataGridControl_Loaded;
            this.Unloaded += EnhancedDataGridControl_Unloaded;

        }

        private void EnhancedDataGridControl_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row == null)
                return;
            e.Row.PreviewMouseDown -= Row_PreviewMouseDown;
        }

        private void EnhancedDataGridControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.DettachScrollViewerEvent();
        }

        private void EnhancedDataGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.LoadMoreDataCommand != null)
                this.AttachScrollViewerEvent();

        }

        private void Row_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //this functionality is to ensure that even if the left/right click is on some "subcontrol" of the row, the row still gets selected 
            //   basically we found this issue when users tried to right click on a row for context menu, but the place they right clicked was on a hyperlink column, so the row did not become selected
            var row = sender as DataGridRow;
            if (row == null)
                return;
            else
                row.IsSelected = true;
        }

        private void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //only if we have a command and the change was a "Scrolldown"
            if (LoadMoreDataCommand != null && e.VerticalChange > 0)
            {
                if (this.CanLoadMoreData && this.HasScrolledToBottom() && LoadMoreDataCommand.CanExecute(this.LoadMoreDataCommandParameter))
                {
                    LoadMoreDataCommand.Execute(this.LoadMoreDataCommandParameter);
                }
            }
        }

        private Boolean HasScrolledToBottom()
        {
            if (_scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                return (_scrollViewer.VerticalOffset == _scrollViewer.ScrollableHeight);
            else
                return false;
        }

        public ICommand LoadMoreDataCommand
        {
            get
            {
                return (ICommand)GetValue(LoadMoreDataCommandProperty);
            }
            set
            {
                if (!object.Equals(this.LoadMoreDataCommand, value))
                {
                    SetValue(LoadMoreDataCommandProperty, value);
                    OnPropertyChanged();
                }
                if (value == null)
                    this.DettachScrollViewerEvent();
                else
                    this.AttachScrollViewerEvent();
            }
        }

        public Object LoadMoreDataCommandParameter
        {
            get
            {
                return GetValue(LoadMoreDataCommandParameterProperty);
            }
            set
            {
                if (!object.Equals(this.LoadMoreDataCommandParameter, value))
                {
                    SetValue(LoadMoreDataCommandParameterProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public Boolean CanLoadMoreData
        {
            get
            {
                return (Boolean)GetValue(CanLoadMoreDataProperty);
            }
            set
            {
                if (!object.Equals(this.CanLoadMoreData, value))
                {
                    SetValue(CanLoadMoreDataProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsLoadingData
        {
            get
            {
                return (Boolean)GetValue(IsLoadingDataProperty);
            }
            set
            {
                if (!object.Equals(this.IsLoadingData, value))
                {
                    SetValue(IsLoadingDataProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String caller = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName: caller));
        }

        private void EnhancedDataGridControl_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            var vm = e.Row.DataContext as ViewModels.ScriptScripterViewModelBase;

            if (vm != null)
            {
                object o = vm.ViewBound;
            }


            if (e.Row == null)
                return;
            e.Row.PreviewMouseDown += Row_PreviewMouseDown;
        }

        private void DettachScrollViewerEvent()
        {
            if (_scrollViewer == null)
                return;

            _scrollViewer.ScrollChanged -= _scrollViewer_ScrollChanged;
            _scrollViewer = null;
        }

        private void AttachScrollViewerEvent()
        {
            //if already set, then we are already wired up
            if (_scrollViewer != null)
                return;

            _scrollViewer = this.GetScrollViewer(this);
            var x = _scrollViewer.Content;
            var y = new GridContentControl();

            System.Windows.Data.Binding commandBinding = new System.Windows.Data.Binding(nameof(LoadMoreDataCommand));
            commandBinding.Source = this;
            y.LoadButton.SetBinding(Button.CommandProperty, commandBinding);

            System.Windows.Data.Binding paramBinding = new System.Windows.Data.Binding(nameof(LoadMoreDataCommandParameter));
            paramBinding.Source = this;
            y.LoadButton.SetBinding(Button.CommandParameterProperty, paramBinding);



            System.Windows.Data.Binding isLoadingBinding = new System.Windows.Data.Binding(nameof(IsLoadingData));
            isLoadingBinding.Source = this;

            y.DataContext = this;
            //y.LoadingPanel.SetBinding(Button.VisibilityProperty, isLoadingBinding);


            _scrollViewer.Content = y;
            y.GridContent.Content = x;

            if (_scrollViewer != null)
                _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
        }

        private ScrollViewer GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
                return (ScrollViewer)o;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }

        #region IExportCSVSupport implementation

        public string GetCSVData()
        {
            var temp = this.CanSelectMultipleItems;
            this.CanSelectMultipleItems = true;
            this.SelectAllCells();

            this.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, this);

            this.UnselectAllCells();
            this.CanSelectMultipleItems = temp;

            string result = (string)System.Windows.Clipboard.GetData(System.Windows.DataFormats.CommaSeparatedValue);

            return result;
        }

        #endregion
    }
}
