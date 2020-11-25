using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services
{
    public class DatabaseUpdater : Contracts.IDatabaseUpdater, IDisposable
    {
        private Microsoft.SqlServer.Management.Smo.Server _server;
        private Microsoft.SqlServer.Management.Smo.Server ConnectedServer
        {
            get
            {
                if (_server == null)
                {
                    //just aliasing for simple shorthand use
                    var connParams = this.DatabaseConnectionParams;

                    _server = new Microsoft.SqlServer.Management.Smo.Server();
                    _server.ConnectionContext.LoginSecure = connParams.UseTrustedConnection;
                    _server.ConnectionContext.ServerInstance = connParams.Server;
                    if (!connParams.UseTrustedConnection)
                    {
                        _server.ConnectionContext.Password = connParams.Password;
                        _server.ConnectionContext.Login = connParams.Username;
                    }

                    _server.ConnectionContext.Connect();
                }

                return _server;
            }
        }

        private Microsoft.SqlServer.Management.Smo.Database ConnectedDatabase
        {
            get
            {
                return this.ConnectedServer.Databases[name: this.DatabaseConnectionParams.DatabaseName];
            }
        }

        public Data.Models.DatabaseConnectionParameters DatabaseConnectionParams { get; set; }

        public void BeginTransaction()
        {
            this.ConnectedServer.ConnectionContext.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.ConnectedServer.ConnectionContext.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            this.ConnectedServer.ConnectionContext.RollBackTransaction();
        }

        public void CreateScriptingSupportObjects()
        {
            string sql = @"
                            /* first create our ScriptScripter schema if we don't already have it */
                            if NOT EXISTS (select * from sys.schemas where name='ScriptScripter')
                             begin
	                            -- have to use an exec statement because create schema must be the first statement in a batch
	                            exec ( 'CREATE SCHEMA ScriptScripter' );
                             end

                            /* our table to log what revisions have been applied and by who */
                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ScriptScripter].[AppliedRevision]') AND type in (N'U'))
                            BEGIN
	                            CREATE TABLE [ScriptScripter].[AppliedRevision] (
		                            [ScriptId] [Uniqueidentifier] NOT NULL DEFAULT(newid()),
		                            [SqlStatement] [varchar](MAX),
		                           
                                    [ScriptDeveloperName] [varchar](255),
		                            [ScriptNotes] [varchar](MAX),
		                            [ScriptDate] [datetimeoffset](7) NOT NULL ,
		
		                            [RunByDeveloperName] [varchar](255),
		                            [RunOnMachineName] [varchar](255),
		                            [RunDate] [datetimeoffset](7) NOT NULL ,

		                            CONSTRAINT [PK_ScriptScripter_AppliedRevision] PRIMARY KEY  CLUSTERED 
		                            (
			                            [ScriptId]
		                            )
	                            ) ;
                            END";

            ConnectedDatabase.ExecuteNonQuery(sql);
        }

        public void LogScript(Data.Models.Script script, string executedByDeveloperName)
        {
            string scriptId = script.ScriptId.ToString();
            string sqlStatement = SqlSafeString(script.SqlStatement);

            string scriptDeveloperName = SqlSafeString(script.DeveloperName, maxLength: 255);
            string scriptNotes = SqlSafeString(script.Notes);
            string scriptDate = script.ScriptDate.ToString();

            string runByDeveloperName = SqlSafeString(executedByDeveloperName, maxLength: 255);
            string runOnMachineName = SqlSafeString(Environment.MachineName, maxLength: 255);
            string runDate = DateTimeOffset.Now.ToString();

            //why am i not worries about sql injection?  well because this whole tool is you executing whatever you want against the database.  
            //    therefore, you don't need a sql injection to F things up, just freaking run whatever you want
            string sql = $@"
                    INSERT INTO [ScriptScripter].[AppliedRevision]
                        ([ScriptId]
                        ,[SqlStatement]
                        ,[ScriptDeveloperName]
                        ,[ScriptNotes]
                        ,[ScriptDate]
                        ,[RunByDeveloperName]
                        ,[RunOnMachineName]
                        ,[RunDate])
                     VALUES
                           ('{scriptId}'
                           ,'{sqlStatement}'
                           ,'{scriptDeveloperName}'
                           ,'{scriptNotes}'
                           ,'{scriptDate}'
                           ,'{runByDeveloperName}'
                           ,'{runOnMachineName}'
                           ,'{runDate}')
                ";

            //NOTE: we need this run here on our SMO connection, otherwise for the transaction to work, we'll be requireing MSDTC (if the server is not local)
            this.ConnectedDatabase.ExecuteNonQuery(sql);
        }

        public void RunScript(Data.Models.Script script)
        {
            this.ConnectedDatabase.ExecuteNonQuery(script.SqlStatement);
        }

        private string TruncateString(string value, int length)
        {
            if (value == null || value.Length <= length)
                return value;
            else
                return value.Substring(startIndex: 0, length: length);
        }

        private string SqlSafeString(string sql, int? maxLength = null)
        {
            if (string.IsNullOrEmpty(sql))
                return sql;

            if (maxLength.HasValue)
                sql = this.TruncateString(sql, maxLength.Value);


            return sql.Replace("'", "''");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_server != null)
                    {
                        try
                        {
                            if (_server.ConnectionContext.IsOpen)
                                _server.ConnectionContext.Disconnect();
                        }
                        catch { }
                        _server = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
