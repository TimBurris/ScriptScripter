using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services.Contracts
{
    public interface IDatabaseUpdater : IDisposable
    {
        Data.Models.DatabaseConnectionParameters DatabaseConnectionParams { get; set; }

        void CreateScriptingSupportObjects();
        void RunScript(Data.Models.Script script);

        void LogScript(Data.Models.Script script, string executedByDeveloperName);

        //maybe transaciton should be separte?
        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
