using System.Windows;

namespace ScriptScripter.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ScriptView.xaml
    /// </summary>
    public partial class ScriptView
    {
        public ScriptView()
        {
            InitializeComponent();
            this.Loaded += ScriptView_Loaded;
        }

        private void ScriptView_Loaded(object sender, RoutedEventArgs e)
        {
            SqlStatementTextBox.Focus();
        }
    }
}
