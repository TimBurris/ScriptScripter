using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace ScriptScripter.Processor.IntegrationTests.Services
{
    [TestClass]
    public class ScriptingServiceTests : DatabaseTestingBase
    {
        private ScriptScripter.Processor.Services.ScriptingService _service;
        [TestInitialize]
        public void Init()
        {
            _service = new ScriptScripter.Processor.Services.ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null, fileSystem: null);
        }

        [TestMethod]
        public void TestDatabaseConnectionAsync_succeeds()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var tresult = _service.TestDatabaseConnectionAsync(_databaseConnectionParams);
            tresult.Wait();
            var result = tresult.Result;

            /*************  assert   ******************/
            result.WasSuccessful.Should().BeTrue();
        }
        [TestMethod]
        public void TestDatabaseConnectionAsync_fails_when_db_does_not_exist()
        {
            /*************  arrange  ******************/
            _databaseConnectionParams.DatabaseName = "IHopeThisDBDoesNotExist";

            /*************    act    ******************/
            var tresult = _service.TestDatabaseConnectionAsync(_databaseConnectionParams);
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

            /*************    act    ******************/
            var tresult = _service.TestServerConnectionAsync(_serverConnectionParams);
            tresult.Wait();
            var result = tresult.Result;

            /*************  assert   ******************/
            result.WasSuccessful.Should().BeTrue();
        }


        [TestMethod]
        public void TestServerConnectionAsync_fails_when_user_pass_are_wrong()
        {
            /*************  arrange  ******************/
            var badParams = new Data.Models.ServerConnectionParameters()
            {
                Server = _serverConnectionParams.Server,
                UseTrustedConnection = false,
                Username = "a bad user name", //i'm counting on you not having this user name.  if you do... you prolly need therapy
            };

            /*************    act    ******************/
            var tresult = _service.TestServerConnectionAsync(badParams);
            tresult.Wait();
            var result = tresult.Result;

            /*************  assert   ******************/
            result.WasSuccessful.Should().BeFalse();
            result.Message.Should().Be("Login failed for user 'a bad user name'.");
        }
    }
}
