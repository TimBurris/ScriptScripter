using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ScriptScripter.Processor.Data.Repositories;

namespace ScriptScripter.Processor.IntegrationTests.Repositories
{
    [TestClass]
    public class RevisionRepositoryTests
    {
        //i don't have any tests here, because i have 2 ways i need to test, 
        //  one is when the scripting table exists, the other is when it does not, so i use the 2 classes below to test in each scenario
    }


    [TestClass]
    public class given_table_exists : DatabaseTestingBase
    {

        private Processor.Services.DatabaseUpdater _databaseUpdater;

        [TestInitialize]
        public void Init()
        {
            _databaseUpdater = new Processor.Services.DatabaseUpdater();
            _databaseUpdater.DatabaseConnectionParams = _databaseConnectionParams;
            _databaseUpdater.CreateScriptingSupportObjects();
        }

        [TestMethod]
        public void IsUnderControl_returns_true()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();

            /*************    act    ******************/
            var result = repo.IsUnderControl(_databaseConnectionParams);

            /*************  assert   ******************/
            result.Should().BeTrue();
        }

        #region GetLastRevision Tests

        [TestMethod]
        public void GetLastRevision_returns_only_record()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();
            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                RevisionNumber = 87,
                ScriptDate = new DateTime(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SQLStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");


            /*************    act    ******************/
            var result = repo.GetLastRevision(_databaseConnectionParams);

            /*************  assert   ******************/
            result.RevisionId.Should().BeGreaterThan(0);
            result.RevisionNumber.Should().Be(87);
            result.ScriptDate.Should().Be(new DateTime(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55));
            result.ScriptDeveloperName.Should().Be("Richard Dawkins");
            result.ScriptNotes.Should().Be("here are some notes");
            result.SQLStatement.Should().Be("select 1");
            result.RunByDeveloperName.Should().Be("Sam Harris");
            result.RunDate.Should().BeCloseTo(DateTime.Now, precision: 10000);
            result.RunOnMachineName.Should().Be(Environment.MachineName);
        }

        [TestMethod]
        public void GetLastRevision_returns_last_by_revisionid()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();
            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                RevisionNumber = 1,
                ScriptDate = DateTime.Now.AddMilliseconds(-87494654),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SQLStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");

            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                RevisionNumber = 2,
                ScriptDate = DateTime.Now.AddMilliseconds(-87494654),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SQLStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");

            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                RevisionNumber = 3,
                ScriptDate = DateTime.Now.AddMilliseconds(-87494654),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SQLStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");

            /*************    act    ******************/
            var result = repo.GetLastRevision(_databaseConnectionParams);

            /*************  assert   ******************/
            result.RevisionNumber.Should().Be(3);
        }

        [TestMethod]
        public void GetLastRevision_returns_null_when_no_recs()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();

            /*************    act    ******************/
            var result = repo.GetLastRevision(_databaseConnectionParams);

            /*************  assert   ******************/
            result.Should().BeNull();
        }

        #endregion
    }

    [TestClass]
    public class given_table_does_not_exist : DatabaseTestingBase
    {

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void GetLastRevision_returns_null()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();

            /*************    act    ******************/
            var result = repo.GetLastRevision(_databaseConnectionParams);

            /*************  assert   ******************/
            result.Should().BeNull();
        }

        [TestMethod]
        public void IsUnderControl_returns_false()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();

            /*************    act    ******************/
            var result = repo.IsUnderControl(_databaseConnectionParams);

            /*************  assert   ******************/
            result.Should().BeFalse();
        }
    }
}
