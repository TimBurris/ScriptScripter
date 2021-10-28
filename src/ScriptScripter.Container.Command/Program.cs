using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
namespace ScriptScripter.Container.Command
{
    partial class Program
    {
        //these variables will be setup based on input parameters
        private static bool _verbose;
        private static string _sourceContainerPath;
        private static string _destinationContainerPath;

        //these will come from our IoC Container
        private static NLog.ILogger _logger;
        private static Processor.Data.Contracts.IScriptRepositoryFactory _scriptRepositoryFactory;
        private static Processor.Services.Contracts.IScriptingService _scriptingService;

        static void Main(string[] args)
        {
            InitIoc();

            //logger used in inputparam init, so make sure you get it going before processing input params
            _logger = Ninjector.Container.Get<NLog.ILogger>();

            //setup based on the our input parameters
            InitFromInputParamsExitIfInvalid(args);

            //for debugging/loggin purposes, log out what the params were 
            LogParamState();

            //confirm both source and destination exist
            _scriptingService = Ninjector.Container.Get<Processor.Services.Contracts.IScriptingService>();
            ConfirmContainerExistsExitIfInvalid(_sourceContainerPath);
            ConfirmContainerExistsExitIfInvalid(_destinationContainerPath);


            //fire up our factory so we can get to work
            _scriptRepositoryFactory = Ninjector.Container.Get<Processor.Data.Contracts.IScriptRepositoryFactory>();

            //fetch the scripts from source
            IEnumerable<Processor.Data.Models.Script> scriptsToCopy = GetScriptsToCopyExitIfFailOrNone();

            if (scriptsToCopy.Any())
            {
                //now apply the scripts to the database
                CopyScriptsToDestination(scriptsToCopy);
            }
            else
            {
                _logger.Info("No scripts in source");
            }

            //all above routines are set to exit on fail
            //   so if we make it here, everything worked
            Environment.Exit(0);//success
        }

        private static void ConfirmContainerExistsExitIfInvalid(string containerPath)
        {
            var result = _scriptingService.TestScriptContainerExists(containerPath);
            if (!result.WasSuccessful)
            {
                _logger.Info($"Script container '{containerPath}' does not exist");
                Environment.Exit(5003);
            }
        }

        private static void CopyScriptsToDestination(IEnumerable<Processor.Data.Models.Script> scriptsToCopy)
        {
            var _destRepo = _scriptRepositoryFactory.GetScriptsRepository(_destinationContainerPath);

            foreach (var script in scriptsToCopy)
            {
                try
                {
                    _logger.Info($"Adding '{script.ScriptId}' to the destination container");
                    _destRepo.AddNewScript(script);

                    _logger.Info("Script copied successfully");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "error copying scripts");
                    Environment.Exit(5006);
                }
            }
        }

        private static IEnumerable<Processor.Data.Models.Script> GetScriptsToCopyExitIfFailOrNone()
        {

            IEnumerable<Processor.Data.Models.Script> scripts = null;
            try
            {
                var repo = _scriptRepositoryFactory.GetScriptsRepository(_sourceContainerPath);
                scripts = repo.GetAllScripts();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error getting scripts from source");
                Environment.Exit(5005);
            }
            return scripts;
        }

        private static void InitFromInputParamsExitIfInvalid(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<IncomingOptions>(args)
                         .WithParsed<IncomingOptions>(o =>
                         {
                             _verbose = o.Verbose;
                             _sourceContainerPath = o.SourceContainerPath;
                             _destinationContainerPath = o.DestinationContainerPath;
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

            if (_sourceContainerPath == null)
            {
                _logger.Error("sourcecontainerpath is missing");
                Environment.Exit(5002);
            }
            if (_destinationContainerPath == null)
            {
                _logger.Error("destinationcontainerpath is missing");
                Environment.Exit(5002);
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
            _logger.Info($"SourceContainerPath: {_sourceContainerPath}");
            _logger.Info($"DestinationContainerPath: {_destinationContainerPath}");
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
