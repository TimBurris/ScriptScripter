using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Repositories
{
    public class ScriptsRepository : Contracts.IScriptsRepository
    {
        private System.IO.Abstractions.IFileSystem _fileSystem;

        public ScriptsRepository(System.IO.Abstractions.IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string ScriptFilePath { get; set; }

        public Models.Script AddNewScript(Models.Script script)
        {
            var data = this.ReadScripts();

            script.ScriptId = Guid.NewGuid();
            data.Add(script);

            this.WriteScripts(data);

            return script;
        }

        public Models.Script UpdateScript(Models.Script script)
        {
            var data = this.ReadScripts();
            bool found = false;

            int index;
            for (index = 0; index < data.Count; index++)
            {
                if (data[index].ScriptId == script.ScriptId)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                throw new ApplicationException($"Could not locate script with ScriptId {script.ScriptId}.");

            //replace existing script with the updated one
            data[index] = script;

            this.WriteScripts(data);

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

                return this.ReadScripts();
            }
        }

        /// <summary>
        /// just a shorthand method for getting the scripts already ordered... we could implement caching in here if performance ever becomes an issues
        /// </summary>
        private List<Models.Script> OrderedScriptContainerData
        {
            get
            {
                return this.ScriptContainerData.OrderBy(s => s.ScriptDate).ToList();
            }
        }

        #region Read and Write file

        public virtual List<Models.Script> ReadScripts()
        {
            var fileName = this.ScriptFilePath;

            if (!_fileSystem.File.Exists(fileName))
                return new List<Models.Script>();

            var sb = new StringBuilder();

            //you can't use a simple _fileSystem.File.ReadAllText(fileName) because if it's locked by another app it will bomb
            //    I saw the issue occuring when i was testing filesystem changes, if i changed the file in notepadd++ after a save or 2 there would be locked file error
            //TODO: there is still an issue where if the file is in use/locked we get no data, so maybe we delay the firing of the event from filewatcher? and do a couple retrys in here?
            using (var fileStream = _fileSystem.File.Open(fileName, mode: System.IO.FileMode.Open, access: System.IO.FileAccess.Read, share: System.IO.FileShare.ReadWrite))
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
                return new List<Models.Script>();

            using (var x = new System.IO.StringReader(txt))
            {
                return this.ReadScripts(x);
            }
        }

        public virtual List<Models.Script> ReadScripts(System.IO.TextReader reader)
        {
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Models.Script>), new System.Xml.Serialization.XmlRootAttribute("Scripts"));
                return (List<Models.Script>)serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidScriptContainterContentsException(message: "Failed to read scripts", innerException: ex);
            }
        }

        public virtual void WriteScripts(IEnumerable<Models.Script> scripts)
        {
            using (var x = new System.IO.StringWriter())
            {
                this.WriteScripts(x, scripts);

                _fileSystem.File.WriteAllText(this.ScriptFilePath, x.ToString());
            }
        }

        public virtual void WriteScripts(System.IO.TextWriter writer, IEnumerable<Models.Script> scripts)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Models.Script>), new System.Xml.Serialization.XmlRootAttribute("Scripts"));

            //this newlinehandling settings is required for newlines to be written as \r\n instead of just \n
            //     see here:   https://social.msdn.microsoft.com/Forums/en-US/5a81f838-a89f-4f48-899c-1c3973222c53/xml-serializationdeserializtion-turns-rn-to-n-only?forum=xmlandnetfx
            var ws = new System.Xml.XmlWriterSettings();
            ws.NewLineHandling = System.Xml.NewLineHandling.Entitize;
            ws.Indent = true;
            using (var wr = System.Xml.XmlWriter.Create(writer, ws))
            {
                serializer.Serialize(wr, scripts.ToList());
            }
        }

        #endregion
    }
}
