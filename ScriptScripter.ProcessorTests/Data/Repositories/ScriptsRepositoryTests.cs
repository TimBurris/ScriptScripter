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
    public class ScriptsRepositoryTests
    {
        private List<Models.Script> GetTestScripts()
        {
            return new List<Models.Script>()
            {
                new Models.Script(){
                    DeveloperName ="Sam Harris",
                    Notes="Create the Sysmail account table",
                    RevisionNumber=87,
                    ScriptDate=new DateTime(month:5, day:11,year:2000, hour:15, minute:37, second:23),
                    SQLStatement=                @"Select 1"
                },

                new Models.Script(){
                    DeveloperName ="Richard Dawkins",
                    Notes="Create the add alert sp",
                    RevisionNumber=88,
                    ScriptDate=new DateTime(month:3, day:29,year:2001, hour:4, minute:59, second:00),
                    SQLStatement= @"Select 2",
                },

                new Models.Script(){
                    DeveloperName ="Bill Nye",
                    Notes="insert some data into the sys mail account",
                    RevisionNumber=89,
                    ScriptDate=new DateTime(month:12, day:1,year:2001, hour:1, minute:7, second:3),
                    SQLStatement=@"Select 3"
                 },

                new Models.Script(){
                    DeveloperName ="Christopher Hitchens",
                    Notes="make a very important change",
                    RevisionNumber=90,
                    ScriptDate=new DateTime(month:12, day:1,year:2001, hour:1, minute:7, second:3),
                    SQLStatement=@"Select 4"
                },
            };
        }

        [TestMethod]
        public void ReadScript_non_xml_throws_InvalidScriptContainterContentsException()
        {
            /*************  arrange  ******************/
            var x = new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>();
            x.Add(@"C:\myfile.xml",
                new System.IO.Abstractions.TestingHelpers.MockFileData(textContents: "this text is not valid XML"));

            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(x);

            var repo = new ScriptsRepository(fileSystem);
            repo.ScriptContainer = new Models.ScriptContainer() { ScriptFilePath = @"C:\myfile.xml" };

            /*************    act    ******************/
            /*************  assert   ******************/
            Action a = () => repo.ReadScripts();

            a.Should().Throw<InvalidScriptContainterContentsException>();
        }


        [TestMethod]
        public void ReadScript_unrecognized_xml_throws_InvalidScriptContainterContentsException()
        {
            /*************  arrange  ******************/
            var x = new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>();
            x.Add(@"C:\myfile.xml",
                new System.IO.Abstractions.TestingHelpers.MockFileData(textContents: "<myroot><myscript></myscript></myroot>"));

            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(x);

            var repo = new ScriptsRepository(fileSystem);
            repo.ScriptContainer = new Models.ScriptContainer() { ScriptFilePath = @"C:\myfile.xml" };

            /*************    act    ******************/
            /*************  assert   ******************/
            Action a = () => repo.ReadScripts();

            a.Should().Throw<InvalidScriptContainterContentsException>();
        }

        [TestMethod]
        public void ReadScripts_returns_empty_if_file_not_exists()
        {
            /*************  arrange  ******************/
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
            {
            });

            var repo = new ScriptsRepository(fileSystem);
            repo.ScriptContainer = new Models.ScriptContainer() { ScriptFilePath = @"C:\myfile.xml" };

            /*************    act    ******************/
            var results = repo.ReadScripts();

            /*************  assert   ******************/
            results.Should().BeEmpty();
        }

        [TestMethod]
        public void WriteScripts_simpletest()
        {
            /*************  arrange  ******************/
            var repo = new ScriptsRepository(fileSystem: null);
            var stringWriter = new System.IO.StringWriter();
            var script = new Models.Script()
            {
                DeveloperName = "Dev Name",
                Notes = "My Note",
                RevisionNumber = 35,
                ScriptDate = new DateTime(month: 12, day: 1, year: 2001, hour: 1, minute: 7, second: 3),
                SQLStatement = "Select 1",
            };

            /*************    act    ******************/
            repo.WriteScripts(stringWriter, new Models.Script[] { script });
            var result = stringWriter.ToString();

            /*************  assert   ******************/
            result.Should().Be(@"<?xml version=""1.0"" encoding=""utf-16""?>
<Scripts xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Script>
    <RevisionNumber>35</RevisionNumber>
    <SQLStatement>Select 1</SQLStatement>
    <DeveloperName>Dev Name</DeveloperName>
    <Notes>My Note</Notes>
    <ScriptDate>2001-12-01T01:07:03</ScriptDate>
  </Script>
</Scripts>");
        }

        [TestMethod]
        public void Write_then_ReadScriptsTest()
        {
            /*************  arrange  ******************/
            var repo = new ScriptsRepository(fileSystem: null);
            var stringWriter = new System.IO.StringWriter();
            List<Models.Script> originalScripts = this.GetTestScripts();
            List<Models.Script> reloadedScripts;

            string fileContents;

            /*************    act    ******************/
            repo.WriteScripts(stringWriter, originalScripts);
            fileContents = stringWriter.ToString();
            using (var x = new System.IO.StringReader(fileContents))
            {
                reloadedScripts = repo.ReadScripts(x);
            }

            /*************  assert   ******************/
            reloadedScripts.Should().BeEquivalentTo(originalScripts);
        }

    }

    [TestClass]
    public class given_file_has_data
    {
        private Mock<ScriptsRepository> _mockRepo;
        private ScriptsRepository _repo;
        private List<Models.Script> _scriptList;
        private List<Models.Script> _savedScriptList;

        [TestInitialize]
        public void Init()
        {
            _scriptList = new List<Models.Script>()
            {
                new Models.Script(){
                    DeveloperName ="Sam Harris",
                    Notes="Create the Sysmail account table",
                    RevisionNumber=87,
                    ScriptDate=new DateTime(month:5, day:11,year:2000, hour:15, minute:37, second:23),
                    SQLStatement=                @"Select 1"
                },

                new Models.Script(){
                    DeveloperName ="Richard Dawkins",
                    Notes="Create the add alert sp",
                    RevisionNumber=88,
                    ScriptDate=new DateTime(month:3, day:29,year:2001, hour:4, minute:59, second:00),
                    SQLStatement= @"Select 2",
                },

                new Models.Script(){
                    DeveloperName ="Bill Nye",
                    Notes="insert some data into the sys mail account",
                    RevisionNumber=89,
                    ScriptDate=new DateTime(month:12, day:1,year:2001, hour:1, minute:7, second:3),
                    SQLStatement=@"Select 3"
                 },

                new Models.Script(){
                    DeveloperName ="Christopher Hitchens",
                    Notes="make a very important change",
                    RevisionNumber=90,
                    ScriptDate=new DateTime(month:12, day:1,year:2001, hour:1, minute:7, second:3),
                    SQLStatement=@"Select 4"
                },
            };

            //we are going to mock to prevent actual use of the filesystem, so we don't need to set it up
            System.IO.Abstractions.IFileSystem fileSystem = null;

            _mockRepo = new Mock<ScriptsRepository>(fileSystem);
            _repo = _mockRepo.Object;
            _mockRepo.CallBase = true;

            //mock read scripts to always return our list of scripts, so that we don't have to mock or care about the filesystem
            _mockRepo.Setup(m => m.ReadScripts())
                .Returns(() => _scriptList);

            //mock write scripts to capture was was written so tests can easily check what was saved.  how it's "written" to file is outside of the scope of these tests
            _mockRepo.Setup(m => m.WriteScripts(It.IsAny<IEnumerable<Models.Script>>()))
                .Callback<IEnumerable<Models.Script>>(lst => _savedScriptList = lst.ToList());
        }

        [TestMethod]
        public void GetAllScriptsTest()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var results = _repo.GetAllScripts();

            /*************  assert   ******************/
            results.Select(s => s.RevisionNumber).Should().BeEquivalentTo(new int[] { 87, 88, 89, 90 });
        }

        [TestMethod]
        public void GetAllScriptsAfterRevisionNumber_returns_later_scripts()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var results = _repo.GetAllScriptsAfterRevisionNumber(revisionNumber: 88);

            /*************  assert   ******************/
            results.Select(s => s.RevisionNumber).Should().BeEquivalentTo(new int[] { 89, 90 });
        }

        [TestMethod]
        public void GetAllScriptsAfterRevisionNumber_returns_empty_when_none_after()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var results = _repo.GetAllScriptsAfterRevisionNumber(revisionNumber: 90);

            /*************  assert   ******************/
            results.Should().BeEmpty();
        }

        [TestMethod]
        public void GetLastScriptTest()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var result = _repo.GetLastScript();

            /*************  assert   ******************/
            result.RevisionNumber.Should().Be(90);
        }

        [TestMethod]
        public void GetScriptByRevisionNumber_returns_script()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var result = _repo.GetScriptByRevisionNumber(89);

            /*************  assert   ******************/
            result.RevisionNumber.Should().Be(89);
        }

        [TestMethod]
        public void GetScriptByRevisionNumber_returns_null_when_not_found()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var result = _repo.GetScriptByRevisionNumber(95);

            /*************  assert   ******************/
            result.Should().BeNull();
        }

        [TestMethod]
        public void AddNewScript_sets_revision_number()
        {
            /*************  arrange  ******************/
            var newScript = new Models.Script();

            /*************    act    ******************/
            var result = _repo.AddNewScript(newScript);

            /*************  assert   ******************/
            result.RevisionNumber.Should().Be(91);

        }

        [TestMethod]
        public void AddNewScript_writes_to_file()
        {
            /*************  arrange  ******************/
            var newScript = new Models.Script();

            /*************    act    ******************/
            var result = _repo.AddNewScript(newScript);

            /*************  assert   ******************/
            _savedScriptList.Should().Contain(_scriptList, because: "all original scripts should still be in the saved list");
            _savedScriptList.Should().Contain(result, because: "new script be in the saved list");
        }

        [TestMethod()]
        public void UpdateScript_replaces_script_in_file()
        {
            /*************  arrange  ******************/
            var orginalScript = _scriptList[1];

            var script = new Models.Script()
            {
                RevisionNumber = orginalScript.RevisionNumber,
                SQLStatement = "My Updated Script"
            };

            /*************    act    ******************/
            var result = _repo.UpdateScript(script);

            /*************  assert   ******************/
            _savedScriptList.Should().NotContain(orginalScript);
            _savedScriptList.Should().Contain(result);
            result.SQLStatement.Should().Be("My Updated Script");
        }
    }

    [TestClass]
    public class given_file_has_no_data
    {
        private System.IO.Abstractions.IFileSystem _fs;
        private ScriptsRepository _repo;
        private string _mockedContainerFile = @"c:\temp\myfolder\myscripts.xml";

        [TestInitialize]
        public void Init()
        {
            _fs = this.GetMockedFileSystem();
            _repo = new ScriptsRepository(fileSystem: _fs);
            _repo.ScriptContainer = new Models.ScriptContainer() { ScriptFilePath = _mockedContainerFile };
        }

        private System.IO.Abstractions.IFileSystem GetMockedFileSystem()
        {
            //these contents are the same as what is in out "GetTestScripts"

            string contents = @"";
            return new System.IO.Abstractions.TestingHelpers.MockFileSystem(new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                                {
                                    { _mockedContainerFile, new System.IO.Abstractions.TestingHelpers.MockFileData(contents) },
                                });
        }

        [TestMethod]
        public void GetAllScripts_returns_empty()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var results = _repo.GetAllScripts();

            /*************  assert   ******************/
            results.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllScriptsAfterRevisionNumber_returns_empty()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var results = _repo.GetAllScriptsAfterRevisionNumber(revisionNumber: 88);

            /*************  assert   ******************/
            results.Should().BeEmpty();
        }

        [TestMethod]
        public void GetLastScript_returns_null()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var result = _repo.GetLastScript();

            /*************  assert   ******************/
            result.Should().BeNull();
        }


        [TestMethod]
        public void GetScriptByRevisionNumber_returns_null()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var result = _repo.GetScriptByRevisionNumber(89);

            /*************  assert   ******************/
            result.Should().BeNull();
        }

        [TestMethod]
        public void AddNewScript_sets_revision_number()
        {
            /*************  arrange  ******************/
            var newScript = new Models.Script();

            /*************    act    ******************/
            var result = _repo.AddNewScript(newScript);

            /*************  assert   ******************/
            result.RevisionNumber.Should().Be(1);
        }

        [TestMethod]
        public void AddNewScript_writes_to_file()
        {
            /*************  arrange  ******************/
            var newScript = new Models.Script();

            /*************    act    ******************/
            _repo.AddNewScript(newScript);


            /*************  assert   ******************/
            _fs.File.ReadAllText(_mockedContainerFile)
                .Should().Contain($"<RevisionNumber>1</RevisionNumber>");

        }


        [TestMethod]
        public void ExportScripts()
        {
            //*************  arrange  ******************
            _repo = new ScriptsRepository(new System.IO.Abstractions.FileSystem());
            _repo.ScriptContainer = new Models.ScriptContainer()
            {
                ScriptFilePath = @"C:\Code\ClientProjects\CinemaInventory\DBScripts_MovieMunchDB.xml"
            };
            var scripts = _repo.GetAllScripts();

            foreach (var s in scripts)
            {
                System.Diagnostics.Debug.WriteLine("/***********************************************************************");
                System.Diagnostics.Debug.WriteLine(s.Notes);
                System.Diagnostics.Debug.WriteLine("***********************************************************************/");
                System.Diagnostics.Debug.WriteLine(s.SQLStatement);
                System.Diagnostics.Debug.WriteLine(string.Empty);
                System.Diagnostics.Debug.WriteLine(string.Empty);
                System.Diagnostics.Debug.WriteLine(string.Empty);
                System.Diagnostics.Debug.WriteLine(string.Empty);
                System.Diagnostics.Debug.WriteLine(string.Empty);


            }


            //*************    act    ******************


            //*************  assert   ******************


        }
    }
}