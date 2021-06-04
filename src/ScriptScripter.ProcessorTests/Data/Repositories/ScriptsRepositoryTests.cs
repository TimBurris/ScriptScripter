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
                    ScriptId=new Guid("4f058d66-92a8-4c02-8620-48e8a73768f5"),
                    ScriptDate=new DateTime(month:5, day:11,year:2000, hour:15, minute:37, second:23),
                    SqlStatement=                @"Select 1"
                },

                new Models.Script(){
                    DeveloperName ="Richard Dawkins",
                    Notes="Create the add alert sp",
                    ScriptId=new Guid("fdee992d-7ae3-45ef-bd91-0270074ca457"),
                    ScriptDate=new DateTime(month:3, day:29,year:2001, hour:4, minute:59, second:00),
                    SqlStatement= @"Select 2",
                },

                //this script is inserted in position 3, BUT, it's date makes it the LAST script
                new Models.Script(){
                    DeveloperName ="Bill Nye",
                    Notes="insert some data into the sys mail account",
                    ScriptId=new Guid("86111dff-6e26-4641-b83a-5cfe961d3e46"),
                    ScriptDate=new DateTime(month:12, day:31,year:2001, hour:1, minute:7, second:3),
                    SqlStatement=@"Select 3"
                 },

                new Models.Script(){
                    DeveloperName ="Christopher Hitchens",
                    Notes="make a very important change",
                    ScriptId=new Guid("3343ae7d-f44d-4089-ba62-6c3c182e768a"),
                    ScriptDate=new DateTime(month:12, day:1,year:2001, hour:1, minute:7, second:3),
                    SqlStatement=@"Select 4"
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
            repo.ScriptFilePath = @"C:\myfile.xml";

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
            repo.ScriptFilePath = @"C:\myfile.xml";

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
            repo.ScriptFilePath = @"C:\myfile.xml";

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
                ScriptId = new Guid("a8693068-e251-41ca-8afe-5ae6badeeaad"),
                ScriptDate = new DateTimeOffset(month: 12, day: 1, year: 2001, hour: 1, minute: 7, second: 3, offset: TimeSpan.FromHours(-5)),
                SqlStatement = "Select 1",
            };

            /*************    act    ******************/
            repo.WriteScripts(stringWriter, new Models.Script[] { script });
            var result = stringWriter.ToString();


            /*************  assert   ******************/
            result.Should().Be(@"<?xml version=""1.0"" encoding=""utf-16""?>
<Scripts xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Script>
    <ScriptId>a8693068-e251-41ca-8afe-5ae6badeeaad</ScriptId>
    <SqlStatement>Select 1</SqlStatement>
    <DeveloperName>Dev Name</DeveloperName>
    <Notes>My Note</Notes>
    <ScriptDate>2001-12-01T01:07:03.0000000-05:00</ScriptDate>
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
                    ScriptId=new Guid("4f058d66-92a8-4c02-8620-48e8a73768f5"),
                    ScriptDate=new DateTime(month:5, day:11,year:2000, hour:15, minute:37, second:23),
                    SqlStatement=                @"Select 1"
                },

                new Models.Script(){
                    DeveloperName ="Richard Dawkins",
                    Notes="Create the add alert sp",
                    ScriptId=new Guid("fdee992d-7ae3-45ef-bd91-0270074ca457"),
                    ScriptDate=new DateTime(month:3, day:29,year:2001, hour:4, minute:59, second:00),
                    SqlStatement= @"Select 2",
                },

                new Models.Script(){
                    DeveloperName ="Bill Nye",
                    Notes="insert some data into the sys mail account",
                    ScriptId=new Guid("86111dff-6e26-4641-b83a-5cfe961d3e46"),
                    ScriptDate=new DateTime(month:12, day:1,year:2002, hour:4, minute:58, second:1),
                    SqlStatement=@"Select 3"
                 },

                new Models.Script(){
                    DeveloperName ="Christopher Hitchens",
                    Notes="make a very important change",
                    ScriptId=new Guid("3343ae7d-f44d-4089-ba62-6c3c182e768a"),
                    ScriptDate=new DateTime(month:12, day:1,year:2001, hour:1, minute:7, second:3),
                    SqlStatement=@"Select 4"
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
            results.Select(s => s.ScriptId).Should().BeEquivalentTo(new Guid[] { new Guid("4f058d66-92a8-4c02-8620-48e8a73768f5"), new Guid("fdee992d-7ae3-45ef-bd91-0270074ca457"), new Guid("3343ae7d-f44d-4089-ba62-6c3c182e768a"), new Guid("86111dff-6e26-4641-b83a-5cfe961d3e46") });
        }


        [TestMethod]
        public void GetLastScriptTest()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var result = _repo.GetLastScript();

            /*************  assert   ******************/
            result.ScriptId.Should().Be(new Guid("86111dff-6e26-4641-b83a-5cfe961d3e46"));
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
                ScriptId = orginalScript.ScriptId,
                SqlStatement = "My Updated Script"
            };

            /*************    act    ******************/
            var result = _repo.UpdateScript(script);

            /*************  assert   ******************/
            _savedScriptList.Should().NotContain(orginalScript);
            _savedScriptList.Should().Contain(result);
            result.SqlStatement.Should().Be("My Updated Script");
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
            _repo.ScriptFilePath = _mockedContainerFile;
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
        public void GetLastScript_returns_null()
        {
            /*************  arrange  ******************/

            /*************    act    ******************/
            var result = _repo.GetLastScript();

            /*************  assert   ******************/
            result.Should().BeNull();
        }

        [TestMethod]
        public void AddNewScript_writes_to_file()
        {
            /*************  arrange  ******************/
            var newScript = new Models.Script();

            /*************    act    ******************/
            var result = _repo.AddNewScript(newScript);


            /*************  assert   ******************/
            _fs.File.ReadAllText(_mockedContainerFile)
                .Should().Contain($"<ScriptId>{result.ScriptId}</ScriptId>");

        }


        [TestMethod]
        public void ExportScripts()
        {
            //*************  arrange  ******************
            _repo = new ScriptsRepository(new System.IO.Abstractions.FileSystem());
            _repo.ScriptFilePath = @"C:\Code\ClientProjects\CinemaInventory\DBScripts_MovieMunchDB.xml";
            var scripts = _repo.GetAllScripts();

            foreach (var s in scripts)
            {
                System.Diagnostics.Debug.WriteLine("/***********************************************************************");
                System.Diagnostics.Debug.WriteLine(s.Notes);
                System.Diagnostics.Debug.WriteLine("***********************************************************************/");
                System.Diagnostics.Debug.WriteLine(s.SqlStatement);
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