using ScriptScripter.Processor.Data.Models;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Repositories
{

    /// <summary>
    /// a file/folder implementation which uses the assigned ScriptContainerPath to decide whehter or not to use <see cref="ScriptFolderRepository"/> or the <see cref="ScriptFileRepository"/>
    /// </summary>
    public class ScriptsRepository : Contracts.IScriptsRepository
    {
        private readonly ScriptFolderRepository _scriptFolderRepository;
        private readonly ScriptFileRepository _scriptFileRepository;
        private readonly IFileSystem _fileSystem;
        private string _scriptContainerPath;

        public ScriptsRepository(ScriptFolderRepository scriptFolderRepository, ScriptFileRepository scriptFileRepository, System.IO.Abstractions.IFileSystem fileSystem)
        {
            _scriptFolderRepository = scriptFolderRepository;
            _scriptFileRepository = scriptFileRepository;
            _fileSystem = fileSystem;
        }
        public string ScriptContainerPath
        {
            get => _scriptContainerPath; set
            {
                _scriptContainerPath = value;
                _scriptFileRepository.ScriptContainerPath = value;
                _scriptFolderRepository.ScriptContainerPath = value;
            }
        }

        private bool? ScriptPathIsAFolder()
        {
            var path = this.ScriptContainerPath;

            if (_fileSystem.File.Exists(path))
                return true;
            else if (_fileSystem.Directory.Exists(path))
                return true;
            else
                return null;// unknown
        }

        private Contracts.IScriptsRepository ResolvedRepository()
        {
            var isFolder = this.ScriptPathIsAFolder();

            if (isFolder == true)
            {
                return _scriptFolderRepository;
            }
            else // we return file repo even if NULL, if the thing does not exists (null filepath) we'll just let the file repo handle however it handles missing files
            {
                return _scriptFileRepository;
            }
        }

        public Script AddNewScript(Script script)
        {
            return this.ResolvedRepository().AddNewScript(script);
        }

        public IEnumerable<Script> GetAllScripts()
        {
            return this.ResolvedRepository().GetAllScripts();
        }

        public Script GetLastScript()
        {
            return this.ResolvedRepository().GetLastScript();
        }

        public Script UpdateScript(Script script)
        {
            return this.ResolvedRepository().UpdateScript(script);
        }
    }
}
