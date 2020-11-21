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
        //these variables will be setup based on input parameters
        private static bool _verbose;
        private static string _scriptFilePath;
        private static Processor.Data.Models.DatabaseConnectionParameters _connectionParams;

        //these will come from our IoC Container
        private static NLog.ILogger _logger;
        private static Processor.Services.Contracts.IScriptingService _service;

        static async Task Main(string[] args)
        {
            InitIoc();

            //logger used in inputparam init, so make sure you get it going before processing input params
            _logger = Ninjector.Container.Get<NLog.ILogger>();

            //setup based on the our input parameters
            InitFromInputParamsExitIfInvalid(args);

            //for debugging/loggin purposes, log out what the params were 
            LogParamState();

            //fire up our scripting service so we can get to work
            _service = Ninjector.Container.Get<Processor.Services.Contracts.IScriptingService>();

            //test the database connection
            await TestDatabaseConnectionExitIfFailAsync();

            //fetch the scripts that we should run
            IEnumerable<Processor.Data.Models.Script> scriptsToRun = GetScriptsToRunExitIfFail();

            if (!scriptsToRun.Any())
            {
                _logger.Info("Database is already up to date (no scripts to run)");
            }
            else
            {
                //now apply the scripts to the database
                await ApplyScriptsExitIfFailAsync(scriptsToRun);
            }

            //all above routines are set to exit on fail
            //   so if we make it here, everything worked
            Environment.Exit(0);//success
        }
        private static async Task ApplyScriptsExitIfFailAsync(IEnumerable<Processor.Data.Models.Script> scriptsToRun)
        {
            //init a handler to process the progress updates
            var progress = new Progress<Processor.Dto.ApplyScriptProgress>();
            progress.ProgressChanged += Progress_ProgressChanged;

            try
            {
                var result = await _service.ApplyScriptsToDatabaseAsync(_connectionParams, scriptsToRun, progress);
                if (result.WasSuccessful)
                {
                    _logger.Info("***************************************************\r\nScripts Applied Successfully\r\n***************************************************");
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
        private static async Task TestDatabaseConnectionExitIfFailAsync()
        {
            var connectionResult = await _service.TestDatabaseConnectionAsync(_connectionParams);
            if (!connectionResult.WasSuccessful)
            {
                _logger.Error($"Failed to connect to database: {connectionResult.Message}");
                Environment.Exit(5004);
            }
        }
        private static IEnumerable<Processor.Data.Models.Script> GetScriptsToRunExitIfFail()
        {

            IEnumerable<Processor.Data.Models.Script> scriptsToRun = null;
            try
            {
                scriptsToRun = _service.GetScriptsThatNeedRun(_connectionParams, _scriptFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error getting scripts to run");
                Environment.Exit(5005);
            }
            return scriptsToRun;
        }

        private static void InitFromInputParamsExitIfInvalid(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<IncomingOptions>(args)
                         .WithParsed<IncomingOptions>(o =>
                         {
                             _verbose = o.Verbose;
                             _scriptFilePath = o.ScriptFilePath;
                             _connectionParams = new Processor.Data.Models.DatabaseConnectionParameters()
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


            if (_connectionParams == null)
            {
                _logger.Error("connection parameters wer now received");
                Environment.Exit(5001);
            }
            if (_scriptFilePath == null)
            {
                _logger.Error("scriptfilepath is missing");
                Environment.Exit(5002);
            }
            if (!System.IO.File.Exists(_scriptFilePath))
            {
                _logger.Error($"{_scriptFilePath} does not exist on disk");
                Environment.Exit(5003);
            }
        }

        private static void InitIoc()
        {
            var kernel = new Ninject.StandardKernel(new Ninject.Modules.INinjectModule[] { new Ninjector(), new ScriptScripter.Processor.Ninjector() });

            Ninjector.Container = kernel;
            ScriptScripter.Processor.Ninjector.Container = kernel;
        }

        private static void LogParamState()
        {
            _logger.Info($"ScriptFilePath: {_scriptFilePath}");

            var pw = _connectionParams.Password;
            if (!string.IsNullOrEmpty(pw))
            {
                _connectionParams.Password = "- password removed for security, but it was NOT NULL -";
            }
            _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_connectionParams, Newtonsoft.Json.Formatting.Indented));
            _connectionParams.Password = pw;
        }

        private static void Progress_ProgressChanged(object sender, Processor.Dto.ApplyScriptProgress e)
        {
            if (e.IsStarting)
            {
                var total = e.ScriptsCompleted + e.ScriptsRemaining;
                var current = e.ScriptsCompleted + 1;
                _logger.Info($"Processing Revision #{e.Script.ScriptId} ({current} of {total})");
                if (_verbose)
                {
                    _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(e.Script, Newtonsoft.Json.Formatting.Indented));
                }
            }
            else
            {
                _logger.Info($"Completed Revision #{e.Script.ScriptId}");
            }
        }
    }
}
