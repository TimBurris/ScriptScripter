using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.IntegrationTests
{
    public abstract class DatabaseTestingBase
    {
        protected Data.Models.ServerConnectionParameters _serverConnectionParams;
        protected Data.Models.DatabaseConnectionParameters _databaseConnectionParams;

        [TestInitialize]
        public virtual void BaseInit()
        {
            //TODO: pull this from a config file
            _serverConnectionParams = new Data.Models.ServerConnectionParameters()
            {
                Server = "(local)",
                UseTrustedConnection = true,
            };
            _databaseConnectionParams = new Data.Models.DatabaseConnectionParameters(_serverConnectionParams)
            {
                DatabaseName = "SSTest"
            };

            this.CreateTestDB();
        }

        [TestCleanup]
        public virtual void BaseCleanup()
        {
            this.KillTestDB();
        }

        //just incase the developer changes the DB name in a test, when we KILL, we want to kill what we created, regardless of what the params say
        private string _createdDbName;
        protected virtual void CreateTestDB()
        {
            _createdDbName = _databaseConnectionParams.DatabaseName;

            //need to user SERVERconnection string, can't have the connectionstring include dbname yet cuz.... um.. we have not created it!
            DatabaseTestHelpers.CreateTestDB(_createdDbName, _serverConnectionParams.GetConnectionString());
        }

        protected virtual void KillTestDB()
        {
            //need to user SERVERconnection string, can't have the connectionstring include dbname yet cuz.... um.. we have not created it!
            DatabaseTestHelpers.KillTestDB(_createdDbName, _serverConnectionParams.GetConnectionString());
        }

        protected bool ExecuteExistsSql(string sql)
        {
            return DatabaseTestHelpers.ExecuteBooleanSql($"SELECT 1 WHERE EXISTS ({sql})", _databaseConnectionParams.GetConnectionString());
        }

        protected bool ExecuteBooleanSql(string sql)
        {
            return DatabaseTestHelpers.ExecuteBooleanSql(sql, _databaseConnectionParams.GetConnectionString());
        }

        protected void ExecuteNonQuery(string sql)
        {
            DatabaseTestHelpers.ExecuteNonQuery(sql, _databaseConnectionParams.GetConnectionString());
        }
    }
}
