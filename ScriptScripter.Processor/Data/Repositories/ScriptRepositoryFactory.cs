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
        public Contracts.IScriptsRepository GetScriptsRepository(Models.ScriptContainer scriptContainer)
        {
            var repo = Ninjector.Container.Get<Contracts.IScriptsRepository>();// new Ninject.Parameters.ConstructorArgument(name: "scriptContainer", value: scriptContainer));
            repo.ScriptContainer = scriptContainer;

            return repo;
        }

    }
}
