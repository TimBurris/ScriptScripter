using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ScriptScripter.Processor.Data.Repositories;
using System.Linq;

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

        #region GetAll Tests

        [TestMethod]
        public void GetAll_returns_all_record()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();
            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                ScriptId = new Guid("7fb37634-3cd7-4237-9b9f-527219ea630a"),
                ScriptDate = new DateTimeOffset(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55, offset: new TimeSpan()),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SqlStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");

            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                ScriptId = new Guid("3d68f8e3-4b37-4a7d-b0b9-6d4c23803dbc"),
                ScriptDate = new DateTimeOffset(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55, offset: new TimeSpan()),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SqlStatement = "select 2"
            }, executedByDeveloperName: "Sam Harris");

            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                ScriptId = new Guid("7672f7fc-ce08-4dfe-83fa-369beb361993"),
                ScriptDate = new DateTimeOffset(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55, offset: new TimeSpan()),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SqlStatement = "select 3"
            }, executedByDeveloperName: "Sam Harris");

            /*************    act    ******************/
            var results = repo.GetAll(_databaseConnectionParams).ToList();

            /*************  assert   ******************/
            results.Count.Should().Be(3);

            var result = results.FirstOrDefault(x => x.ScriptId == new Guid("7fb37634-3cd7-4237-9b9f-527219ea630a"));
            result.Should().NotBeNull();
            result.ScriptId.Should().Be(new Guid("7fb37634-3cd7-4237-9b9f-527219ea630a"));
            result.ScriptDate.Should().Be(new DateTimeOffset(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55, offset: new TimeSpan()));
            result.ScriptDeveloperName.Should().Be("Richard Dawkins");
            result.ScriptNotes.Should().Be("here are some notes");
            result.SqlStatement.Should().Be("select 1");
            result.RunByDeveloperName.Should().Be("Sam Harris");
            result.RunDate.Should().BeCloseTo(DateTime.UtcNow, precision: 10000);
            result.RunOnMachineName.Should().Be(Environment.MachineName);
        }



        #endregion

        #region GetLastRevision Tests

        [TestMethod]
        public void GetLastRevision_returns_only_record()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();
            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                ScriptId = new Guid("7fb37634-3cd7-4237-9b9f-527219ea630a"),
                ScriptDate = new DateTime(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SqlStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");


            /*************    act    ******************/
            var result = repo.GetLastRevision(_databaseConnectionParams);

            /*************  assert   ******************/
            result.ScriptId.Should().Be(new Guid("7fb37634-3cd7-4237-9b9f-527219ea630a"));
            result.ScriptDate.Should().Be(new DateTime(month: 12, day: 31, year: 1999, hour: 11, minute: 59, second: 55));
            result.ScriptDeveloperName.Should().Be("Richard Dawkins");
            result.ScriptNotes.Should().Be("here are some notes");
            result.SqlStatement.Should().Be("select 1");
            result.RunByDeveloperName.Should().Be("Sam Harris");
            result.RunDate.Should().BeCloseTo(DateTime.UtcNow, precision: 10000);
            result.RunOnMachineName.Should().Be(Environment.MachineName);
        }

        [TestMethod]
        public void GetLastRevision_returns_last_by_scriptdate()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();
            var baseDateTime = DateTime.UtcNow;

            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                ScriptId = new Guid("7fb37634-3cd7-4237-9b9f-527219ea630a"),
                ScriptDate = baseDateTime.AddSeconds(-87494654),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SqlStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");

            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                ScriptId = new Guid("3d68f8e3-4b37-4a7d-b0b9-6d4c23803dbc"),
                ScriptDate = baseDateTime.AddSeconds(-50),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SqlStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");

            _databaseUpdater.LogScript(new Data.Models.Script()
            {
                ScriptId = new Guid("7672f7fc-ce08-4dfe-83fa-369beb361993"),
                ScriptDate = baseDateTime.AddSeconds(-100),
                DeveloperName = "Richard Dawkins",
                Notes = "here are some notes",
                SqlStatement = "select 1"
            }, executedByDeveloperName: "Sam Harris");

            /*************    act    ******************/
            var result = repo.GetLastRevision(_databaseConnectionParams);

            /*************  assert   ******************/
            result.ScriptId.Should().Be(new Guid("3d68f8e3-4b37-4a7d-b0b9-6d4c23803dbc"));
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
        public void GetAll_returns_empty()
        {
            /*************  arrange  ******************/
            var repo = new RevisionRepository();

            /*************    act    ******************/
            var result = repo.GetAll(_databaseConnectionParams);

            /*************  assert   ******************/
            result.Should().NotBeNull();
            result.Should().BeEmpty();
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
