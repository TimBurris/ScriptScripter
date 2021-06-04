using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Repositories
{
    public class ConfigFileBase
    {
        private System.IO.Abstractions.IFileSystem _fileSystem;

        public ConfigFileBase(System.IO.Abstractions.IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        protected void WriteFile(Settings settings)
        {
            var fileName = this.GetFileName();
            var directoryName = _fileSystem.Path.GetDirectoryName(fileName);

            if (!_fileSystem.Directory.Exists(directoryName))
                _fileSystem.Directory.CreateDirectory(directoryName);

            // serialize JSON to a string and then write string to a file
            _fileSystem.File.WriteAllText(fileName, Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented));
        }

        public string ConfigurationFileName { get; set; }

        protected Settings ReadFile()
        {
            var fileName = this.GetFileName();

            //exists
            if (_fileSystem.File.Exists(fileName))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(_fileSystem.File.ReadAllText(fileName)) ?? new Settings();
            else
                return new Settings();
        }

        private string GetFileName()
        {
            if (string.IsNullOrEmpty(this.ConfigurationFileName))
            {
                this.ConfigurationFileName = Properties.Settings.Default.ConfigurationSettingsFile;
            }
            var s = Environment.ExpandEnvironmentVariables(this.ConfigurationFileName);

            return _fileSystem.Path.GetFullPath(s);
        }

        protected class Settings
        {
            public string ThemeName { get; set; }
            public string DeveloperName { get; set; }
            public Models.ServerConnectionParameters ServerConnectionInfo { get; set; } = new Models.ServerConnectionParameters();
            public List<Models.ScriptContainer> ScriptContainers { get; set; } = new List<Models.ScriptContainer>();
        }
    }
}
