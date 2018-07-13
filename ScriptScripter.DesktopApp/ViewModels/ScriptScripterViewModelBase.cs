using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class ScriptScripterViewModelBase : NinjaMvvm.Wpf.WpfViewModelBase
    {
        public ScriptScripterViewModelBase()
        {
            if (!this.IsInDesignMode())
                try
                {
                    Ninjector.Container?.Inject(this);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Assert(false, "Error in DI, probably forgot to map something in Ninjector", ex.Message);
                }
        }

        [Ninject.Inject]
        public Contracts.INavigator Navigator { get; set; }



        public async Task ReloadAsync()
        {
            await base.ReloadDataAsync();
        }
    }
}
