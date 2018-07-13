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


        [TestMethod()]
        public void GetDeveloperNameTest()
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });

            var repo = new ConfigurationRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");

            var name = repo.GetDeveloperName();

            name.Should().Be("Dumpster Ninja");
        }

        [TestMethod()]
        [DataRow(_validSettingsJson)]
        [DataRow("")]
        public void SetDeveloperNameTest(string settingsJson)
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(settingsJson) },
                                });

            var repo = new ConfigurationRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");

            repo.SetDeveloperName("Sam Harris");

            var contents = fileSystem.File.ReadAllText(@"c:\temp\myfolder\myfile.json");

            contents.Should().Contain("\"DeveloperName\": \"Sam Harris\",");
        }

        [TestMethod()]
        public void GetServerConnectionParametersTest()
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });
            var mockCryptoService = new Mock<Services.Contracts.ICryptoService>();
            mockCryptoService.Setup(m => m.Decrypt("this_is_my_encrypted_pw"))
                .Returns("my_Decrypted_pw");

            var repo = new ConfigurationRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.CryptoService = mockCryptoService.Object;

            var p = repo.GetServerConnectionParameters();

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
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(settingsJson) },
                                });
            var mockCryptoService = new Mock<Services.Contracts.ICryptoService>();
            mockCryptoService.Setup(m => m.Encrypt("MyPass"))
                .Returns("super_unbreakable_pw");

            var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();

            var repo = new ConfigurationRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.CryptoService = mockCryptoService.Object;
            repo.EventNotificationService = mockEventService.Object;

            var p = new Models.ServerConnectionParameters();
            p.Server = "MyServer";
            p.Username = "sa";
            p.Password = "MyPass";
            p.UseTrustedConnection = true;

            repo.SetServerConnectionParameters(p);

            var contents = fileSystem.File.ReadAllText(@"c:\temp\myfolder\myfile.json");

            contents.Should().Contain("\"Server\": \"MyServer\"");
            contents.Should().Contain("\"Username\": \"sa\"");
            contents.Should().Contain("\"Password\": \"super_unbreakable_pw\"");
            contents.Should().Contain("\"UseTrustedConnection\": true");

            // ensure notify is called
            mockEventService.Verify(m => m.NotifyServerConnectionChanged(), Times.Once);
        }

    }
}