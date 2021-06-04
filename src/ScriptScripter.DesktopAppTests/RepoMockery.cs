using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace ScriptScripter.DesktopAppTests
{
    public class RepoMockery
    {
        public Mock<Processor.Data.Contracts.IScriptRepositoryFactory> MockScriptRepositoryFactory { get; set; }
        public Mock<Processor.Data.Contracts.IScriptContainerRepository> MockScriptContainerRepo { get; set; }
        public Mock<Processor.Data.Contracts.IRevisionRepository> MockRevisionRepo { get; set; }
        public List<Mock<Processor.Data.Contracts.IScriptsRepository>> MockScriptsRepos { get; } = new List<Mock<Processor.Data.Contracts.IScriptsRepository>>();
        public Mock<Processor.Data.Contracts.IConfigurationRepository> MockConfigurationRepo { get; set; }

        public void RegisterNewScriptRepoMock(Processor.Data.Models.ScriptContainer scriptContainer)
        {
            var repo = new Mock<Processor.Data.Contracts.IScriptsRepository>(MockScriptRepositoryFactory.Behavior);
            MockScriptsRepos.Add(repo);

            MockScriptRepositoryFactory.Setup(m => m.GetScriptsRepository(scriptContainer.ScriptFilePath))
                .Returns(repo.Object);
        }

        public void RegisterNewScriptRepoMocks(IEnumerable<Processor.Data.Models.ScriptContainer> scriptContainers)
        {
            foreach (var container in scriptContainers)
                RegisterNewScriptRepoMock(container);
        }

        public void SetupMocks(MockBehavior mockBehavior)
        {
            MockScriptRepositoryFactory = new Mock<Processor.Data.Contracts.IScriptRepositoryFactory>(mockBehavior);
            MockScriptContainerRepo = new Mock<Processor.Data.Contracts.IScriptContainerRepository>(mockBehavior);
            MockRevisionRepo = new Mock<Processor.Data.Contracts.IRevisionRepository>(mockBehavior);
            MockConfigurationRepo = new Mock<Processor.Data.Contracts.IConfigurationRepository>(mockBehavior);

            //****** DON'T FORGET TO ADD TO VERIFY ALL!!!! *******
        }

        public void VerifyAllMocks()
        {
            //Repos
            MockScriptRepositoryFactory.VerifyAll();
            MockScriptContainerRepo.VerifyAll();
            MockRevisionRepo.VerifyAll();
            MockConfigurationRepo.VerifyAll();

            foreach (var repo in MockScriptsRepos)
                repo.VerifyAll();
        }

    }
}
