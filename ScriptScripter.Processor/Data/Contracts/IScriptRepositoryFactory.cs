using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Data.Contracts
{
    public interface IScriptRepositoryFactory
    {
        IScriptsRepository GetScriptsRepository(string scriptFilePath);
    }
}
