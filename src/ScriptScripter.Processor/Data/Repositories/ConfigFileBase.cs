using Newtonsoft.Json;
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
            {
                string fileContents = _fileSystem.File.ReadAllText(fileName);
                var jsonsettings = new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = ConfigFileContractResolver.Instance };
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(fileContents, jsonsettings) ?? new Settings();
            }
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

        private class ConfigFileContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            public static readonly ConfigFileContractResolver Instance = new ConfigFileContractResolver();
            protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);

                //we are supporting the old "ScriptFilePath" propertery when deserializing 
                //  this allows the settings file to be in the new format "ScriptContainerPath" or the previous one, "ScriptFilePath"
                if (type == typeof(Models.ScriptContainer))
                {
                    var property = properties.First(x => x.PropertyName == nameof(Models.ScriptContainer.ScriptContainerPath));
                    var clone = property.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    var newProperty = (Newtonsoft.Json.Serialization.JsonProperty)clone.Invoke(property, new object[0]);
                    newProperty.Readable = false;
                    newProperty.PropertyName = "ScriptFilePath";
                    properties.Add(newProperty);
                }

                return properties;
            }
        }
    }
}
