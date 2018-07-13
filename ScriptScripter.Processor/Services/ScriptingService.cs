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
        private Data.Contracts.IRevisionRepository _revisionRepository;

        public ScriptingService(Contracts.IDatabaseUpdaterFactory dbupdaterFactory,
            Data.Contracts.IScriptRepositoryFactory scriptRepoFactory,
            Data.Contracts.IRevisionRepository revisionRepository,
            Data.Contracts.IConfigurationRepository configurationRepository)
        {
            _databaseUpdaterFactory = dbupdaterFactory;
            _scriptRepoFactory = scriptRepoFactory;
            _revisionRepository = revisionRepository;
            _configurationRepository = configurationRepository;
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
                                Revision = new Data.Models.Revision() { RevisionNumber = script.RevisionNumber },
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
        public IEnumerable<Data.Models.Script> GetScriptsThatNeedRun(Data.Models.DatabaseConnectionParameters databaseConnectionParams, Processor.Data.Models.ScriptContainer scriptContainer)
        {
            var repo = _scriptRepoFactory.GetScriptsRepository(scriptContainer);
            var lastRevision = _revisionRepository.GetLastRevision(databaseConnectionParams);
            IEnumerable<Data.Models.Script> scripts;

            if (lastRevision == null)
                scripts = repo.GetAllScripts();
            else
                scripts = repo.GetAllScriptsAfterRevisionNumber(lastRevision.RevisionNumber);

            return scripts;
        }

        public Contracts.DatabaseScriptStates GetDatabaseScriptState(Data.Models.Script latestScript, Data.Models.Revision latestRevision)
        {
            if (latestRevision == null && latestScript == null)
                return Contracts.DatabaseScriptStates.UpToDate;
            else if (latestRevision == null)
                return Contracts.DatabaseScriptStates.OutOfdate;
            else if (latestScript == null)
                return Contracts.DatabaseScriptStates.Newer;
            else if (latestScript.RevisionNumber > latestRevision.RevisionNumber)
                return Contracts.DatabaseScriptStates.OutOfdate;
            else if (latestScript.RevisionNumber == latestRevision.RevisionNumber)
                return Contracts.DatabaseScriptStates.UpToDate;
            else
                return Contracts.DatabaseScriptStates.Newer;
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
