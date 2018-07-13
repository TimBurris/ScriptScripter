﻿using Ninject;
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
            Bind<NLog.ILogger>().ToMethod(p => NLog.LogManager.GetLogger(name: p.Request.Target.Member.DeclaringType.FullName));
            Bind<Contracts.INavigator>().To<Navigator>().InSingletonScope();
            Bind<Contracts.IFileNameDropHandler>().To<FileNameDropHandler>();
            Bind<FileAndFolderDialog.Abstractions.IFileDialogService>().To<FileAndFolderDialog.Wpf.FileDialogService>();
            Bind<FaultlessExecution.Abstractions.IFaultlessExecutionService>().To<FaultlessExecution.FaultlessExecutionService>();
            Bind<Contracts.IViewModelFaultlessService>().To<ViewModelFaultlessService>();
            Bind<Contracts.IThemeService>().To<Themes.ThemeService>();
        }
    }
}
