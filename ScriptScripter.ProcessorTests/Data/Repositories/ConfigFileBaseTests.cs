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
    public class ConfigFileBaseTests
    {
        private string _validSettingsJson = @"{
                                                DeveloperName: ""Dumpster Ninja"",
                                                ServerConnectionInfo :{
                                                                            UseTrustedConnection: true,
                                                                            Server: ""(local)\\myInstance"",
                                                                            Username: ""myuser"",
                                                                            Password: ""this_is_my_encrypted_pw""
                                                                        },
                                                }";

        private ConfigurationRepository _repo;
        private Mock<System.IO.Abstractions.IFileSystem> _mockFS = new Mock<System.IO.Abstractions.IFileSystem>(MockBehavior.Strict);
        private Mock<Services.Contracts.IEventNotificationService> _mockENS = new Mock<Services.Contracts.IEventNotificationService>();
        private Mock<Services.Contracts.ICryptoService> _mockCryptoService = new Mock<Services.Contracts.ICryptoService>();

        [TestInitialize]
        public void Init()
        {
            _repo = new ConfigurationRepository(_mockFS.Object,
                _mockENS.Object,
                _mockCryptoService.Object
                );
        }

        [TestMethod]
        public void Repo_ReadsFile_from_settings()
        {

            _mockFS.Setup(m => m.File.Exists(@"F:\ScriptScripter\Test\config.json"))
                .Returns(true);

            _mockFS.Setup(m => m.File.ReadAllText(@"F:\ScriptScripter\Test\config.json"))
                .Returns(_validSettingsJson);

            _mockFS.Setup(m => m.Path.GetFullPath(@"F:\ScriptScripter\Test\config.json"))
                .Returns(@"F:\ScriptScripter\Test\config.json");

            PathVerifyHack(_mockFS);

            var x = _repo.GetDeveloperName();
            _mockFS.VerifyAll();
        }

        [TestMethod]
        public void Repo_resolves_environmentvariables_in_path()
        {
            // programdata %\ScriptScripter\Test\config.json

            _mockFS.Setup(m => m.File.Exists(@"C:\ProgramData\ScriptScripter\config.json"))
                .Returns(true);

            _mockFS.Setup(m => m.File.ReadAllText(@"C:\ProgramData\ScriptScripter\config.json"))
                .Returns(_validSettingsJson);

            _mockFS.Setup(m => m.Path.GetFullPath(@"C:\ProgramData\ScriptScripter\config.json"))
                .Returns(@"C:\ProgramData\ScriptScripter\config.json");

            PathVerifyHack(_mockFS);

            _repo.ConfigurationFileName = @"%programdata%\ScriptScripter\config.json";
            var x = _repo.GetDeveloperName();

            _mockFS.VerifyAll();
        }

        [TestMethod]
        public void Repo_resolves_resolves_relativepath()
        {
            _mockFS.Setup(m => m.File.Exists(@"C:\ScriptScripter\config.json"))
                 .Returns(true);

            _mockFS.Setup(m => m.File.ReadAllText(@"C:\ScriptScripter\config.json"))
                .Returns(_validSettingsJson);

            //here is where we resolve
            _mockFS.Setup(m => m.Path.GetFullPath(@".\ScriptScripter\config.json"))
                .Returns(@"C:\ScriptScripter\config.json");

            PathVerifyHack(_mockFS);

            _repo.ConfigurationFileName = @".\ScriptScripter\config.json";
            var x = _repo.GetDeveloperName();

            _mockFS.VerifyAll();
        }

        [TestMethod]
        public void Repo_creates_folder()
        {
            string fulldirectoryName = @"E:\myFolder\YourFolder\HisFolder\HerFolder\ScriptScripter\";
            string fullFileName = @"E:\myFolder\YourFolder\HisFolder\HerFolder\ScriptScripter\config.json";


            this.MockReadSettings(_mockFS, fullFileName);

            _mockFS.Setup(m => m.Path.GetDirectoryName(fullFileName))
                .Returns(fulldirectoryName);

            _mockFS.Setup(m => m.Directory.Exists(fulldirectoryName))
                .Returns(false);

            _mockFS.Setup(m => m.Directory.CreateDirectory(fulldirectoryName))
                .Returns(new System.IO.Abstractions.DirectoryInfoWrapper(_mockFS.Object,new System.IO.DirectoryInfo(fulldirectoryName)));

            _mockFS.Setup(m => m.File.WriteAllText(fullFileName, It.IsAny<string>()));

            _repo.ConfigurationFileName = fullFileName;
            _repo.SetDeveloperName("Dumpster Ninja");

            _mockFS.VerifyAll();
        }

        private void MockReadSettings(Mock<System.IO.Abstractions.IFileSystem> mockFS, string fullFileName)
        {
            mockFS.Setup(m => m.File.Exists(fullFileName))
               .Returns(true);

            mockFS.Setup(m => m.Path.GetFullPath(fullFileName))
                .Returns(fullFileName);

            mockFS.Setup(m => m.File.ReadAllText(fullFileName))
                .Returns(_validSettingsJson);

            PathVerifyHack(mockFS);
        }

        private void PathVerifyHack(Mock<System.IO.Abstractions.IFileSystem> mockFileSystem)
        {
            //weird about the mockfilesystem... for somereason if you mock anything on Path, verifyall will fail saying these items were not fired, even though we did not setup a mock... 
            //  so i'm making a call to them just to satisfy the verifyall.. pretty much a hack
            var c = mockFileSystem.Object.Path.AltDirectorySeparatorChar;
            var c2 = mockFileSystem.Object.Path.DirectorySeparatorChar;
            var c3 = mockFileSystem.Object.Path.PathSeparator;
            var c4 = mockFileSystem.Object.Path.VolumeSeparatorChar;
            var cs = mockFileSystem.Object.Path.GetInvalidPathChars();
        }
    }
}