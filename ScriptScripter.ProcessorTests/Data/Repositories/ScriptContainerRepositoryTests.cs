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
    public class ScriptContainerRepositoryTests
    {
        private const string _validSettingsJson = @"{
                                                DeveloperName: ""Dumpster Ninja"",
                                                ServerConnectionInfo :{
                                                                            UseTrustedConnection: true,
                                                                            Server: ""(local)\\myInstance"",
                                                                            Username: ""myuser"",
                                                                            Password: ""this_is_my_encrypted_pw""
                                                                        },
                                                ScriptContainers: [
                                                                        {
                                                                            ContainerUid: ""11111111-1111-1111-1111-111111111111"",
                                                                            DatabaseName: ""SampleData"",
                                                                            ScriptFilePath: ""C:\\Code\\MyProject\\Database\\Scripts\\DBScripts_SampleDatabase.xml"",
                                                                        },
                                                                        {
                                                                            ContainerUid: ""22222222-2222-2222-2222-222222222222"",
                                                                            DatabaseName: ""NorthWind"",
                                                                            ScriptFilePath: ""C:\\Microsoft\\Database\\DBScripts_Northwind.xml"",
                                                                        },
                                                                        {
                                                                            ContainerUid: ""33333333-3333-3333-3333-333333333333"",
                                                                            DatabaseName: ""CreditCardInfo"",
                                                                            ScriptFilePath: ""C:\\Microsoft\\Database\\DBScripts_CreditCardInfo.xml"",
                                                                        }
                                                                 ]
                                                }";

        [TestMethod()]
        public void GetAllTest()
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");

            var results = repo.GetAll().ToList();

            results.Count.Should().Be(3);
            results[0].DatabaseName.Should().Be("SampleData");
            results[0].ScriptFilePath.Should().Be(@"C:\Code\MyProject\Database\Scripts\DBScripts_SampleDatabase.xml");

            results[1].DatabaseName.Should().Be("NorthWind");
            results[1].ScriptFilePath.Should().Be(@"C:\Microsoft\Database\DBScripts_Northwind.xml");

            results[2].DatabaseName.Should().Be("CreditCardInfo");
            results[2].ScriptFilePath.Should().Be(@"C:\Microsoft\Database\DBScripts_CreditCardInfo.xml");

        }

        [TestMethod()]
        public void GetAll_succeeds_when_file_is_empty()
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(string.Empty) },
                                });

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");

            var results = repo.GetAll().ToList();

            results.Count.Should().Be(0);
        }

        [TestMethod()]
        public void AddNew_fails_when_database_name_already_exists()
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");

            var result = repo.AddNew(databaseName: "saMPledaTA", scriptFilePath: @"C:\temp\myscript.xml", customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeFalse();
            result.Message.Should().Be("Database already in the list");
        }

        [TestMethod()]
        public void AddNew_allows_same_database_for_different_server()
        {
            var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.EventNotificationService = mockEventService.Object;

            string path = "C:\\Microsoft\\Database\\DBScripts_Northwind.xml";
            var result = repo.AddNew(databaseName: "Northwind", scriptFilePath: path,
                customConnectionParameters: new Models.ServerConnectionParameters() { Server = "otherserver" },
                tags: null);

            result.WasSuccessful.Should().BeTrue();
            repo.GetAll().Where(d => d.DatabaseName.Equals("Northwind", StringComparison.CurrentCultureIgnoreCase))
                .Count().Should().Be(2);

            result = repo.AddNew(databaseName: "Northwind", scriptFilePath: path,
                customConnectionParameters: new Models.ServerConnectionParameters() { Server = "anotherotherserver" },
                tags: null);

            result.WasSuccessful.Should().BeTrue();
            repo.GetAll().Where(d => d.DatabaseName.Equals("Northwind", StringComparison.CurrentCultureIgnoreCase))
                .Count().Should().Be(3);
        }

        [TestMethod()]
        public void AddNew_allows_same_script_file_for_different_database()
        {
            var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.EventNotificationService = mockEventService.Object;

            string path = "C:\\Microsoft\\Database\\DBScripts_Northwind.xml";
            var result = repo.AddNew(databaseName: "MyTestDB", scriptFilePath: path, customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeTrue();
            repo.GetAll().Where(d => d.ScriptFilePath.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                .Count().Should().Be(2);
        }

        [TestMethod()]
        [DataRow(_validSettingsJson)]
        [DataRow("")]
        public void AddNew_succeeds(string settingsJson)
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(settingsJson) },
                                });
            var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.EventNotificationService = mockEventService.Object;

            var result = repo.AddNew(databaseName: "MyTestDB", scriptFilePath: @"C:\temp\myscript.xml", customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeTrue();
            var contents = fileSystem.File.ReadAllText(@"c:\temp\myfolder\myfile.json");
            contents.Should().Contain("\"DatabaseName\": \"MyTestDB\"");
            contents.Should().Contain("\"ScriptFilePath\": \"C:\\\\temp\\\\myscript.xml\"");

            // ensure notify is called
            mockEventService.Verify(m => m.NotifyScriptContainerAdded(It.IsAny<Models.ScriptContainer>()), Times.Once);
        }

        [TestMethod()]
        public void Update_succeeds()
        {
            string settingsJson = _validSettingsJson;
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(settingsJson) },
                                });
            var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.EventNotificationService = mockEventService.Object;

            var container = new Models.ScriptContainer()
            {
                ContainerUid = new System.Guid("11111111-1111-1111-1111-111111111111"),
                DatabaseName = "MyUpdatedDB",
                ScriptFilePath = @"c:\temp\updated.xml",
                Tags = new List<string>() { "A", "B", "C" }
            };

            var result = repo.Update(container);

            result.WasSuccessful.Should().BeTrue();
            var contents = fileSystem.File.ReadAllText(@"c:\temp\myfolder\myfile.json");
            contents.Should().Contain("\"ContainerUid\": \"11111111-1111-1111-1111-111111111111\"");
            contents.Should().Contain("\"DatabaseName\": \"MyUpdatedDB\"");
            contents.Should().Contain("\"ScriptFilePath\": \"c:\\\\temp\\\\updated.xml\"");

            contents.Should().NotContain("\"DatabaseName\": \"MyTestDB\"");
            contents.Should().NotContain("\"ScriptFilePath\": \"C:\\\\temp\\\\myscript.xml\"");

            // ensure notify is called
            mockEventService.Verify(m => m.NotifyScriptContainerUpdated(It.IsAny<Models.ScriptContainer>()), Times.Once);
        }

        [TestMethod()]
        public void AddNew_assigns_guid()
        {
            var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(string.Empty) },
                                });

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.EventNotificationService = mockEventService.Object;

            string path = "C:\\Microsoft\\Database\\DBScripts_Northwind.xml";
            var result = repo.AddNew(databaseName: "MyTestDB", scriptFilePath: path, customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeTrue();
            repo.GetAll().Single().ContainerUid.Should().NotBeEmpty();
        }

        [TestMethod()]
        public void Remove_succeeds()
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });
            var mockEventService = new Mock<Services.Contracts.IEventNotificationService>();

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");
            repo.EventNotificationService = mockEventService.Object;

            var result = repo.Remove(new System.Guid("11111111-1111-1111-1111-111111111111"));

            result.WasSuccessful.Should().BeTrue();
            var contents = fileSystem.File.ReadAllText(@"c:\temp\myfolder\myfile.json").ToLower();
            contents.Should().NotContain("sampledata");
            contents.Should().NotContain("dbscripts_sampledatabase.xml");

            // ensure notify is called
            mockEventService.Verify(m => m.NotifyScriptContainerRemoved(It.IsAny<Models.ScriptContainer>()), Times.Once);
        }

        [TestMethod()]
        public void Remove_fails_when_database_not_in_list()
        {
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { @"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson) },
                                });

            var repo = new ScriptContainerRepository(fileSystem, configurationFileName: @"c:\temp\myfolder\myfile.json");

            var result = repo.Remove(new System.Guid("99999999-9999-9999-9999-999999999999"));

            result.WasSuccessful.Should().BeFalse();
            result.Message.Should().Be("Database not in the list");
        }
    }
}