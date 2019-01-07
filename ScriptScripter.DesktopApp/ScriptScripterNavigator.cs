using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp
{
    public class ScriptScripterNavigator
          : NinjaMvvm.Wpf.Navigator<MainWindow, DialogWindow>, NinjaMvvm.Wpf.Abstractions.INavigator
    {
        public ScriptScripterNavigator(NinjaMvvm.Wpf.Abstractions.IViewModelResolver viewModelResolver)
            : base(viewModelResolver)
        {

        }
        protected override void BindViewModelToMainWindow<TViewModel>(TViewModel viewModel, MainWindow mainWindow)
        {
            mainWindow.mainContent.Content = viewModel;
        }
    }
}
