using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace ScriptScripter.Processor.IntegrationTests.Services
{
    [TestClass]
    public class ScriptingServiceTests : DatabaseTestingBase
    {
        [TestMethod]
        public void TestDatabaseConnectionAsync_succeeds()
        {
            /*************  arrange  ******************/
            var x = new ScriptScripter.Processor.Services.ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);

            /*************    act    ******************/
            var tresult = x.TestDatabaseConnectionAsync(_databaseConnectionParams);
            tresult.Wait();
            var result = tresult.Result;

            /*************  assert   ******************/
            result.WasSuccessful.Should().BeTrue();
        }
        [TestMethod]
        public void TestDatabaseConnectionAsync_fails_when_db_does_not_exist()
        {
            /*************  arrange  ******************/
            var x = new ScriptScripter.Processor.Services.ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            _databaseConnectionParams.DatabaseName = "IHopeThisDBDoesNotExist";

            /*************    act    ******************/
            var tresult = x.TestDatabaseConnectionAsync(_databaseConnectionParams);
            tresult.Wait();
            var result = tresult.Result;

            /*************  assert   ******************/
            result.WasSuccessful.Should().BeFalse();
            result.Message.Should().Contain("Cannot open database \"IHopeThisDBDoesNotExist\"");
        }

        [TestMethod]
        public void TestServerConnectionAsync_succeeds()
        {
            /*************  arrange  ******************/
            var x = new ScriptScripter.Processor.Services.ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);

            /*************    act    ******************/
            var tresult = x.TestServerConnectionAsync(_serverConnectionParams);
            tresult.Wait();
            var result = tresult.Result;

            /*************  assert   ******************/
            result.WasSuccessful.Should().BeTrue();
        }


        [TestMethod]
        public void TestServerConnectionAsync_fails_when_user_pass_are_wrong()
        {
            /*************  arrange  ******************/
            var x = new ScriptScripter.Processor.Services.ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            var badParams = new Data.Models.ServerConnectionParameters()
            {
                Server = _serverConnectionParams.Server,
                UseTrustedConnection = false,
                Username = "a bad user name", //i'm counting on you not having this user name.  if you do... you prolly need therapy
            };

            /*************    act    ******************/
            var tresult = x.TestServerConnectionAsync(badParams);
            tresult.Wait();
            var result = tresult.Result;

            /*************  assert   ******************/
            result.WasSuccessful.Should().BeFalse();
            result.Message.Should().Be("Login failed for user 'a bad user name'.");
        }
    }
}
