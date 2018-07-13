using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Repositories
{
    public class ConfigurationRepository : ConfigFileBase, Contracts.IConfigurationRepository
    {
        [Ninject.Inject]
        public Services.Contracts.ICryptoService CryptoService { get; set; }

        [Ninject.Inject]
        public Services.Contracts.IEventNotificationService EventNotificationService { get; set; }

        public ConfigurationRepository(System.IO.Abstractions.IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public ConfigurationRepository(System.IO.Abstractions.IFileSystem fileSystem, string configurationFileName)
            : base(fileSystem, configurationFileName)
        {
        }

        public ConfigurationRepository()
            : base()
        {
        }
        public string GetThemeName()
        {
            var settings = this.ReadFile();

            return settings.ThemeName;
        }

        public void SetThemeName(string themeName)
        {
            var settings = this.ReadFile();
            settings.ThemeName = themeName;

            this.WriteFile(settings);
        }

        public string GetDeveloperName()
        {
            var settings = this.ReadFile();

            return settings.DeveloperName;
        }

        public void SetDeveloperName(string developerName)
        {
            var settings = this.ReadFile();
            settings.DeveloperName = developerName;

            this.WriteFile(settings);
        }

        public Models.ServerConnectionParameters GetServerConnectionParameters()
        {
            var settings = this.ReadFile();

            //decrypt
            if (!string.IsNullOrEmpty(settings.ServerConnectionInfo.Password))
                settings.ServerConnectionInfo.Password = this.CryptoService.Decrypt(settings.ServerConnectionInfo.Password);

            return settings.ServerConnectionInfo;
        }

        public void SetServerConnectionParameters(Models.ServerConnectionParameters connectionParameters)
        {
            var settings = this.ReadFile();

            //first write all values to a new instance
            settings.ServerConnectionInfo.Server = connectionParameters.Server;
            settings.ServerConnectionInfo.Password = connectionParameters.Password;
            settings.ServerConnectionInfo.Username = connectionParameters.Username;
            settings.ServerConnectionInfo.UseTrustedConnection = connectionParameters.UseTrustedConnection;

            //encrypt the NEW isntance, we don'w want to change the object they passed us
            if (!string.IsNullOrEmpty(settings.ServerConnectionInfo.Password))
                settings.ServerConnectionInfo.Password = this.CryptoService.Encrypt(settings.ServerConnectionInfo.Password);

            this.WriteFile(settings);

            EventNotificationService.NotifyServerConnectionChanged();
        }
    }
}
