using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Contracts
{
    public interface IScriptsRepository
    {
        Models.ScriptContainer ScriptContainer { get; set; }

        IEnumerable<Models.Script> GetAllScripts();
        IEnumerable<Models.Script> GetAllScriptsAfterRevisionNumber(int revisionNumber);

        Models.Script GetLastScript();
        Models.Script GetScriptByRevisionNumber(int revisionNumber);

        Models.Script AddNewScript(Models.Script script);
        Models.Script UpdateScript(Models.Script script);
    }
}
