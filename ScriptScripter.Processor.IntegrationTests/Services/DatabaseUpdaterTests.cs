using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ScriptScripter.Processor.IntegrationTests.Services
{
    [TestClass()]
    public class DatabaseUpdaterTests : DatabaseTestingBase
    {
        private DatabaseUpdater _updater;

        #region Init and Cleanup

        [TestInitialize]
        public void Init()
        {
            _updater = new DatabaseUpdater();
            _updater.DatabaseConnectionParams = _databaseConnectionParams;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _updater?.Dispose(); _updater = null;
        }

        #endregion

        [TestMethod()]
        public void CommitTransactionTest()
        {
            /*************  arrange  ******************/
            var script = new Data.Models.Script()
            {
                SQLStatement = @"create table TestTable(id int);",
            };

            /*************    act    ******************/
            _updater.BeginTransaction();
            _updater.RunScript(script);
            _updater.CommitTransaction();

            /*************  assert   ******************/
            this.ExecuteExistsSql("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestTable]') AND type in (N'U')")
                .Should().BeTrue();
        }

        [TestMethod()]
        public void RollbackTransactionTest()
        {
            /*************  arrange  ******************/
            var script = new Data.Models.Script()
            {
                SQLStatement = @"create table TestTable(id int);",
            };

            /*************    act    ******************/
            _updater.BeginTransaction();
            _updater.RunScript(script);
            _updater.RollbackTransaction();

            /*************  assert   ******************/
            this.ExecuteExistsSql("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestTable]') AND type in (N'U')")
                .Should().BeFalse();
        }

        #region LogScript 

        [TestMethod]
        public void LogScript_adds_record_to_table()
        {
            /*************  arrange  ******************/
            var script = new Data.Models.Script()
            {
                DeveloperName = "Cpt. Jack Sparrow",
                RevisionNumber = 71,
                ScriptDate = DateTime.Now.AddDays(-10),// using a date in the past to prevent false pass if scriptdate and rundate are accidentally misused (i'm making sure they are different)
                SQLStatement = "Select 1",
                Notes = "Some Notes",
            };
            _updater.CreateScriptingSupportObjects();

            /*************    act    ******************/
            _updater.LogScript(script, executedByDeveloperName: "Dumpster Ninja");

            /*************  assert   ******************/
            string connString = _databaseConnectionParams.GetConnectionString();

            using (var con = new System.Data.SqlClient.SqlConnection(connString))
            {
                con.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(@"SELECT [RevisionId]
                                                                              ,[RevisionNumber]
                                                                              ,[SQLStatement]
                                                                              ,[ScriptDeveloperName]
                                                                              ,[ScriptNotes]
                                                                              ,[ScriptDate]
                                                                              ,[RunByDeveloperName]
                                                                              ,[RunOnMachineName]
                                                                              ,[RunDate]
                                                                          FROM [ScriptScripter].[Revision] ", connection: con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        reader.GetInt32(0).Should().BeGreaterThan(0);
                        reader.GetInt32(1).Should().Be(71);
                        reader.GetString(2).Should().Be("Select 1");
                        reader.GetString(3).Should().Be("Cpt. Jack Sparrow");
                        reader.GetString(4).Should().Be("Some Notes");
                        reader.GetDateTime(5).Should().BeCloseTo(script.ScriptDate, precision: 999); //closeto instead of exact because of precision that might get lost
                        reader.GetString(6).Should().Be("Dumpster Ninja");
                        reader.GetString(7).Should().Be(Environment.MachineName);
                        reader.GetDateTime(8).Should().BeCloseTo(DateTime.Now, precision: 5000); //we are expecting there to be less than 5 sends between when we logged and when we hit this line.  if that's not enough, might need a new computer

                    }
                }
            }

        }

        [TestMethod]
        public void LogScript_truncates_script_developername_at_255()
        {
            /*************  arrange  ******************/
            var script = new Data.Models.Script()
            {
                DeveloperName = new string('x', count: 300),
                RevisionNumber = 71,
                ScriptDate = DateTime.Now.AddDays(-10),// using a date in the past to prevent false pass if scriptdate and rundate are accidentally misused (i'm making sure they are different)
                SQLStatement = "Select 1",
                Notes = "Some Notes",
            };
            _updater.CreateScriptingSupportObjects();

            /*************    act    ******************/
            _updater.LogScript(script, executedByDeveloperName: "Dumpster Ninja");

            /*************  assert   ******************/
            string connString = _databaseConnectionParams.GetConnectionString();

            using (var con = new System.Data.SqlClient.SqlConnection(connString))
            {
                con.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(@"SELECT [ScriptDeveloperName]
                                                                          FROM [ScriptScripter].[Revision] ", connection: con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        reader.GetString(0).Should().Be(new string('x', count: 255));
                    }
                }
            }
        }

        [TestMethod]
        public void LogScript_truncates_runby_developername_at_255()
        {
            /*************  arrange  ******************/
            var script = new Data.Models.Script()
            {
                DeveloperName = "Cpt. Jack Sparrow",
                RevisionNumber = 71,
                ScriptDate = DateTime.Now.AddDays(-10),// using a date in the past to prevent false pass if scriptdate and rundate are accidentally misused (i'm making sure they are different)
                SQLStatement = "Select 1",
                Notes = "Some Notes",
            };
            _updater.CreateScriptingSupportObjects();

            /*************    act    ******************/
            _updater.LogScript(script, executedByDeveloperName: new string('z', count: 300));

            /*************  assert   ******************/
            string connString = _databaseConnectionParams.GetConnectionString();

            using (var con = new System.Data.SqlClient.SqlConnection(connString))
            {
                con.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(@"SELECT [RunByDeveloperName]
                                                                          FROM [ScriptScripter].[Revision] ", connection: con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        reader.GetString(0).Should().Be(new string('z', count: 255));
                    }
                }
            }
        }

        //TODO: can't do this test unless we inject the machinename
        //[TestMethod]
        //public void LogScript_truncates_machinename_at_255()
        //{
        //    Assert.Fail();
        //}

        #endregion

        #region RunScript

        [TestMethod()]
        public void RunScript_simple()
        {
            /*************  arrange  ******************/
            var script = new Data.Models.Script()
            {
                SQLStatement = @"create table TestTable(id int);",
            };

            /*************    act    ******************/
            _updater.RunScript(script);

            /*************  assert   ******************/
            this.ExecuteExistsSql("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestTable]') AND type in (N'U')")
                .Should().BeTrue();
        }

        [TestMethod()]
        public void RunScript_with_go()
        {
            /*************  arrange  ******************/
            var script = new Data.Models.Script()
            {
                SQLStatement = @"create table TestTable(id int);
                                    GO
                                    create view v1 as select * from testtable;",
            };

            /*************    act    ******************/
            _updater.RunScript(script);

            /*************  assert   ******************/
            this.ExecuteExistsSql("SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v1]')")
                .Should().BeTrue();
        }

        #endregion

        #region CreateScriptingSupportObjects tests

        [TestMethod()]
        public void CreateScriptingSupportObjects_creates_ScriptScripter_schema()
        {
            /*************  arrange  ******************/


            /*************    act    ******************/
            _updater.CreateScriptingSupportObjects();

            /*************  assert   ******************/
            this.ExecuteExistsSql("select * from sys.schemas where name='ScriptScripter'")
                .Should().BeTrue();
        }

        [TestMethod()]
        public void CreateScriptingSupportObjects_creates_Revision_table()
        {
            /*************  arrange  ******************/


            /*************    act    ******************/
            _updater.CreateScriptingSupportObjects();

            /*************  assert   ******************/
            this.ExecuteExistsSql("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ScriptScripter].[Revision]') AND type in (N'U')")
                .Should().BeTrue();
        }

        #endregion

    }
}