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
                                                                            ScriptContainerPath: ""C:\\Code\\MyProject\\Database\\Scripts\\DBScripts_SampleDatabase.xml"",
                                                                        },
                                                                        {
                                                                            ContainerUid: ""22222222-2222-2222-2222-222222222222"",
                                                                            DatabaseName: ""NorthWind"",
                                                                            ScriptContainerPath: ""C:\\Microsoft\\Database\\DBScripts_Northwind.xml"",
                                                                        },
                                                                        {
                                                                            ContainerUid: ""33333333-3333-3333-3333-333333333333"",
                                                                            DatabaseName: ""CreditCardInfo"",
                                                                            ScriptContainerPath: ""C:\\Microsoft\\Database\\DBScripts_CreditCardInfo.xml"",
                                                                        }
                                                                 ]
                                                }";

        private ScriptContainerRepository _repo;
        private System.IO.Abstractions.TestingHelpers.MockFileSystem _mockFS = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
        private Mock<Services.Contracts.IEventNotificationService> _mockENS = new Mock<Services.Contracts.IEventNotificationService>();

        [TestInitialize]
        public void Init()
        {
            _repo = new ScriptContainerRepository(_mockFS,
                _mockENS.Object
                );
        }

        [TestMethod()]
        public void GetAllTest()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var results = _repo.GetAll().ToList();

            results.Count.Should().Be(3);
            results[0].DatabaseName.Should().Be("SampleData");
            results[0].ScriptContainerPath.Should().Be(@"C:\Code\MyProject\Database\Scripts\DBScripts_SampleDatabase.xml");

            results[1].DatabaseName.Should().Be("NorthWind");
            results[1].ScriptContainerPath.Should().Be(@"C:\Microsoft\Database\DBScripts_Northwind.xml");

            results[2].DatabaseName.Should().Be("CreditCardInfo");
            results[2].ScriptContainerPath.Should().Be(@"C:\Microsoft\Database\DBScripts_CreditCardInfo.xml");

        }

        [TestMethod()]
        public void GetAll_succeeds_when_file_is_empty()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(string.Empty));


            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var results = _repo.GetAll().ToList();

            results.Count.Should().Be(0);
        }

        [TestMethod()]
        public void AddNew_fails_when_database_name_already_exists()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));


            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var result = _repo.AddNew(databaseName: "saMPledaTA", scriptContainerPath: @"C:\temp\myscript.xml", customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeFalse();
            result.Message.Should().Be("Database already in the list");
        }

        [TestMethod()]
        public void AddNew_allows_same_database_for_different_server()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));


            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            string path = "C:\\Microsoft\\Database\\DBScripts_Northwind.xml";
            var result = _repo.AddNew(databaseName: "Northwind", scriptContainerPath: path,
                customConnectionParameters: new Models.ServerConnectionParameters() { Server = "otherserver" },
                tags: null);

            result.WasSuccessful.Should().BeTrue();
            _repo.GetAll().Where(d => d.DatabaseName.Equals("Northwind", StringComparison.CurrentCultureIgnoreCase))
                .Count().Should().Be(2);

            result = _repo.AddNew(databaseName: "Northwind", scriptContainerPath: path,
                customConnectionParameters: new Models.ServerConnectionParameters() { Server = "anotherotherserver" },
                tags: null);

            result.WasSuccessful.Should().BeTrue();
            _repo.GetAll().Where(d => d.DatabaseName.Equals("Northwind", StringComparison.CurrentCultureIgnoreCase))
                .Count().Should().Be(3);
        }

        [TestMethod()]
        public void AddNew_allows_same_script_file_for_different_database()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            string path = "C:\\Microsoft\\Database\\DBScripts_Northwind.xml";
            var result = _repo.AddNew(databaseName: "MyTestDB", scriptContainerPath: path, customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeTrue();
            _repo.GetAll().Where(d => d.ScriptContainerPath.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                .Count().Should().Be(2);
        }

        [TestMethod()]
        [DataRow(_validSettingsJson)]
        [DataRow("")]
        public void AddNew_succeeds(string settingsJson)
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(settingsJson));


            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var result = _repo.AddNew(databaseName: "MyTestDB", scriptContainerPath: @"C:\temp\myscript.xml", customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeTrue();
            var contents = _mockFS.File.ReadAllText(@"c:\temp\myfolder\myfile.json");
            contents.Should().Contain("\"DatabaseName\": \"MyTestDB\"");
            contents.Should().Contain("\"ScriptContainerPath\": \"C:\\\\temp\\\\myscript.xml\"");

            // ensure notify is called
            _mockENS.Verify(m => m.NotifyScriptContainerAdded(It.IsAny<Models.ScriptContainer>()), Times.Once);
        }

        [TestMethod()]
        public void Update_succeeds()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));


            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var container = new Models.ScriptContainer()
            {
                ContainerUid = new System.Guid("11111111-1111-1111-1111-111111111111"),
                DatabaseName = "MyUpdatedDB",
                ScriptContainerPath = @"c:\temp\updated.xml",
                Tags = new List<string>() { "A", "B", "C" }
            };

            var result = _repo.Update(container);

            result.WasSuccessful.Should().BeTrue();
            var contents = _mockFS.File.ReadAllText(@"c:\temp\myfolder\myfile.json");
            contents.Should().Contain("\"ContainerUid\": \"11111111-1111-1111-1111-111111111111\"");
            contents.Should().Contain("\"DatabaseName\": \"MyUpdatedDB\"");
            contents.Should().Contain("\"ScriptContainerPath\": \"c:\\\\temp\\\\updated.xml\"");

            contents.Should().NotContain("\"DatabaseName\": \"MyTestDB\"");
            contents.Should().NotContain("\"ScriptContainerPath\": \"C:\\\\temp\\\\myscript.xml\"");

            // ensure notify is called
            _mockENS.Verify(m => m.NotifyScriptContainerUpdated(It.IsAny<Models.ScriptContainer>()), Times.Once);
        }

        [TestMethod()]
        public void AddNew_assigns_guid()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(string.Empty));

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            string path = "C:\\Microsoft\\Database\\DBScripts_Northwind.xml";
            var result = _repo.AddNew(databaseName: "MyTestDB", scriptContainerPath: path, customConnectionParameters: null, tags: null);

            result.WasSuccessful.Should().BeTrue();
            _repo.GetAll().Single().ContainerUid.Should().NotBeEmpty();
        }

        [TestMethod()]
        public void Remove_succeeds()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));


            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var result = _repo.Remove(new System.Guid("11111111-1111-1111-1111-111111111111"));

            result.WasSuccessful.Should().BeTrue();
            var contents = _mockFS.File.ReadAllText(@"c:\temp\myfolder\myfile.json").ToLower();
            contents.Should().NotContain("sampledata");
            contents.Should().NotContain("dbscripts_sampledatabase.xml");

            // ensure notify is called
            _mockENS.Verify(m => m.NotifyScriptContainerRemoved(It.IsAny<Models.ScriptContainer>()), Times.Once);
        }

        [TestMethod()]
        public void Remove_fails_when_database_not_in_list()
        {
            _mockFS.AddFile(@"c:\temp\myfolder\myfile.json", new System.IO.Abstractions.TestingHelpers.MockFileData(_validSettingsJson));

            _repo.ConfigurationFileName = @"c:\temp\myfolder\myfile.json";

            var result = _repo.Remove(new System.Guid("99999999-9999-9999-9999-999999999999"));

            result.WasSuccessful.Should().BeFalse();
            result.Message.Should().Be("Database not in the list");
        }
    }
}