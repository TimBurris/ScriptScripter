using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ScriptScripter.DesktopApp.ViewModels.Tests
{
    [TestClass()]
    public class DatabaseListViewModelTests : VMTestBase
    {
        [TestMethod()]
        public void Load_should_load_data_for_all_databases()
        {
            var containers = new List<Processor.Data.Models.ScriptContainer>() {
                     new Processor.Data.Models.ScriptContainer() {DatabaseName="DB1",ScriptFilePath="C:\\DB1" },
                     new Processor.Data.Models.ScriptContainer() {DatabaseName="DB2",ScriptFilePath="C:\\DB2" },
                     new Processor.Data.Models.ScriptContainer() {DatabaseName="DB3" ,ScriptFilePath="C:\\DB3"}
             };
            RepoMocks.MockScriptContainerRepo.Setup(m => m.GetAll())
                 .Returns(containers);

            this.RepoMocks.RegisterNewScriptRepoMocks(containers);
            var d1 = DateTimeOffset.UtcNow;
            var d2 = DateTimeOffset.UtcNow.AddDays(1);
            RepoMocks.MockScriptsRepos[0].Setup(m => m.GetLastScript())
                .Returns(new Processor.Data.Models.Script() { DeveloperName = "D1", ScriptDate = d1, ScriptId = new Guid("4f058d66-92a8-4c02-8620-48e8a73768f5") });
            RepoMocks.MockScriptsRepos[1].Setup(m => m.GetLastScript())
                .Returns(new Processor.Data.Models.Script() { DeveloperName = "D2", ScriptDate = d2, ScriptId = new Guid("fdee992d-7ae3-45ef-bd91-0270074ca457") });
            RepoMocks.MockScriptsRepos[2].Setup(m => m.GetLastScript())
                .Returns((Processor.Data.Models.Script)null); //makes usre it's fine wiht NULL (meaning no scripts in the file yet)

            var vm = new DatabaseListViewModel(fileSystem: null,
                navigator: null,
                viewModelFaultlessService: new ViewModelFaultlessService(navigator: null),
                scriptsRepoFactory: RepoMocks.MockScriptRepositoryFactory.Object,
                scriptsContainerRepository: RepoMocks.MockScriptContainerRepo.Object,
                eventNotificationService: null, 
                logger: null);

            vm.ReloadDataAsync().Wait();

            vm.LineItems.Count.Should().Be(3);
            vm.LineItems[0].DatabaseName.Should().Be("DB1");
            vm.LineItems[0].ScriptDate.Should().Be(d1.LocalDateTime.ToString());

            vm.LineItems[1].DatabaseName.Should().Be("DB2");
            vm.LineItems[1].ScriptDate.Should().Be(d2.LocalDateTime.ToString());

            vm.LineItems[2].DatabaseName.Should().Be("DB3");
            vm.LineItems[2].ScriptDate.Should().BeNull();

            VerifyAllMocks();
        }
    }
}