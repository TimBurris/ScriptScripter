using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace ScriptScripter.Processor.Data.Repositories
{
    public class ScriptRepositoryFactory : Contracts.IScriptRepositoryFactory
    {
        public Contracts.IScriptsRepository GetScriptsRepository(string scriptFilePath)
        {
            var repo = Ninjector.Container.Get<Contracts.IScriptsRepository>();
            repo.ScriptFilePath = scriptFilePath;

            return repo;
        }

    }
}
