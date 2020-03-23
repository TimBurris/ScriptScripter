using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
namespace ScriptScripter.Command
{
    class Program
    {
        public class Options
        {
            //[Option('c', "connectionstring", Required = true, HelpText = "The connection string for the database where scripts should be applied")]
            //public string ConnectionString { get; set; }
            [Option('f', "scriptfilepath", Required = true, HelpText = "The full path to the script file that is to be applied")]
            public string ScriptFilePath { get; set; }

            [Option('s', "server", Required = true, HelpText = "The server name (with instance) to connect to")]
            public String Server { get; set; }

            [Option('d', "databasename", Required = true, HelpText = "The database to apply the scripts to")]
            public String DatabaseName { get; set; }


            [Option('t', "trustedconnection", Required = false, HelpText = "y|yes|t|true to use windows auth")]
            public bool UseTrustedConnection { get; set; }

            [Option('p', "password", Required = false, HelpText = "Optional, passord for the database connection use when not using Trusted Connection")]
            public String Password { get; set; }

            [Option('u', "user", Required = false, HelpText = "Optional, passord for the database connection use when not using Trusted Connection")]
            public String Username { get; set; }


            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
        }

        public static bool _verbose;
        static async Task Main(string[] args)
        {
            Processor.Data.Models.DatabaseConnectionParameters connectionParams = null;
            string scriptFilePath = null;
            bool failed = false;
            var parseResult = Parser.Default.ParseArguments<Options>(args)
                         .WithParsed<Options>(o =>
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
                         .WithNotParsed<Options>(o =>
                         {
                             failed = true;
                         });

            if (failed)
            {
                throw new ApplicationException("Exiting due to missing or failed parameters");
            }
            if (connectionParams == null)
            {
                throw new ApplicationException("connection parameters wer now received");
            }
            if (scriptFilePath == null)
            {
                throw new ApplicationException("scriptfilepath is missing");

            }
            if (!System.IO.File.Exists(scriptFilePath))
            {
                throw new ApplicationException($"{scriptFilePath} does not exist on disk");
            }

            var kernel = new Ninject.StandardKernel(new Ninject.Modules.INinjectModule[] { new Ninjector(), new ScriptScripter.Processor.Ninjector() });

            Ninjector.Container = kernel;
            ScriptScripter.Processor.Ninjector.Container = kernel;

            var progress = new Progress<Processor.Dto.ApplyScriptProgress>();
            progress.ProgressChanged += Progress_ProgressChanged;


            var service = Ninjector.Container.Get<Processor.Services.Contracts.IScriptingService>();
            var connectionResult=await service.TestDatabaseConnectionAsync(connectionParams);
            if(!connectionResult.WasSuccessful)
            {
                throw new ApplicationException($"Failed to connect to database: {connectionResult.Message}");
            }

            var scriptsToRun = service.GetScriptsThatNeedRun(connectionParams, scriptFilePath);
            await service.ApplyScriptsToDatabaseAsync(connectionParams, scriptsToRun, progress);

        }

        private static void Progress_ProgressChanged(object sender, Processor.Dto.ApplyScriptProgress e)
        {
            if (e.IsStarting)
            {
                var total = e.ScriptsCompleted + e.ScriptsRemaining;
                var current = e.ScriptsCompleted + 1;
                Console.WriteLine($"Processing Revision #{e.Script.RevisionNumber} ({current} of {total})");
                if (_verbose)
                {
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(e.Script, Newtonsoft.Json.Formatting.Indented));
                }
            }
            else
            {
                Console.WriteLine($"Completed Revision #{e.Script.RevisionNumber}");
            }
        }
    }
}
