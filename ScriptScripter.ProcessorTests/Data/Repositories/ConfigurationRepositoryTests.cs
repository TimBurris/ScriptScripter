using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;

namespace ScriptScripter.Processor.Data.Repositories.Tests
{
    [TestClass()]
    public class ConfigurationRepositoryTests
    {
        private const string _validSettingsJson = @"{
                                                DeveloperName: ""Dumpster Ninja"",
                                                ServerConnectionInfo :{
                                                                            UseTrustedConnection: true,
                                                                            Server: ""(local)\\myInstance"",
                                                                            Username: ""myuser"",
                                                                            Password: ""this_is_my_encrypted_pw""
                                                                        }
                                                }";

        private ConfigurationRepository _repo;
        private System.IO.Abstractions.TestingHelpers.MockFileSystem _mockFS;
        private Mock<Services.Contracts.IEventNotificationService> _mockENS = new Mock<Services.Contracts.IEventNotificationService>();
        private Mock<Services.Contracts.ICryptoService> _mockCryptoService = new Mock<Services.Contracts.ICryptoService>();

        [TestInitialize]
        public void Init()
        {
            _mockFS = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
            _repo = new ConfigurationRepository(_mockFS,
                _mockENS.Object,
                _mockCryptoService.Object
                );
        }

        [TestMethod()]
        public void GetDeveloperNameTest()
        {


            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var name = _repo.GetDeveloperName();

            name.Should().Be("Dumpster Ninja");
        }

        [TestMethod()]
        [DataRow(_validSettingsJson)]
        [DataRow("")]
        public void SetDeveloperNameTest(string settingsJson)
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(settingsJson));

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            _repo.SetDeveloperName("Sam Harris");

            var contents = _mockFS.File.ReadAllText(@"c:\temp\myfolder\myfile.json");

            contents.Should().Contain("\"DeveloperName\": \"Sam Harris\",");
        }

        [TestMethod()]
        public void GetServerConnectionParametersTest()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));
            _mockCryptoService.Setup(m => m.Decrypt("this_is_my_encrypted_pw"))
                .Returns("my_Decrypted_pw");

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var p = _repo.GetServerConnectionParameters();

            p.Server.Should().Be("(local)\\myInstance");
            p.Username.Should().Be("myuser");
            p.UseTrustedConnection.Should().BeTrue();
            p.Password.Should().Be("my_Decrypted_pw");
        }

        [TestMethod()]
        [DataRow(_validSettingsJson)]
        [DataRow("")]
        public void SetServerConnectionParametersTest(string settingsJson)
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(settingsJson));
            _mockCryptoService.Setup(m => m.Encrypt("MyPass"))
                .Returns("super_unbreakable_pw");

            //var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var p = new Models.ServerConnectionParameters();
            p.Server = "MyServer";
            p.Username = "sa";
            p.Password = "MyPass";
            p.UseTrustedConnection = true;

            _repo.SetServerConnectionParameters(p);

            var contents = _mockFS.File.ReadAllText(@"c:\temp\myfolder\myfile.json");

            contents.Should().Contain("\"Server\": \"MyServer\"");
            contents.Should().Contain("\"Username\": \"sa\"");
            contents.Should().Contain("\"Password\": \"super_unbreakable_pw\"");
            contents.Should().Contain("\"UseTrustedConnection\": true");

            // ensure notify is called
            _mockENS.Verify(m => m.NotifyServerConnectionChanged(), Times.Once);
        }

    }
}