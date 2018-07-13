using System;
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
using Ninject;
namespace ScriptScripter.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = Ninjector.Container.Get<ViewModels.MainViewModel>();
            this.DataContext = vm;
            object obj = vm.ViewBound;

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Properties.Settings.Default.MainWindowPlacement = this.GetPlacement();
                Properties.Settings.Default.Save();
            }
            catch { }//not concerned about issues saving placement
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                base.OnSourceInitialized(e);
                this.SetPlacement(Properties.Settings.Default.MainWindowPlacement);
            }
            catch { }//not concerned about issues restoring placement
        }
    }
}
