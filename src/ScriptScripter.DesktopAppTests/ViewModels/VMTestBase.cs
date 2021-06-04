using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace ScriptScripter.DesktopApp.ViewModels.Tests
{
    public class VMTestBase
    {
        protected DesktopAppTests.RepoMockery RepoMocks { get; set; } = new DesktopAppTests.RepoMockery();
        protected DesktopAppTests.ServiceMockery ServiceMocks { get; set; } = new DesktopAppTests.ServiceMockery();

        protected Mock<NinjaMvvm.Wpf.Abstractions.INavigator> MockNavigator { get; set; }

        [TestInitialize]
        public void BaseTestInit()
        {
            SetupMocks();
        }

        protected virtual void SetupMocks()
        {
            MockNavigator = new Mock<NinjaMvvm.Wpf.Abstractions.INavigator>(Moq.MockBehavior.Strict);
            RepoMocks.SetupMocks(Moq.MockBehavior.Strict);
            ServiceMocks.SetupMocks(Moq.MockBehavior.Strict);
        }

        protected virtual void VerifyAllMocks()
        {
            MockNavigator.VerifyAll();
            RepoMocks.VerifyAllMocks();
            ServiceMocks.VerifyAllMocks();
        }

    }
}
