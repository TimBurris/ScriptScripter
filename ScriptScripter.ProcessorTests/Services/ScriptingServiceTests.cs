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
    public class ScriptingServiceTests
    {
        #region GetDatabaseScriptState Tests

        [TestMethod]
        public void GetDatabaseScriptState_newer_script_returns_out_of_date()
        {
            //arrange
            var script = new Data.Models.Script { RevisionNumber = 9 };
            var rev = new Data.Models.Revision { RevisionNumber = 8 };

            //act
            var service = new ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            var result = service.GetDatabaseScriptState(script, rev);

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.OutOfdate);

        }
        [TestMethod]
        public void GetDatabaseScriptState_null_revision_returns_out_of_date()
        {
            //arrange
            var script = new Data.Models.Script { RevisionNumber = 9 };
            Data.Models.Revision rev = null;

            //act
            var service = new ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            var result = service.GetDatabaseScriptState(script, rev);

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.OutOfdate);

        }

        [TestMethod]
        public void GetDatabaseScriptState_older_script_returns_newer()
        {
            //arrange
            var script = new Data.Models.Script { RevisionNumber = 7 };
            var rev = new Data.Models.Revision { RevisionNumber = 8 };

            //act
            var service = new ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            var result = service.GetDatabaseScriptState(script, rev);

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.Newer);
        }

        [TestMethod]
        public void GetDatabaseScriptState_null_script_returns_newer()
        {
            //arrange
            Data.Models.Script script = null;
            var rev = new Data.Models.Revision { RevisionNumber = 8 };

            //act
            var service = new ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            var result = service.GetDatabaseScriptState(script, rev);

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.Newer);
        }

        [TestMethod]
        public void GetDatabaseScriptState_equal_script_returns_up_to_date()
        {
            //arrange
            var script = new Data.Models.Script { RevisionNumber = 8 };
            var rev = new Data.Models.Revision { RevisionNumber = 8 };

            //act
            var service = new ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            var result = service.GetDatabaseScriptState(script, rev);

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.UpToDate);
        }

        [TestMethod]
        public void GetDatabaseScriptState_null_script_and_null_revision_returns_up_to_date()
        {
            //arrange
            Data.Models.Script script = null;
            Data.Models.Revision rev = null;

            //act
            var service = new ScriptingService(dbupdaterFactory: null, scriptRepoFactory: null, configurationRepository: null, revisionRepository: null);
            var result = service.GetDatabaseScriptState(script, rev);

            //assert
            result.Should().Be(Contracts.DatabaseScriptStates.UpToDate);
        }

        //TODO: script null checks!

        #endregion
    }
}