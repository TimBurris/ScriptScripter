using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.ViewModels
{
    public class ScriptScripterViewModelBase : NinjaMvvm.Wpf.WpfViewModelBase
    {

        //simply exposing the protected method
        public async Task ReloadAsync()
        {
            await this.ReloadDataAsync();
        }

    }
}
