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
        public Contracts.IScriptsRepository GetScriptsRepository(string scriptContainerPath)
        {
            var repo = Ninjector.Container.Get<Contracts.IScriptsRepository>();
            repo.ScriptContainerPath = scriptContainerPath;

            return repo;
        }

    }
}
