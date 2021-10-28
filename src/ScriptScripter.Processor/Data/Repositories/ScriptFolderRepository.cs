using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptScripter.Processor.Data.Repositories
{
    public class ScriptFolderRepository : Contracts.IScriptsRepository
    {
        private System.IO.Abstractions.IFileSystem _fileSystem;
        private readonly ILogger _logger;

        public ScriptFolderRepository(System.IO.Abstractions.IFileSystem fileSystem, NLog.ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public string ScriptContainerPath { get; set; }

        public Models.Script AddNewScript(Models.Script script)
        {
            script.ScriptId = Guid.NewGuid();

            this.WriteScript(script, overwrite: false);//no overwrite, if it exists and you called addnew, then you've done something wrong

            return script;
        }

        public Models.Script UpdateScript(Models.Script script)
        {

            var originalScript = ReadScript(script.ScriptId);
            bool found = originalScript != null;

            if (!found)
                throw new ApplicationException($"Could not locate script with ScriptId {script.ScriptId}.");

            //replace existing script with the updated one
            this.WriteScript(script, overwrite: true);


            return script;
        }

        public IEnumerable<Models.Script> GetAllScripts()
        {
            return this.ScriptContainerData.ToList();
        }

        public Models.Script GetLastScript()
        {
            return this.OrderedScriptContainerData.LastOrDefault();
        }

        private List<Models.Script> ScriptContainerData
        {
            get
            {
                //should we cache? or load everytime?
                ///   problem with cache is, if we return a instance to them, and they modify it in memory... that will affect future calls that return the list (because it's the same instance)
                ///   but that would only occur for calls using the exact same repo...
                //if (_allScripts == null)
                //    _allScripts = this.ReadScripts();

                return this.ReadAllScriptFiles();
            }
        }

        /// <summary>
        /// just a shorthand method for getting the scripts already ordered... we could implement caching in here if performance ever becomes an issue
        /// </summary>
        private List<Models.Script> OrderedScriptContainerData
        {
            get
            {
                return this.ScriptContainerData.OrderBy(s => s.ScriptDate).ToList();
            }
        }

        #region Read and Write file
        private string BuildFilePathForScriptId(Guid scriptId)
        {
            return _fileSystem.Path.Combine(this.ScriptContainerPath, scriptId.ToString() + ".xml");
        }
        public virtual List<Models.Script> ReadAllScriptFiles()
        {
            var results = new List<Models.Script>();
            var folderPath = this.ScriptContainerPath;

            if (!_fileSystem.Directory.Exists(folderPath))
                return new List<Models.Script>();

            foreach (var filePath in _fileSystem.Directory.GetFiles(path: folderPath, "*.xml"))
            {
                var fileName = _fileSystem.Path.GetFileNameWithoutExtension(filePath);

                //filename must be a guid, else it's not in the correct format and will be ignored
                if (Guid.TryParse(fileName, out Guid scriptId))
                {
                    var script = this.ReadScript(filePath);
                    if (script == null)
                    {
                        _logger.Warn($"'{filePath}' returned and empty script");
                    }
                    else
                    {
                        results.Add(script);
                    }
                }
                else
                {
                    //skip, not a scriptid
                    _logger.Warn($"'{filePath}' skipped becuase filename is not a valid guid");
                }
            }

            return results;
        }

        public virtual Models.Script ReadScript(Guid scriptId)
        {
            string filePath = BuildFilePathForScriptId(scriptId);

            return this.ReadScript(filePath);
        }

        public virtual Models.Script ReadScript(string filePath)
        {
            var sb = new StringBuilder();
            //you can't use a simple _fileSystem.File.ReadAllText(fileName) because if it's locked by another app it will bomb
            //    I saw the issue occuring when i was testing filesystem changes, if i changed the file in notepadd++ after a save or 2 there would be locked file error
            //TODO: there is still an issue where if the file is in use/locked we get no data, so maybe we delay the firing of the event from filewatcher? and do a couple retrys in here?

            using (var fileStream = _fileSystem.File.Open(filePath, mode: System.IO.FileMode.Open, access: System.IO.FileAccess.Read, share: System.IO.FileShare.ReadWrite))
            {
                using (var reader = new System.IO.StreamReader(fileStream))
                {
                    while (!reader.EndOfStream)
                    {
                        sb.AppendLine(reader.ReadLine());
                    }
                }
            }

            var txt = sb.ToString();

            if (string.IsNullOrEmpty(txt))
                return null;

            using (var x = new System.IO.StringReader(txt))
            {
                return this.ReadScript(x);
            }
        }

        public virtual Models.Script ReadScript(System.IO.TextReader reader)
        {
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.Script));
                return (Models.Script)serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidScriptContainterContentsException(message: "Failed to read scripts", innerException: ex);
            }
        }

        public virtual void WriteScript(Models.Script script, bool overwrite)
        {
            string filePath = BuildFilePathForScriptId(script.ScriptId);

            var exists = _fileSystem.File.Exists(filePath);
            if (!overwrite && exists)
            {
                throw new ApplicationException($"'{filePath}' already exists");
            }

            if (!exists)
                _fileSystem.File.CreateText(filePath).Close();

            using (var x = new System.IO.StringWriter())
            {
                this.WriteScript(x, script);

                _fileSystem.File.WriteAllText(filePath, x.ToString());
            }
        }

        public virtual void WriteScript(System.IO.TextWriter writer, Models.Script script)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.Script));
            //, 
            //new System.Xml.Serialization.XmlRootAttribute("Scripts")
            //this newlinehandling settings is required for newlines to be written as \r\n instead of just \n
            //     see here:   https://social.msdn.microsoft.com/Forums/en-US/5a81f838-a89f-4f48-899c-1c3973222c53/xml-serializationdeserializtion-turns-rn-to-n-only?forum=xmlandnetfx
            var ws = new System.Xml.XmlWriterSettings();
            ws.NewLineHandling = System.Xml.NewLineHandling.Entitize;
            ws.Indent = true;
            using (var wr = System.Xml.XmlWriter.Create(writer, ws))
            {
                serializer.Serialize(wr, script);
            }
        }

        #endregion
    }
}
