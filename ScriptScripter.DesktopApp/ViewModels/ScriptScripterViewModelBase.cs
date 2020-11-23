using NLog;
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
        protected readonly ILogger _logger;

        public ScriptScripterViewModelBase(ILogger logger)
        {
            _logger = logger;
        }

        protected override void OnReloadDataFailed()
        {
            _logger?.Error(this.LoadFailedException, message: "Error in load/reload");
            base.OnReloadDataFailed();
        }
    }
}
