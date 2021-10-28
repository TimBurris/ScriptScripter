using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Contracts
{
    public interface IScriptsRepository
    {
        /// <summary>
        /// Full path to the file or folder that holds all the scripts
        /// </summary>
        string ScriptContainerPath { get; set; }

        IEnumerable<Models.Script> GetAllScripts();

        Models.Script GetLastScript();

        Models.Script AddNewScript(Models.Script script);
        Models.Script UpdateScript(Models.Script script);
    }
}
