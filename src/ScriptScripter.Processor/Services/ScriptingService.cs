using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services
{
    public class ScriptingService : Contracts.IScriptingService
    {
        private Contracts.IDatabaseUpdaterFactory _databaseUpdaterFactory;
        private Data.Contracts.IScriptRepositoryFactory _scriptRepoFactory;
        private Data.Contracts.IConfigurationRepository _configurationRepository;
        private System.IO.Abstractions.IFileSystem _fileSystem;
        private Data.Contracts.IRevisionRepository _revisionRepository;

        public ScriptingService(Contracts.IDatabaseUpdaterFactory dbupdaterFactory,
            Data.Contracts.IScriptRepositoryFactory scriptRepoFactory,
            Data.Contracts.IRevisionRepository revisionRepository,
            Data.Contracts.IConfigurationRepository configurationRepository,
            System.IO.Abstractions.IFileSystem fileSystem)
        {
            _databaseUpdaterFactory = dbupdaterFactory;
            _scriptRepoFactory = scriptRepoFactory;
            _revisionRepository = revisionRepository;
            _configurationRepository = configurationRepository;
            _fileSystem = fileSystem;
        }

        //TODO: tests for apply scripts
        public async Task<Dto.ActionResult> ApplyScriptsToDatabaseAsync(Data.Models.DatabaseConnectionParameters databaseConnectionParams,
            IEnumerable<Data.Models.Script> scripts, IProgress<Dto.ApplyScriptProgress> progress)
        {
            return await Task.Run(() => this.ApplyScriptsToDatabase(databaseConnectionParams, scripts, progress));
        }

        private Dto.ActionResult ApplyScriptsToDatabase(Data.Models.DatabaseConnectionParameters databaseConnectionParams,
            IEnumerable<Data.Models.Script> scripts, IProgress<Dto.ApplyScriptProgress> progress)
        {
            string runningDeveloper = _configurationRepository.GetDeveloperName();
            int count = 0;
            //TODO: validate that the first script is the next expected number
            //      then validate that every script is the next number (no gaps)
            var scriptList = scripts.ToList();

            using (var updater = _databaseUpdaterFactory.CreateDatabaseUpdater())//.Invoke())
            {
                updater.DatabaseConnectionParams = databaseConnectionParams;
                updater.CreateScriptingSupportObjects();

                updater.BeginTransaction();
                try
                {
                    foreach (var script in scriptList)
                    {
                        if (progress != null)
                            progress.Report(new Dto.ApplyScriptProgress()
                            {
                                Script = script,
                                IsStarting = true,
                                ScriptsCompleted = count,
                                ScriptsRemaining = scriptList.Count - count,
                            });

                        //apply
                        updater.RunScript(script);
                        updater.LogScript(script, executedByDeveloperName: runningDeveloper);

                        count++;

                        if (progress != null)
                            progress.Report(new Dto.ApplyScriptProgress()
                            {
                                Script = script,
                                Revision = new Data.Models.Revision() { ScriptId = script.ScriptId },
                                IsStarting = false,
                                ScriptsCompleted = count,
                                ScriptsRemaining = scriptList.Count - count,
                            });
                    };

                    updater.CommitTransaction();
                }
                catch (Exception ex)
                {
                    updater.RollbackTransaction();
                    return Dto.ActionResult.FailedResult(this.GetDetailedExceptionMessage(ex));
                }
            }
            return Dto.ActionResult.SuccessResult();
        }
        private String GetDetailedExceptionMessage(Exception ex)
        {
            var message = string.Empty;

            var tempEx = ex;

            while (tempEx != null)
            {
                message += tempEx.Message + Environment.NewLine + tempEx.StackTrace + Environment.NewLine + "******************************************" + Environment.NewLine;
                tempEx = tempEx.InnerException;
            }
            return message;
        }
        public IEnumerable<Data.Models.Script> GetScriptsThatNeedRun(Data.Models.DatabaseConnectionParameters databaseConnectionParams, string scriptContainerPath)
        {
            //Simple, get all the scripts, get all the revisions.  whichever scripts don't exist in the revision list, well that's what you need to run :)

            var repo = _scriptRepoFactory.GetScriptsRepository(scriptContainerPath);
            var revisionsByScriptId = _revisionRepository.GetAll(databaseConnectionParams).ToDictionary(x => x.ScriptId);
            List<Data.Models.Script> scripts = repo.GetAllScripts().ToList();

            if (revisionsByScriptId.Any())
            {
                foreach (var s in scripts.ToList())
                {
                    if (revisionsByScriptId.ContainsKey(s.ScriptId))
                    {
                        scripts.Remove(s);
                    }
                }
            }
            return scripts.OrderBy(x => x.ScriptDate).ToList();
        }

        public Contracts.DatabaseScriptStates GetDatabaseScriptState(Data.Models.DatabaseConnectionParameters databaseConnectionParams, string scriptContainerPath)
        {
            var repo = _scriptRepoFactory.GetScriptsRepository(scriptContainerPath);
            var revisionsByScriptId = _revisionRepository.GetAll(databaseConnectionParams).ToDictionary(x => x.ScriptId);
            var scriptsById = repo.GetAllScripts().ToDictionary(x => x.ScriptId);

            //first priorty, does the database have revisions that are not in the script file?  if so, that's bad and is the highest priority
            if (revisionsByScriptId.Any(x => !scriptsById.ContainsKey(x.Key)))
            {
                return Contracts.DatabaseScriptStates.Newer;
            }

            //next, are there scripts that are not in the DB?  if so, the the database needs to run revisions
            if (scriptsById.Any(x => !revisionsByScriptId.ContainsKey(x.Key)))
            {
                return Contracts.DatabaseScriptStates.OutOfdate;
            }

            //ok, all that's left then is that they are equal, so it's up to date

            return Contracts.DatabaseScriptStates.UpToDate;
        }

        public Dto.ActionResult TestScriptContainerExists(string scriptContainerPath)
        {
            if (_fileSystem.File.Exists(scriptContainerPath) || _fileSystem.Directory.Exists(scriptContainerPath))
            {
                return Dto.ActionResult.SuccessResult();
            }
            else
            {
                return Dto.ActionResult.FailedResult("Script Container does not exist");
            }
        }

        public Dto.ActionResult TryCreateScriptContainer(string scriptContainerPath)
        {
            var fle = _fileSystem.FileInfo.FromFileName(scriptContainerPath);
            if (fle.Exists)
            {
                return Dto.ActionResult.FailedResult("The file already exists");
            }
            else
            {
                try
                {
                    if (!fle.Directory.Exists)
                    {
                        fle.Directory.Create();
                    }

                    using (var stream = _fileSystem.File.Create(scriptContainerPath))
                    {

                    }
                }
                catch
                {
                    return Dto.ActionResult.FailedResult("Failed to create Script Container file");
                }

                return Dto.ActionResult.SuccessResult();
            }
        }

        public async Task<Dto.ActionResult> TestServerConnectionAsync(Data.Models.ServerConnectionParameters connectionParameters)
        {
            return await this.TestServerConnectionAsync(connectionParameters, System.Threading.CancellationToken.None);
        }

        public async Task<Dto.ActionResult> TestServerConnectionAsync(Data.Models.ServerConnectionParameters connectionParameters, System.Threading.CancellationToken cancellationToken)
        {
            var connString = connectionParameters.GetConnectionString();
            try
            {
                using (var sqlCon = new System.Data.SqlClient.SqlConnection(connString))
                {
                    await sqlCon.OpenAsync(cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        return Dto.ActionResult.FailedResult("User Cancelled");
                    else
                        return Dto.ActionResult.SuccessResult();
                }
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Dto.ActionResult.FailedResult("User Cancelled");
                else
                    return Dto.ActionResult.FailedResult(ex.Message);
            }
        }
        public async Task<Dto.ActionResult> TestDatabaseConnectionAsync(Data.Models.DatabaseConnectionParameters databaseConnectionParams)
        {
            return await this.TestDatabaseConnectionAsync(databaseConnectionParams, System.Threading.CancellationToken.None);

        }
        public async Task<Dto.ActionResult> TestDatabaseConnectionAsync(Data.Models.DatabaseConnectionParameters databaseConnectionParams, System.Threading.CancellationToken cancellationToken)
        {
            var connString = databaseConnectionParams.GetConnectionString();
            try
            {
                using (var sqlCon = new System.Data.SqlClient.SqlConnection(connString))
                {
                    await sqlCon.OpenAsync(cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        return Dto.ActionResult.FailedResult("User Cancelled");
                    else
                        return Dto.ActionResult.SuccessResult();
                }
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Dto.ActionResult.FailedResult("User Cancelled");
                else
                    return Dto.ActionResult.FailedResult(ex.Message);
            }
        }
    }
}
