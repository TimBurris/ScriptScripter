using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services.Contracts
{
    public interface IScriptWarningService
    {
        IEnumerable<string> CheckSql(string sql);

    }
}
