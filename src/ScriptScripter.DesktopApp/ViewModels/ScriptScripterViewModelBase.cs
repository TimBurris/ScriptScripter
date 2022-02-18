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

            //when reload is complete tell the commandmanager that all buttons should be retested for "canexecute"
            //   this fixes and issue where we see buttons not getting enabled/disabled after the page takes a bit of time (>3 seconds) to load/reload
            this.ReloadCompleted += (sender, e) => System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        protected override void OnReloadDataFailed()
        {
            _logger?.Error(this.LoadFailedException, message: "Error in load/reload");
            base.OnReloadDataFailed();
        }
    }
}
