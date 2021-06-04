using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp
{
    public class ViewModelFaultlessService : FaultlessExecution.FaultlessExecutionService, Contracts.IViewModelFaultlessService
    {
        private readonly NinjaMvvm.Wpf.Abstractions.INavigator _navigator;

        public ViewModelFaultlessService(NinjaMvvm.Wpf.Abstractions.INavigator navigator)
        {
            this._navigator = navigator;
        }
        protected override void OnException(Exception ex)
        {
            _navigator.ShowDialog<ViewModels.ErrorViewModel>(vm => vm.LoadFromException(ex));
        }
    }
}
