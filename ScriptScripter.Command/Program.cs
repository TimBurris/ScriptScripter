using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
namespace ScriptScripter.Command
{
    partial class Program
    {

        public static bool _verbose;
        public static NLog.ILogger _logger;

        static async Task Main(string[] args)
        {
            var kernel = new Ninject.StandardKernel(new Ninject.Modules.INinjectModule[] { new Ninjector(), new ScriptScripter.Processor.Ninjector() });

            Ninjector.Container = kernel;
            ScriptScripter.Processor.Ninjector.Container = kernel;
            _logger = Ninjector.Container.Get<NLog.ILogger>();

            Processor.Data.Models.DatabaseConnectionParameters connectionParams = null;
            string scriptFilePath = null;

            var parseResult = Parser.Default.ParseArguments<IncomingOptions>(args)
                         .WithParsed<IncomingOptions>(o =>
                        {
                            _verbose = o.Verbose;
                            scriptFilePath = o.ScriptFilePath;
                            connectionParams = new Processor.Data.Models.DatabaseConnectionParameters()
                            {
                                DatabaseName = o.DatabaseName,
                                Server = o.Server,
                                Password = o.Password,
                                Username = o.Username,
                                UseTrustedConnection = o.UseTrustedConnection

                            };

                        })
                         .WithNotParsed<IncomingOptions>(o =>
                         {
                             _logger.Error("Exiting due to missing or failed parameters");
                             foreach (var x in o)
                             {
                                 _logger.Error(Newtonsoft.Json.JsonConvert.SerializeObject((object)x, Newtonsoft.Json.Formatting.Indented));
                             }
                             Environment.Exit(5000);
                         });


            if (connectionParams == null)
            {
                _logger.Error("connection parameters wer now received");
                Environment.Exit(5001);
            }
            if (scriptFilePath == null)
            {
                _logger.Error("scriptfilepath is missing");
                Environment.Exit(5002);
            }
            if (!System.IO.File.Exists(scriptFilePath))
            {
                _logger.Error($"{scriptFilePath} does not exist on disk");
                Environment.Exit(5003);
            }

            if (_verbose)
            {
                _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(connectionParams, Newtonsoft.Json.Formatting.Indented));
            }

            var progress = new Progress<Processor.Dto.ApplyScriptProgress>();
            progress.ProgressChanged += Progress_ProgressChanged;


            var service = Ninjector.Container.Get<Processor.Services.Contracts.IScriptingService>();
            var connectionResult = await service.TestDatabaseConnectionAsync(connectionParams);
            if (!connectionResult.WasSuccessful)
            {
                _logger.Error($"Failed to connect to database: {connectionResult.Message}");
                Environment.Exit(5004);
            }


            IEnumerable<Processor.Data.Models.Script> scriptsToRun = null;
            try
            {
                scriptsToRun = service.GetScriptsThatNeedRun(connectionParams, scriptFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error getting scripts to run");
                Environment.Exit(5005);
            }

            try
            {
                var result = await service.ApplyScriptsToDatabaseAsync(connectionParams, scriptsToRun, progress);
                if (result.WasSuccessful)
                {
                    Environment.Exit(0);//success
                }
                else
                {
                    _logger.Error($"error applying scripts: {result.Message}");
                    Environment.Exit(5007);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error applying scripts");
                Environment.Exit(5006);
            }
        }
        private static void Progress_ProgressChanged(object sender, Processor.Dto.ApplyScriptProgress e)
        {
            if (e.IsStarting)
            {
                var total = e.ScriptsCompleted + e.ScriptsRemaining;
                var current = e.ScriptsCompleted + 1;
                _logger.Info($"Processing Revision #{e.Script.RevisionNumber} ({current} of {total})");
                if (_verbose)
                {
                    _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(e.Script, Newtonsoft.Json.Formatting.Indented));
                }
            }
            else
            {
                _logger.Info($"Completed Revision #{e.Script.RevisionNumber}");
            }
        }
    }
}
