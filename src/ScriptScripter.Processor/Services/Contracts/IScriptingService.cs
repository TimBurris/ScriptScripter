﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services.Contracts
{
    [Flags]
    public enum DatabaseScriptStates
    {
        UpToDate = 1,
        OutOfdate = 2,
        Newer = 4,
    }

    public interface IScriptingService
    {
        //Future maybe?
        // Data.Models.ActionResult ApplyScriptsToAllDatabases();


        Task<Dto.ActionResult> ApplyScriptsToDatabaseAsync(Data.Models.DatabaseConnectionParameters databaseConnectionParams, IEnumerable<Data.Models.Script> scripts, IProgress<Dto.ApplyScriptProgress> progress);

        DatabaseScriptStates GetDatabaseScriptState(Data.Models.DatabaseConnectionParameters databaseConnectionParams, string scriptContainerPath);

        IEnumerable<Data.Models.Script> GetScriptsThatNeedRun(Data.Models.DatabaseConnectionParameters databaseConnectionParams, string scriptContainerPath);

        Task<Dto.ActionResult> TestDatabaseConnectionAsync(Data.Models.DatabaseConnectionParameters databaseConnectionParams);
        Task<Dto.ActionResult> TestDatabaseConnectionAsync(Data.Models.DatabaseConnectionParameters databaseConnectionParams, System.Threading.CancellationToken cancellationToken);

        Task<Dto.ActionResult> TestServerConnectionAsync(Data.Models.ServerConnectionParameters connectionParameters);
        Task<Dto.ActionResult> TestServerConnectionAsync(Data.Models.ServerConnectionParameters connectionParameters, System.Threading.CancellationToken cancellationToken);

        Dto.ActionResult TestScriptContainerExists(string scriptContainerPath);

        Dto.ActionResult TryCreateScriptContainer(string scriptContainerPath);
    }
}
