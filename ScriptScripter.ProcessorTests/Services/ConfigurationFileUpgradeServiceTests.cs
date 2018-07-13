using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ScriptScripter.Processor.Services.Tests
{
    [TestClass()]
    public class ConfigurationFileUpgradeServiceTests
    {
        private ConfigurationFileUpgradeService _service;
        private System.IO.Abstractions.TestingHelpers.MockFileData _mockFileData;
        private System.IO.Abstractions.TestingHelpers.MockFileSystem _mockFileSystem;
        private const string _fileName = @"c:\temp\myfolder\myfile.json";
        [TestInitialize]
        public void Init()
        {
            _mockFileData = new System.IO.Abstractions.TestingHelpers.MockFileData(string.Empty);
            _mockFileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { _fileName, _mockFileData},
                                });

            _service = new ConfigurationFileUpgradeService(_mockFileSystem, configurationFileName: _fileName);
        }

        private void Act()
        {
            _service.UpgradeFile();
        }

        [TestMethod()]
        public void UpgradeFile_sets_container_guids_Test()
        {
            //*************  arrange  ******************
            _mockFileData.TextContents = @"{
                        DeveloperName: ""Dumpster Ninja"",
                        ServerConnectionInfo :{
                                                    UseTrustedConnection: true,
                                                    Server: ""(local)\\myInstance"",
                                                    Username: ""myuser"",
                                                    Password: ""this_is_my_encrypted_pw""
                                                },
                        ScriptContainers: [
                                                {
                                                    DatabaseName: ""SampleData"",
                                                    ScriptFilePath: ""C:\\Code\\MyProject\\Database\\Scripts\\DBScripts_SampleDatabase.xml"",
                                                },
                                                {
                                                    DatabaseName: ""NorthWind"",
                                                    ScriptFilePath: ""C:\\Microsoft\\Database\\DBScripts_Northwind.xml"",
                                                },
                                                {
                                                    DatabaseName: ""CreditCardInfo"",
                                                    ScriptFilePath: ""C:\\Microsoft\\Database\\DBScripts_CreditCardInfo.xml"",
                                                }
                                            ]
                        }";

            //sanity check, everything should be empty, then we'll upgrade and prove not empty
            var before = new Data.Repositories.ScriptContainerRepository(_mockFileSystem, _fileName)
                .GetAll()
                .ToList();

            before.Should().NotBeEmpty();
            before.ForEach(r => r.ContainerUid.Should().BeEmpty());

            //*************    act    ******************
            this.Act();

            //*************  assert   ******************
            var after = new Data.Repositories.ScriptContainerRepository(_mockFileSystem, _fileName)
                 .GetAll()
                 .ToList();
            after.Should().NotBeEmpty();
            after.ForEach(r => r.ContainerUid.Should().NotBeEmpty());
        }

        [TestMethod()]
        public void UpgradeFile_does_not_change_when_is_current_Test()
        {

            //*************  arrange  ******************
            string originalContents = @"{
                        DeveloperName: ""Dumpster Ninja"",
                        ServerConnectionInfo :{
                                                    UseTrustedConnection: true,
                                                    Server: ""(local)\\myInstance"",
                                                    Username: ""myuser"",
                                                    Password: ""this_is_my_encrypted_pw""
                                                },
                        ScriptContainers: [
                                                {
                                                    ContainerUid: ""B659FE94-2FF2-409C-82ED-7CBBD70EAD38"",
                                                    DatabaseName: ""SampleData"",
                                                    ScriptFilePath: ""C:\\Code\\MyProject\\Database\\Scripts\\DBScripts_SampleDatabase.xml"",
                                                },
                                                {
                                                    ContainerUid: ""9188DE9F-C4FC-400E-B8ED-E6AC892231B9"",
                                                    DatabaseName: ""NorthWind"",
                                                    ScriptFilePath: ""C:\\Microsoft\\Database\\DBScripts_Northwind.xml"",
                                                },
                                                {
                                                    ContainerUid: ""2DD4ABEF-FE59-47B4-86C5-5ECB12B1857C"",
                                                    DatabaseName: ""CreditCardInfo"",
                                                    ScriptFilePath: ""C:\\Microsoft\\Database\\DBScripts_CreditCardInfo.xml"",
                                                }
                                            ]
                        }";

            _mockFileData.TextContents = originalContents;

            //*************    act    ******************
            this.Act();

            //*************  assert   ******************
            _mockFileSystem.File.ReadAllText(_fileName)
                    .Should().Be(originalContents);
        }

        [TestMethod()]
        public void UpgradeFile_does_not_change_when_no_containers()
        {

            //*************  arrange  ******************
            string originalContents = @"{
                        DeveloperName: ""Dumpster Ninja"",
                        ServerConnectionInfo :{
                                                    UseTrustedConnection: true,
                                                    Server: ""(local)\\myInstance"",
                                                    Username: ""myuser"",
                                                    Password: ""this_is_my_encrypted_pw""
                                                },
                        }";

            _mockFileData.TextContents = originalContents;

            //*************    act    ******************
            this.Act();

            //*************  assert   ******************
            _mockFileSystem.File.ReadAllText(_fileName)
                    .Should().Be(originalContents);
        }
    }
}