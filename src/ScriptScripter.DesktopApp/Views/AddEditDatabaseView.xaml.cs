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

namespace ScriptScripter.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for AddEditDatabaseView.xaml
    /// </summary>
    public partial class AddEditDatabaseView
    {
        public AddEditDatabaseView()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            try
            {
                Ninjector.Container?.Inject(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Assert(false, "Error in DI, probably forgot to map something in Ninjector", ex.Message);
            }
        }


        private Contracts.IFileNameDropHandler _fileNameDropHandler;
        [Ninject.Inject]
        public Contracts.IFileNameDropHandler FileNameDropHandler
        {
            get
            {
                return _fileNameDropHandler;
            }
            set
            {
                _fileNameDropHandler = value;

                _fileNameDropHandler.Init(ScriptFileBox);
                _fileNameDropHandler.OnDropAction = (name) => ScriptFileBox.Text = name;
            }
        }

    }
}
