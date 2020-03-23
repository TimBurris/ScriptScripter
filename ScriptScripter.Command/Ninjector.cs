using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Command
{
    class Ninjector : Ninject.Modules.NinjectModule
    {
        public static IKernel Container { get; set; }

        public override void Load()
        {
            Container = this.Kernel;

            Bind<NLog.ILogger>().ToMethod(p => NLog.LogManager.GetLogger(name: "ScriptScripter.Command"));
            Bind<FaultlessExecution.Abstractions.IFaultlessExecutionService>().To<FaultlessExecution.FaultlessExecutionService>();
        }
    }
}
