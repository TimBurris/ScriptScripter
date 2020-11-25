using Ninject;
using Ninject.Extensions.Factory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor
{
    public class Ninjector : Ninject.Modules.NinjectModule
    {
        public static IKernel Container { get; set; }

        public override void Load()
        {
            this.Bind<Data.Contracts.IScriptRepositoryFactory>().To<Data.Repositories.ScriptRepositoryFactory>();
            this.Bind<Services.Contracts.IEventNotificationService>().To<Services.EventNotifcationService>().InSingletonScope();//Note: eventnotification must be singleton
            this.Bind<Services.Contracts.ICryptoService>().To<Services.CryptoService>();
            this.Bind<System.IO.Abstractions.IFileSystem>().To<System.IO.Abstractions.FileSystem>(); //when injected just use the real filesystem, but in tests they will use their own filesystem

            //Repos
            this.Bind<Data.Contracts.IConfigurationRepository>().To<Data.Repositories.ConfigurationRepository>();
            this.Bind<Data.Contracts.IScriptContainerRepository>().To<Data.Repositories.ScriptContainerRepository>();
            this.Bind<Data.Contracts.IRevisionRepository>().To<Data.Repositories.RevisionRepository>();
            this.Bind<Data.Contracts.IScriptsRepository>().To<Data.Repositories.ScriptsRepository>();

            //Services
            this.Bind<Services.Contracts.IScriptingService>().To<Services.ScriptingService>();
            this.Bind<Services.Contracts.IDatabaseUpdater>().To<Services.DatabaseUpdater>();
            this.Bind<Services.Contracts.IDatabaseUpdaterFactory>().ToFactory();
            this.Bind<Services.Contracts.IScriptContainerWatcherService>().To<Services.ScriptContainerWatcherService>();
            this.Bind<Services.Contracts.IConfigurationFileUpgradeService>().To<Services.ConfigurationFileUpgradeService>();
            this.Bind<Services.Contracts.IScriptWarningService>().To<Services.ScriptWarningService>();
            this.Bind<IFileSystemWatcherFactory>().To<FileSystemWatcherFactory>();
        }
    }
}
