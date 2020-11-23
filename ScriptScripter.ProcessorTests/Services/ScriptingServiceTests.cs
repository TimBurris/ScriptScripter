using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ScriptScripter.Processor.Services.Tests
{

    [TestClass()]
    public class ScriptingService_Tests
    {

    }

    [TestClass()]
    public class ScriptingService_GetDatabaseScriptState_Tests
    {
        private ScriptingService _service;

        private Mock<Processor.Data.Contracts.IRevisionRepository> _mockRevisionRepo = new Mock<Data.Contracts.IRevisionRepository>();
        private Mock<Processor.Data.Contracts.IScriptRepositoryFactory> _mockScriptRepoFactory = new Mock<Data.Contracts.IScriptRepositoryFactory>();
        private Mock<Processor.Data.Contracts.IScriptsRepository> _mockScriptRepo = new Mock<Data.Contracts.IScriptsRepository>();


        //in y our tests, simply fill these two lists with data to test against
        private List<Data.Models.Revision> _revisions = new List<Data.Models.Revision>();
        private List<Data.Models.Script> _scripts = new List<Data.Models.Script>();


        //path and params don't matter, we've mocked the repos
        private string _inputScriptFilePath = "mypath";
        private Processor.Data.Models.DatabaseConnectionParameters _inputConnectionParams = new Processor.Data.Models.DatabaseConnectionParameters();

        [TestInitialize]
        public void Init()
        {
            _mockRevisionRepo.Setup(m => m.GetAll(_inputConnectionParams))
                .Returns(() => _revisions);

            _mockScriptRepoFactory.Setup(m => m.GetScriptsRepository(_inputScriptFilePath))
                .Returns(() => _mockScriptRepo.Object);

            _mockScriptRepo.Setup(m => m.GetAllScripts())
                .Returns(() => _scripts);

            _service = new ScriptingService(dbupdaterFactory: null, scriptRepoFactory: _mockScriptRepoFactory.Object, configurationRepository: null, revisionRepository: _mockRevisionRepo.Object);

            //go ahed and prime the lists to have equal rev and scripts
            this.FillEqualScriptsAndRevisions();
        }

        public Contracts.DatabaseScriptStates Act()
        {
            return _service.GetDatabaseScriptState(databaseConnectionParams: _inputConnectionParams, scriptFilePath: _inputScriptFilePath);
        }
        private void FillEqualScriptsAndRevisions()
        {

            for (int i = 0; i < 5; i++)
            {
                var g = Guid.NewGuid();
                var dt = DateTime.UtcNow.AddDays(i);
                _scripts.Add(new Data.Models.Script() { ScriptId = g, ScriptDate = dt });
                _revisions.Add(new Data.Models.Revision() { ScriptId = g, ScriptDate = dt });

            }

        }

        [TestMethod]
        public void script_not_in_revisions_returns_out_of_date()
        {
            //arrange
            _revisions.RemoveAt(2);

            //act
            var result = this.Act();

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.OutOfdate);

        }

        [TestMethod]
        public void no_revisions_returns_out_of_date()
        {
            //arrange
            _revisions.Clear();

            //act
            var result = this.Act();

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.OutOfdate);
        }

        [TestMethod]
        public void revision_not_in_script_list_returns_newwer()
        {
            //arrange
            _scripts.RemoveAt(3);

            //act
            var result = this.Act();

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.Newer);
        }

        [TestMethod]
        public void no_scripts_returns_newer()
        {
            //arrange
            _scripts.Clear();

            //act
            var result = this.Act();

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.Newer);
        }

        [TestMethod]
        public void equal_script_returns_up_to_date()
        {
            //arrange

            //act
            var result = this.Act();


            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.UpToDate);
        }

        [TestMethod]
        public void null_script_and_null_revision_returns_up_to_date()
        {
            //arrange
            _scripts.Clear();
            _revisions.Clear();

            //act
            var result = this.Act();


            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.UpToDate);
        }
    }
}