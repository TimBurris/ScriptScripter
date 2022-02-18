using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp
{
    class Ninjector : Ninject.Modules.NinjectModule
    {
        public static IKernel Container { get; set; }

        public override void Load()
        {
            Container = this.Kernel;

            Bind<NLog.ILogger>().ToMethod(p => NLog.LogManager.GetLogger(name: p.Request.Target.Member.DeclaringType.FullName));
            Bind<Contracts.IFileNameDropHandler>().To<FileNameDropHandler>();
            Bind<FileAndFolderDialog.Abstractions.IFileDialogService>().To<FileAndFolderDialog.Wpf.FileDialogService>();
            Bind<FileAndFolderDialog.Abstractions.IFolderDialogService>().To<FileAndFolderDialog.Wpf.FolderDialogService>();
            Bind<FaultlessExecution.Abstractions.IFaultlessExecutionService>().To<FaultlessExecution.FaultlessExecutionService>();
            Bind<Contracts.IViewModelFaultlessService>().To<ViewModelFaultlessService>();
            Bind<Contracts.IThemeService>().To<Themes.ThemeService>();

            NinjaMvvm.Wpf.Ninject.Component.Init<ScriptScripterNavigator>(Container);
        }
    }
}
